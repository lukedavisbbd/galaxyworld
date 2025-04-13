using System.Security.Authentication;
using System.Net.Sockets;
using System.Net;
using System.Web;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using GalaxyWorld.Core.Models;
using GalaxyWorld.Cli.Properties;
using System.Net.Http.Json;

namespace GalaxyWorld.Cli.Services;

public class AuthService
{
    public static string AuthToken { get; private set; } = "";

    private const string CLIENT_ID = "238589488826-vte0lmfp1tnvfacd9gpfndtme3rgd527.apps.googleusercontent.com";
    private const string BASE_URL = "https://localhost:4433";
    private const string BLUE = "";
    private const string RED = "";
    private const string GREEN = "";
    private const string RESET = "";

    public static bool LoginOAuth2() {
        var server = new Socket(SocketType.Stream, ProtocolType.Tcp);
        server.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        server.Listen();

        var port = (server.LocalEndPoint as IPEndPoint)?.Port ?? throw new AuthenticationException("Failed to bind socket.");
        var securityToken = Random.Shared.NextInt64(int.MaxValue, long.MaxValue);
        var callbackUri = $"http://{IPAddress.Loopback}:{port}";

        string errorMessage = "";

        Socket? socket = null;
        
        try
        {
            RequestOAuth2(callbackUri, securityToken);
            socket = server.Accept();
            var code = ExtractCode(socket, securityToken);
            var token = RequestIdToken(code, callbackUri);
            AuthToken = token;
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
            Console.WriteLine(RED + "Signed in failed." + RESET);
        } else
        {
            responseBytes = (byte[]?)Resources.ResourceManager.GetObject("AuthSuccess");
            Console.WriteLine(GREEN + "Succesfully signed in!" + RESET);
        }

        if (responseBytes == null) throw new InvalidOperationException("response html missing");

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

        return errorMessage.Length == 0;
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

        Console.WriteLine(BLUE + "If the browser has not opened, please use this link to sign in: " + RESET + oauth2RequestUri);

        try
        {
            Process.Start(new ProcessStartInfo(oauth2RequestUri) { UseShellExecute = true });
        }
        catch (Exception)
        {
            Console.WriteLine(RED + "Failed to open browser." + RESET);
        }
    }

    private static string ExtractCode(Socket socket, long securityToken)
    {
        var buffer = new byte[1024];
        var count = socket.Receive(buffer);
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

    public static string RequestIdToken(string code, string callbackUri)
    {
        // Make request to F-Up Board API with auth code to receive JWT
        var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, BASE_URL + "/auth");
        request.Content = JsonContent.Create(new AuthRequest
        {
            Code = code,
            Uri = callbackUri,
        });

        string responseBody;
        
        try
        {
            var response = client.Send(request);
            responseBody = new StreamReader(response.Content.ReadAsStream(), Encoding.UTF8).ReadToEnd();
        }
        catch (HttpRequestException)
        {
            throw new AuthenticationException("Failed to request Google JWT.");
        }

        AuthResponse authResponse;
        try
        {
            var authResp = JsonSerializer.Deserialize<AuthResponse>(responseBody, Program.JsonOptions);
            
            if (authResp == null)
                throw new JsonException();
            
            authResponse = authResp;
        }
        catch (JsonException)
        {
            throw new AuthenticationException("Failed to parse response from Google.");
        }

        return authResponse.IdToken;
    }
}
