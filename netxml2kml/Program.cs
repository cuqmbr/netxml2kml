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
            description: "Path to the file to be converted.");

        var outputOption = new Option<FileInfo?>(
            aliases: new[] {"-o", "--output"},
            description: "The name of the file to be created.");

        var rootCommand = new RootCommand("netxml2kml – .netxml to .kml converter.");
        rootCommand.AddOption(inputOption);
        rootCommand.AddOption(outputOption);

        rootCommand.SetHandler(CliOptionsHandlers.UniversalHandler,
            inputOption, outputOption);

        return await rootCommand.InvokeAsync(args);
    }
}