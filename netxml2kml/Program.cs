using System.CommandLine;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using netxml2kml.Models;

namespace netxml2kml;

static class Program
{
    static async Task<int> Main(string[] args)
    {
        var inputOption = new Option<FileInfo?>(
            aliases: new[] {"-i", "--input"},
            description: "The file to be converted to .kml format.");

        var outputOption = new Option<FileInfo?>(
            aliases: new[] {"-o", "--output"},
            description: "The name of the file to be created.");

        var rootCommand =
            new RootCommand("netxml2kml – .netxml to .kml converter.");
        rootCommand.AddOption(inputOption);
        rootCommand.AddOption(outputOption);

        rootCommand.SetHandler((inputFile, outputFile) =>
            {
                if (outputFile == null)
                {
                    outputFile = new FileInfo($"{inputFile!.Name.Substring(0, inputFile!.Name.Length - inputFile.Extension.Length)}.kml");
                }

                var serializer = new XmlSerializer(typeof(detectionrun));
                var detectionRun = (detectionrun?) serializer.Deserialize(XmlReader.Create(inputFile!.OpenRead(), new XmlReaderSettings {DtdProcessing = DtdProcessing.Parse}));

                if (detectionRun == null)
                {
                    throw new ArgumentNullException();
                }
                
                var kmlString = GenerateKml(detectionRun, inputFile.Name);
                var streamWriter = new StreamWriter(outputFile.OpenWrite());
                streamWriter.Write(kmlString);
                streamWriter.Close();
            },
            inputOption, outputOption);

        return await rootCommand.InvokeAsync(args);
        
        string GenerateKml(detectionrun detectionRun, string name)
        {
            var sb = new StringBuilder();

            sb.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sb.Append("\n<kml xmlns=\"http://www.opengis.net/kml/2.2\">");
            sb.Append("\n  <Document>");
            sb.Append($"\n    <name>Capture file: {name}</name>" +
                      $"\n    <description>Started on {detectionRun.starttime}." +
                      $"\nCaptured {detectionRun.wirelessnetwork.Count(wn => wn.type != "probe")} AP(s) and {detectionRun.wirelessnetwork.Count(wn => wn.type == "probe")} client(s)</description>");

            foreach (var wn in detectionRun.wirelessnetwork)
            {
                // If wireless network is a client – skip
                if (wn.type == "probe")
                {
                    continue;
                }
                
                sb.Append("\n    <Placemark>");

                sb.Append($"\n      <name>{wn.SSID.First().essid.First().Value}</name>.");
                sb.Append($"\n      <description>Manufacturer: {wn.manuf}." +
                          $"\n        Last update: {wn.lasttime}." +
                          $"\n        Channel: {wn.channel}." +
                          $"\n        BSSID: {wn.BSSID}." +
                          $"\n        Frequency {wn.freqmhz}." +
                          $"\n      </description>");
                sb.Append($"\n      <Point>" +
                          $"\n        <coordinates>{wn.gpsinfo.First().peaklon},{wn.gpsinfo.First().peaklat},{wn.gpsinfo.First().peakalt}</coordinates>" +
                          $"\n      </Point>");
            
                sb.Append("\n    </Placemark>");
            }

            sb.Append("\n  </Document>");
            sb.Append("\n</kml>");
        
            return sb.ToString();
        }
    }
}