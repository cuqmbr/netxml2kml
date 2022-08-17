using System.CommandLine;
using System.Xml.Linq;
using netxml2kml.Methods;

namespace netxml2kml;

class Program
{
    static int Main(string[] args)
    {
        /*-------------------------Input Option----------------------------*/
        
        var inputOption = new Option<FileInfo?>(
            aliases: new[] {"-i", "--input"},
            description: "Path to the file to be converted.")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        inputOption.AddValidator(result =>
        {
            var inputFile = result.GetValueForOption(inputOption);

            if (inputFile == null)
            {
                result.ErrorMessage = "Argument for input option is not specified.";
                return;
            }

            if (!inputFile.Exists)
            {
                result.ErrorMessage = "Input file doesn't exist.";
                return;
            }
        });

        /*-------------------------Output Option---------------------------*/
        
        var outputOption = new Option<FileInfo?>(
            aliases: new[] {"-o", "--output"},
            description: "The name of the file to be created.",
            parseArgument: result =>
            {
                // If output file with the same name already exists –
                // prompt user to change a name of the file
                var outputFile = new FileInfo(result.Tokens.Single().Value);

                while (outputFile.Exists)
                {
                    Console.Write(
                        "Output file already exists. Do you want to overwrite it? [y/N] ");
                    var opt = Console.ReadLine();

                    if (String.IsNullOrEmpty(opt) || opt.ToLower() == "no" ||
                        opt.ToLower() == "n")
                    {
                        Console.Write("Enter a <new_name>[.kml]: ");
                        var name = Console.ReadLine();

                        if (String.IsNullOrEmpty(name))
                        {
                            continue;
                        }

                        outputFile =
                            new FileInfo(Path.Join(outputFile.DirectoryName,
                                name));
                        continue;
                    }

                    if (opt.ToLower() == "yes" ||
                        opt.ToLower() == "y")
                    {
                        break;
                    }
                }

                return outputFile;
            })
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        outputOption.AddValidator(result =>
        {
            var outputFile = result.GetValueForOption(outputOption);

            if (outputFile == null)
            {
                result.ErrorMessage = "Argument for output option is not specified.";
                return;
            }

            if (!Directory.Exists(outputFile.DirectoryName))
            {
                result.ErrorMessage = "Output directory doesn't exist.";
                return;
            }
        });

        /*-------------------------Concat Option---------------------------*/
        
        var concatOption = new Option<IEnumerable<FileInfo>?>(
            aliases: new[] {"-c", "--concat"},
            description: "Concatenate multiple kml files. ")
        {
            Arity = ArgumentArity.OneOrMore,
            AllowMultipleArgumentsPerToken = true
        };
        
        concatOption.AddValidator(result =>
        {
            var inputFiles = result.GetValueForOption(concatOption);

            if (inputFiles == null)
            {
                result.ErrorMessage = "Argument for concat option is not specified.";
                return;
            }

            var fileInfos = inputFiles as FileInfo[] ?? inputFiles.ToArray();

            foreach (var inputFile in fileInfos)
            {
                if (!inputFile.Exists)
                {
                    result.ErrorMessage = $"File {inputFile.FullName} doesen't exist.";
                    return;
                }
            }

            // Validate that inputted files have the same format
            if (fileInfos.Any(fi => XDocument.Load(fi.FullName).Root?.Name != "kml"))
            {
                result.ErrorMessage = "Some files passed to concat option have invalid content.";
                return;
            }
        });

        /*------------------------Database Option--------------------------*/
        
        var databaseOption = new Option<bool>(
            aliases: new[] {"-d", "--use-database"},
            description:
            "Use database (save/retrieve data from database).")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        /*------------------------Query Option-----------------------------*/
        
        var queryOption = new Option<string?>(
            aliases: new[] {"-q", "--query"},
            description: "Filter input/output using sql query.")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        /*----------------------Root Command Setup-------------------------*/

        var rootCommand =
            new RootCommand("netxml2kml – .netxml to .kml converter.");
        rootCommand.AddOption(inputOption);
        rootCommand.AddOption(outputOption);
        rootCommand.AddOption(databaseOption);
        rootCommand.AddOption(queryOption);
        rootCommand.AddOption(concatOption);

        /*----------------------Handlers Setup-----------------------------*/
        
        rootCommand.SetHandler(CliOptionsHandlers.UniversalHandler,
            inputOption, outputOption, databaseOption, queryOption,
            concatOption);
        
        /*----------------------------------------------------------------*/

        return rootCommand.Invoke(args);
    }
}