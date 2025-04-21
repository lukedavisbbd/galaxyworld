using Spectre.Console.Cli;
using GalaxyWorld.Cli.Commands.Auth;
using GalaxyWorld.Cli.Commands.Stars;
using GalaxyWorld.Cli.Commands.Catalogues;
using GalaxyWorld.Cli.Commands.Constellations;
using GalaxyWorld.Cli.Commands.CatalogueEntries;

namespace GalaxyWorld.Cli.Commands.Setup;

public static class CommandConfigurator
{
    public static void Configure(IConfigurator config)
    {
        config.AddBranch("stars", stars =>
        {
            stars.AddCommand<GetAllStarsCommand>("get-all").WithDescription("List all stars");
            stars.AddCommand<GetStarByIdCommand>("get").WithDescription("Fetch a star by ID");
            stars.AddCommand<GetStarPlanetsCommand>("planets").WithDescription("Fetch star planetary system information");
            stars.AddCommand<CreateStarCommand>("create").WithDescription("Register a star");
            stars.AddCommand<StarBulkUploadCommand>("upload-bulk").WithDescription("Upload a ATHYG format CSV file of stars");
            stars.AddCommand<UpdateStarCommand>("update").WithDescription("Update a star by ID");
            stars.AddCommand<DeleteStarCommand>("delete").WithDescription("Delete a star by ID");
        });

        config.AddBranch("catalogues", cat =>
        {
            cat.AddCommand<GetAllCataloguesCommand>("get-all").WithDescription("List all catalogues");
            cat.AddCommand<GetCatalogueByIdCommand>("get").WithDescription("Fetch a specific catalogue by ID");
            cat.AddCommand<GetCatalogueStarsCommand>("stars").WithDescription("List all star entries");
            cat.AddCommand<CreateCatalogueCommand>("create").WithDescription("Create a new catalogue");
            cat.AddCommand<UpdateCatalogueCommand>("update").WithDescription("Update an existing catalogue");
            cat.AddCommand<DeleteCatalogueCommand>("delete").WithDescription("Delete a catalogue by ID");
        });

        config.AddBranch("constellations", con =>
        {
            con.AddCommand<GetAllConstellationsCommand>("get-all").WithDescription("List all constellations");
            con.AddCommand<GetConstellationByIdCommand>("get").WithDescription("Fetch a constellation by ID");
            con.AddCommand<GetConstellationStarsCommand>("stars").WithDescription("Fetch all stars in a constellation");
            con.AddCommand<DrawConstellationCommand>("draw").WithDescription("Draw a constellation by ID");
            con.AddCommand<CreateConstellationCommand>("create").WithDescription("Create a new constellation");
            con.AddCommand<UpdateConstellationCommand>("update").WithDescription("Update a constellation by ID");
            con.AddCommand<DeleteConstellationCommand>("delete").WithDescription("Delete a constellation by ID");
        });

        config.AddBranch("entries", ent =>
        {
            ent.AddCommand<GetCatalogueEntryCommand>("get").WithDescription("Get a specific entry for catalogue ID and star ID");
            ent.AddCommand<CreateCatalogueEntryCommand>("create").WithDescription("Create an entry linking a star to a catalogue");
            ent.AddCommand<UpdateCatalogueEntryCommand>("update").WithDescription("Update entry ID or designation for a catalogue entry");
            ent.AddCommand<DeleteCatalogueEntryCommand>("delete").WithDescription("Delete a star's entry in a catalogue");
        });

        config.AddCommand<LoginCommand>("login").WithDescription("Login with Google.");
        config.AddCommand<LogoutCommand>("logout").WithDescription("End current session.");
        config.ValidateExamples();
    }
}
