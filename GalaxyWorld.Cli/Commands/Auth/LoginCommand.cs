using GalaxyWorld.Cli.ApiHandler;
using GalaxyWorld.Cli.Exceptions;
using GalaxyWorld.Cli.Properties;
using GalaxyWorld.Core.Models;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Web;
using Spectre.Console;
using Spectre.Console.Cli;

namespace GalaxyWorld.Cli.Commands.Auth;

public class LoginCommand : AsyncCommand
{
    private const string CLIENT_ID = "238589488826-vte0lmfp1tnvfacd9gpfndtme3rgd527.apps.googleusercontent.com";
    
    private static string configPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GalaxyWorld");
    private static string idTokenPath = Path.Join(configPath, "id_token");

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        try
        {
            await LoginOAuth2();
            return 0;
        }
        catch (AuthenticationException e)
        {
            AnsiConsole.MarkupLine(e.Message);
            return 1;
        }
    }

    public static async Task<string?> LoginFromFile()
    {
        try
        {
            var idToken = await File.ReadAllTextAsync(idTokenPath);
            var client = new ApiClient(idToken);

            var roles = (await client.GetAuth()).Roles.Select(role => $"'{role}'").ToArray();
            
            return idToken;
        }
        catch (Exception) {
            return null;
        }
    }

    public static async Task LoginOAuth2()
    {
        var token = await LoginFromFile();

        if (token != null) {
            ApiClient.DefaultAuthToken = token;
            AnsiConsole.MarkupLine($"[green]Logged in.[/]");
            return;
        }

        var server = new Socket(SocketType.Stream, ProtocolType.Tcp);
        server.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        server.Listen();

        var port = (server.LocalEndPoint as IPEndPoint)?.Port ?? throw new AppException("Failed to bind socket.");
        var securityToken = Random.Shared.NextInt64(int.MaxValue, long.MaxValue);
        var callbackUri = $"http://{IPAddress.Loopback}:{port}";

        string errorMessage = "";

        Socket? socket = null;

        try
        {
            RequestOAuth2(callbackUri, securityToken);
            socket = await server.AcceptAsync();
            var code = await ExtractCode(socket, securityToken);
            token = await RequestIdToken(code, callbackUri);
        }
        catch (AuthenticationException e)
        {
            errorMessage = e.Message;
        }

        // Generate response for user.
        byte[]? responseBytes;
        if (errorMessage.Length > 0)
        {
            responseBytes = (byte[]?)Resources.ResourceManager.GetObject("AuthFailure");
            AnsiConsole.MarkupLine("[red]Sign in failed.[/]");
        }
        else
        {
            responseBytes = (byte[]?)Resources.ResourceManager.GetObject("AuthSuccess");
            AnsiConsole.MarkupLine("[green]Successfully signed in![/]");
        }

        if (responseBytes == null) throw new AppException("Response HTML missing.");

        var responseHtml = Encoding.UTF8.GetString(responseBytes);

        responseHtml = responseHtml.Replace("{{errorMessage}}", errorMessage);

        var responseCode = errorMessage.Length > 0 ? "400 BAD REQUEST" : "200 OK";
        var responseHeaders = "HTTP/1.1 " + responseCode + "\r\n" +
                        "Server: F-Up Board OAuth2 Callback Server\r\n" +
                        "Content-Length: " + responseHtml.Length + "\r\n" +
                        "Content-Type: text/html\r\n" +
                        "Connection: Closed\r\n\r\n";

        socket?.Send(Encoding.UTF8.GetBytes(responseHeaders + responseHtml));
        socket?.Close();
        server.Close();

        if (errorMessage.Length > 0)
            throw new AppException(errorMessage);

        try
        {
            Directory.CreateDirectory(configPath);
            File.WriteAllText(idTokenPath, token);
            ApiClient.DefaultAuthToken = token;
        }
        catch (IOException) { }
    }

    private static void RequestOAuth2(string callbackUri, long securityToken)
    {
        var callbackUriEncoded = HttpUtility.UrlEncode(callbackUri);

        var oauth2RequestUri = "https://accounts.google.com/o/oauth2/v2/auth" +
                "?scope=email%20profile" +
                "&response_type=code" +
                "&state=fupboard%3Asecurity_token%3A" + securityToken +
                "&redirect_uri=" + callbackUriEncoded +
                "&client_id=" + CLIENT_ID;

        AnsiConsole.MarkupLine("[blue]If the browser has not opened, please use this link to sign in: [/]" + oauth2RequestUri);

        try
        {
            Process.Start(new ProcessStartInfo(oauth2RequestUri) { UseShellExecute = true });
        }
        catch (Exception) { }
    }

    private static async Task<string> ExtractCode(Socket socket, long securityToken)
    {
        var buffer = new byte[1024];
        var count = await socket.ReceiveAsync(buffer);
        var input = Encoding.UTF8.GetString(buffer, 0, count);

        var callbackRoute = input.Split("\r\n", 2)[0];

        string[] queryParams = [];
        if (callbackRoute != null)
        {
            var splits = callbackRoute.Split(" ");
            if (splits.Length >= 2)
                queryParams = splits[1].Split("&");
            else
                throw new AuthenticationException("Invalid request.");
        }
        else
        {
            throw new AuthenticationException("Empty request.");
        }

        bool foundSecurityToken = false;

        string code = "";

        for (int i = 0; i < queryParams.Length; i++)
        {
            if (queryParams[i].Equals("/?state=fupboard%3Asecurity_token%3A" + securityToken))
            {
                foundSecurityToken = true;
            }
            if (queryParams[i].StartsWith("code="))
            {
                code = HttpUtility.UrlDecode(queryParams[i].Substring(5));
                break;
            }
        }

        if (code == null)
            throw new AuthenticationException("Code not found.");

        if (!foundSecurityToken)
            throw new AuthenticationException("Missing/incorrect security token.");

        return code;
    }

    public static async Task<string> RequestIdToken(string code, string callbackUri)
    {
        var client = new ApiClient();

        try
        {
            var response = await client.PostAuth(new AuthRequest
            {
                Code = code,
                Uri = callbackUri,
            });

            return response.IdToken;
        }
        catch (AppException e)
        {
            throw new AuthenticationException("Failed to request Google JWT. " + e.Message);
        }
    }
}
