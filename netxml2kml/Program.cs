using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using System.Xml.Linq;
using netxml2kml.Methods;

namespace netxml2kml;

class Program
{
    static async Task<int> Main(string[] args)
    {
        /*-------------------------Input Option----------------------------*/
        
        var inputOption = new Option<FileInfo?>(
            aliases: new[] {"-i", "--input"},
            description: "Path to the file to be converted")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        inputOption.ArgumentHelpName = "file_path[.netxml]";
        
        inputOption.AddValidator(result =>
        {
            var inputFile = result.GetValueForOption(inputOption);

            if (inputFile == null)
            {
                result.ErrorMessage =
                    "Argument for input option is not specified";
                return;
            }

            if (!inputFile.Exists)
            {
                result.ErrorMessage = "Input file doesn't exist";
                return;
            }
        });

        /*-------------------------Output Option---------------------------*/
        
        var outputOption = new Option<FileInfo?>(
            aliases: new[] {"-o", "--output"},
            description: "Path to the file to be created",
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

        outputOption.ArgumentHelpName = "file_path[.kml]";
        
        outputOption.AddValidator(result =>
        {
            var outputFile = result.GetValueForOption(outputOption);

            if (outputFile == null)
            {
                result.ErrorMessage =
                    "Argument for output option is not specified";
                return;
            }

            if (!Directory.Exists(outputFile.DirectoryName))
            {
                result.ErrorMessage = "Output directory doesn't exist";
                return;
            }
        });

        /*-------------------------Concat Option---------------------------*/
        
        var concatOption = new Option<IEnumerable<FileInfo>?>(
            aliases: new[] {"-c", "--concat"},
            description: "Concatenate multiple kml files")
        {
            Arity = ArgumentArity.OneOrMore,
            AllowMultipleArgumentsPerToken = true
        };

        concatOption.ArgumentHelpName = "file_path[.kml] file_path[.kml] ...";
        
        concatOption.AddValidator(result =>
        {
            var inputFiles = result.GetValueForOption(concatOption);

            if (inputFiles == null)
            {
                result.ErrorMessage =
                    "Argument for concat option is not specified";
                return;
            }

            var fileInfos = inputFiles as FileInfo[] ?? inputFiles.ToArray();

            foreach (var inputFile in fileInfos)
            {
                if (!inputFile.Exists)
                {
                    result.ErrorMessage =
                        $"File {inputFile.FullName} doesen't exist";
                    return;
                }
            }

            // Validate that inputted files have the same format
            if (fileInfos.Any(fi =>
                    XDocument.Load(fi.FullName).Root?.Name != "kml"))
            {
                result.ErrorMessage =
                    "Some files passed to concat option have invalid content";
                return;
            }
        });

        /*------------------------Database Option--------------------------*/
        
        var databaseOption = new Option<bool>(
            aliases: new[] {"-d", "--use-database"},
            description:
            "Use database (save/retrieve data)")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        /*------------------------Query Option-----------------------------*/
        
        var queryOption = new Option<string?>(
            aliases: new[] {"-q", "--query"},
            description: "Filter input/output using sql query")
        {
            Arity = ArgumentArity.ZeroOrOne
        };

        queryOption.ArgumentHelpName = "\"sql_query\"";
        
        /*----------------------Verbosity Option---------------------------*/
        
        var verboseOption = new Option<bool>(
            aliases: new[] {"-v", "--verbose"},
            description: "Show verbose output")
        {
            Arity = ArgumentArity.ZeroOrOne
        };
        
        verboseOption.AddValidator(result =>
        {
            RuntimeStorage.IsVerbose = result.GetValueForOption(verboseOption);
        });

        RuntimeStorage.ConfigureLogger();
        
        /*----------------------Root Command Setup-------------------------*/

        var rootCommand =
            new RootCommand("netxml2kml – .netxml to .kml CLI converter & tools");
        rootCommand.AddOption(inputOption);
        rootCommand.AddOption(outputOption);
        rootCommand.AddOption(databaseOption);
        rootCommand.AddOption(queryOption);
        rootCommand.AddOption(concatOption);
        rootCommand.AddOption(verboseOption);

        /*----------------------Handlers Setup-----------------------------*/
        
        rootCommand.SetHandler(CliOptionsHandlers.UniversalHandler,
            inputOption, outputOption, databaseOption, queryOption,
            concatOption, verboseOption);
        
        /*----------------------------------------------------------------*/

        var rootCommandParser = new CommandLineBuilder(rootCommand)
            .UseDefaults().UseHelp(ctx =>
            {
                ctx.HelpBuilder.CustomizeLayout(_ =>
                    HelpBuilder.Default
                        .GetLayout()
                        .Skip(HelpBuilder.Default.GetLayout().Count())
                        .Append(_ => Console.WriteLine(
                            "netxml2kml – .netxml to .kml CLI converter & tools" +
                          "\n" +
                          "\nUsage:" +
                          "\n  netxml2kml [options]" +
                          "\n" +
                          "\nOptions:" +
                          "\n  -i, --input <file_path[.netxml]>                    Path to the file to be converted" +
                          "\n  -o, --output <file_path[.kml]>                      Path to the file to be created" +
                          "\n  -c, --concat <file_path[.kml] file_path[.kml] ...>  Concatenate multiple kml files" +
                          "\n  -d, --use-database                                  Use database (save/retrieve data)" +
                          "\n  -q, --query <\"sql_query\">                           Filter input/output using sql query" +
                          "\n  -v, --verbose                                       Show verbose output" +
                          "\n  -h, --help                                          Show help and usage information" +
                          "\n  --version                                           Show version information")));
            }).Build();
         
        try
        {
            return await rootCommandParser.InvokeAsync(args);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return 0;
    }
}