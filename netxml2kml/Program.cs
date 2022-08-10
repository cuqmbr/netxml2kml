using System.CommandLine;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using netxml2kml.Data;
using netxml2kml.Models;

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

        rootCommand.SetHandler((inputFile, outputFile) =>
            {
                if (!IsValidArguments(inputFile, ref outputFile,
                        out string validationError))
                {
                    Console.WriteLine(validationError);
                    return;
                }
                
                var serializer = new XmlSerializer(typeof(detectionrun));
                var detectionRun = (detectionrun?) serializer.Deserialize(XmlReader.Create(inputFile.OpenRead(), new XmlReaderSettings {DtdProcessing = DtdProcessing.Parse}));

                var streamWriter = new StreamWriter(outputFile.OpenWrite());
                streamWriter.Write("test");
                streamWriter.Close();

                bool IsValidArguments(FileInfo? inFile, ref FileInfo? outFile,
                    out string validationErr)
                {
                    if (inFile == null)
                    {
                        validationErr = "You must specify an input file.";
                        return false;
                    }

                    if (!inFile.Exists)
                    {
                        validationErr = "Input file doesn't exist.";
                        return false;
                    }
                    
                    // If output file is not specified – set name to the
                    // name of an input file with .kml extension
                    if (outFile == null)
                    {
                        outFile = new FileInfo(Path.Join(inFile.DirectoryName,
                            $"{inFile.Name.Substring(0, inFile.Name.IndexOf(".", StringComparison.Ordinal))}.kml"));
                    }
                    
                    if (!Directory.Exists(outFile.DirectoryName))
                    {
                        validationErr = "Output directory doesn't exist.";
                        return false;
                    }

                    // If output file with the same name already exists –
                    // prompt user to change a name of the file
                    while (outFile.Exists)
                    {
                        Console.Write("Output file is already exists. Do you want to overwrite it? [y/N] ");
                        var opt = Console.ReadLine();

                        if (String.IsNullOrEmpty(opt) || opt.ToLower() == "no" ||
                            opt.ToLower() == "n")
                        {
                            Console.Write("Enter a new name: ");
                            var name = Console.ReadLine();
                            outFile = new FileInfo(
                                Path.Join(outFile.DirectoryName, name));
                            continue;
                        }
                        
                        if (opt.ToLower() == "yes" ||
                            opt.ToLower() == "y")
                        {
                            break;
                        }
                    }
                    
                    validationErr = string.Empty;
                    return true;
                }
            },
            inputOption, outputOption);

        return await rootCommand.InvokeAsync(args);
    }
}