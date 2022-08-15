using System.CommandLine;
using netxml2kml.Methods;

namespace netxml2kml;

class Program
{
    static async Task<int> Main(string[] args)
    {
        // Define CLI parser options, commands & handlers

        var inputOption = new Option<FileInfo?>(
            aliases: new[] {"-i", "--input"},
            description: "Path to the file to be converted.")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        var outputOption = new Option<FileInfo?>(
            aliases: new[] {"-o", "--output"},
            description: "The name of the file to be created.")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        var databaseOption = new Option<bool>(
            aliases: new[] {"-d", "--use-database"},
            description: "Use database. Save/retrieve wireless networks and clients to/from sqlite database.")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        var queryOption = new Option<string?>(
            aliases: new[] {"-q", "--query"},
            description: "Filter input using sql query.")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        var rootCommand = new RootCommand("netxml2kml – .netxml to .kml converter.");
        rootCommand.AddOption(inputOption);
        rootCommand.AddOption(outputOption);
        rootCommand.AddOption(databaseOption);
        rootCommand.AddOption(queryOption);

        rootCommand.SetHandler(CliOptionsHandlers.UniversalHandler,
            inputOption, outputOption, databaseOption, queryOption);

        return await rootCommand.InvokeAsync(args);
    }
}