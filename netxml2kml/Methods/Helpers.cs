using System.Xml.Linq;
using netxml2kml.Models;

namespace netxml2kml.Methods;

public static class Helpers
{
    public static WirelessNetwork[] DeserializeXml(FileInfo inputFile)
    {
        var srcTree = XDocument.Load(inputFile.OpenRead());
        var srcNets = srcTree.Root!.Elements("wireless-network")
            .Where(wn => wn.Attribute("type")!.Value != "probe").ToList();

        var wirelessNetworks = new WirelessNetwork[srcNets.Count];

        for (int i = 0; i < srcNets.Count; i++)
        {
            var srcNet = srcNets[i];

            var essid = srcNet.Element("SSID")!.Element("essid")!.Value;
            
            wirelessNetworks[i] = new()
            {
                Essid = essid != "" ? essid : null,
                Encryption = srcNet.Element("SSID")!.Elements("encryption").LastOrDefault()?.Value,
                Bssid = srcNet.Element("BSSID")!.Value,
                Manufacturer = srcNet.Element("manuf")!.Value,
                FrequencyMhz = Math.Round(Double.Parse(srcNet.Element("freqmhz")!.Value.Substring(0, 2)) / 10, 1, MidpointRounding.ToPositiveInfinity),
                MaxSignalDbm = Int32.Parse(srcNet.Element("snr-info")!.Element("max_signal_dbm")!.Value),
                MaxLatitude = Double.Parse(srcNet.Element("gps-info")!.Element("max-lat")!.Value),
                MaxLongitude = Double.Parse(srcNet.Element("gps-info")!.Element("max-lon")!.Value),
                MaxAltitude = Double.Parse(srcNet.Element("gps-info")!.Element("max-alt")!.Value),
                FirstSeenDate = StringToDate(srcNet.Attribute("first-time")!.Value),
                LastUpdateDate = StringToDate(srcNet.Attribute("last-time")!.Value)
            };

            // If wireless network has no clients – continue
            if (!srcNet.Elements("wireless-client").Any())
            {
                continue;
            }

            var wirelessConnections = new List<WirelessConnection>(srcNet.Elements("wireless-client").Count());

            foreach (var wc in srcNet.Elements("wireless-client").ToArray())
            {
                wirelessConnections.Add(new WirelessConnection
                {
                    WirelessClient = new WirelessClient
                    {
                        Mac = wc.Element("client-mac")!.Value,
                        Manufacturer = wc.Element("client-manuf")!.Value,
                        FirstSeenDate = StringToDate(wc.Attribute("first-time")!.Value),
                        LastUpdateDate = StringToDate(wc.Attribute("last-time")!.Value)
                    },
                    WirelessNetwork = wirelessNetworks[i]
                });
            }

            wirelessNetworks[i].WirelessConnections = wirelessConnections;
        }

        return wirelessNetworks.ToArray();
    }

    public static DateTime StringToDate(string dateString)
    {
        var monthNameNumber = new Dictionary<string, int>
        {
            {"Jan", 1},
            {"Feb", 2},
            {"Mar", 3},
            {"Apr", 4},
            {"May", 5},
            {"Jun", 6},
            {"Jul", 7},
            {"Aug", 8},
            {"Sep", 9},
            {"Oct", 10},
            {"Nov", 11},
            {"Dec", 12},
        };

        var year = Int32.Parse(dateString.Split(" ")[4]);
        var month = monthNameNumber[dateString.Split(" ")[1]];
        var day = Int32.Parse(dateString.Split(" ")[2]);
        var hour = Int32.Parse(dateString.Split(" ")[3].Split(":")[0]);
        var minute = Int32.Parse(dateString.Split(" ")[3].Split(":")[1]);
        var second = Int32.Parse(dateString.Split(" ")[3].Split(":")[2]);

        return new DateTime(year, month, day, hour, minute, second);
    }

    public static void WriteStringToFile(string str, FileInfo file)
    {
        var writer = new StreamWriter(file.Open(FileMode.Create));
        writer.Write(str);
        writer.Close();
    }
}