using Microsoft.EntityFrameworkCore;
using netxml2kml.Data;
using netxml2kml.Models;

namespace netxml2kml.Methods;

public static class CliOptionsHandlers
{
    public static void UniversalHandler(FileInfo? inputFile,
        FileInfo? outputFile, bool useDatabase, string? sqlQuery)
    {
        // Validate input file
        if (inputFile != null)
        {
            if (!inputFile.Exists)
            {
                Console.WriteLine("Input file doesn't exist.");
                return;
            }
        }

        // Validate output file
        if (outputFile != null)
        {
            if (!Directory.Exists(outputFile.DirectoryName))
            {
                Console.WriteLine("Output directory doesn't exist.");
                return;
            }

            // If output file with the same name already exists –
            // prompt user to change a name of the file
            while (outputFile.Exists)
            {
                Console.Write("Output file already exists. Do you want to overwrite it? [y/N] ");
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
                            
                    outputFile = new FileInfo(
                        Path.Join(outputFile.DirectoryName, name));
                    continue;
                }
                        
                if (opt.ToLower() == "yes" ||
                    opt.ToLower() == "y")
                {
                    break;
                }
            }
        }

        // Run some logic based on options combination
        if (inputFile != null && outputFile != null)
        {
            var wirelessNetworks = Helpers.DeserializeXml(inputFile);
            
            if (useDatabase)
            {
                AddWirelessNetworksToDatabase(wirelessNetworks);
            }
            
            if (sqlQuery != null)
            {
                FilterWirelessNetworksInMemory(ref wirelessNetworks, sqlQuery);
            }

            var kmlString = GetKmlString(wirelessNetworks);
            Helpers.WriteStringToFile(kmlString, outputFile);
        }
        else if (inputFile != null && outputFile == null && useDatabase)
        {
            var wirelessNetworks = Helpers.DeserializeXml(inputFile);

            if (sqlQuery != null)
            {
                FilterWirelessNetworksInMemory(ref wirelessNetworks, sqlQuery);
            }
            
            AddWirelessNetworksToDatabase(wirelessNetworks);
        }
        else if (inputFile == null && outputFile != null && useDatabase)
        {
            string kmlString;
            
            if (sqlQuery != null)
            {
                kmlString = GetKmlString(FilterWirelessNetworksFromDatabase(sqlQuery));
            }
            else
            {
                kmlString = GetKmlString(FilterWirelessNetworksFromDatabase("SELECT * FROM WirelessNetworks"));
            }
            
            Helpers.WriteStringToFile(kmlString, outputFile);
        }
        else if (inputFile == null && outputFile == null && useDatabase &&
                 sqlQuery != null)
        {
            using var dbContext = new DatabaseContext();
            Console.WriteLine(dbContext.Database.ExecuteSqlRaw(sqlQuery));
        }
        else
        {
            Console.WriteLine("Options combination is unsupported or some option lacks an argument." +
                              "\nUse --help to see use case examples.");
        }
    }
    
    private static string GetKmlString(IEnumerable<WirelessNetwork> wirelessNetworks)
    {
        return "test";
    }
    
    private static void AddWirelessNetworksToDatabase(IEnumerable<WirelessNetwork> wirelessNetworks)
    {
        using var dbContext = new DatabaseContext();

        foreach (var wirelessNetwork in wirelessNetworks)
        {
            // If wireless network has wireless clients – add all clients to
            // the database or update their LastUpdateDate
            if (wirelessNetwork.WirelessConnections != null)
            {
                foreach (var wirelessConnection in wirelessNetwork.WirelessConnections)
                {
                    var client = wirelessConnection.WirelessClient;

                    // Add new client to the DB if it is not present
                    if (!dbContext.WirelessClients.Any(wc =>
                            wc.Mac == client.Mac))
                    {
                        dbContext.WirelessClients.Add(client);
                    }
                    // Update LastUpdateDate if the client is present in the DB
                    else if (dbContext.WirelessClients.Any(wc =>
                                 wc.Mac == client.Mac))
                    {
                        var dbClient = dbContext.WirelessClients
                            .First(wc => wc.Mac == client.Mac);

                        if (dbClient.LastUpdateDate < client.FirstSeenDate)
                        {
                            dbClient.LastUpdateDate = client.FirstSeenDate;
                        }
                    }
                }
            }

            // If wireless network has wireless clients – add a wireless
            // connections to the database or update their LastUpdateDate 
            if (wirelessNetwork.WirelessConnections != null)
            {
                foreach (var wirelessConnection in wirelessNetwork.WirelessConnections)
                {
                    var client = wirelessConnection.WirelessClient;

                    // Add new connection to the DB if it is not present
                    if (!dbContext.WirelessConnections.Any(wc =>
                            wc.WirelessClientMac == client.Mac &&
                            wc.WirelessNetworkBssid == wirelessNetwork.Bssid))
                    {
                        dbContext.WirelessConnections.Add(new WirelessConnection
                        {
                            WirelessClientMac = client.Mac,
                            WirelessNetworkBssid = wirelessNetwork.Bssid,
                            FirstSeenDate = client.FirstSeenDate,
                            LastUpdateDate = client.LastUpdateDate
                        });
                    }
                    // Update LastUpdateDate if the connection is present in the DB
                    else if (dbContext.WirelessConnections.Any(wc =>
                                 wc.WirelessClientMac == client.Mac &&
                                 wc.WirelessNetworkBssid == wirelessNetwork.Bssid))
                    {
                        var dbConnection = dbContext.WirelessConnections.First(
                            wc => wc.WirelessClientMac == client.Mac &&
                                  wc.WirelessNetworkBssid == wirelessNetwork.Bssid);
                        var connectedClient = wirelessNetwork
                            .WirelessConnections
                            .First(wc => wc.WirelessClient.Mac == client.Mac)
                            .WirelessClient;

                        if (dbConnection.LastUpdateDate < connectedClient.FirstSeenDate)
                        {
                            dbConnection.LastUpdateDate =
                                connectedClient.FirstSeenDate;
                        }
                    }
                }
            }
            
            // Set wireless connections to null since we manually added
            // connections to the database context
            wirelessNetwork.WirelessConnections = null;
            
            // Add new network to the DB if it is not present
            if (!dbContext.WirelessNetworks.Any(wn =>
                    wn.Bssid == wirelessNetwork.Bssid))
            {
                dbContext.Add(wirelessNetwork);
            }
            // Update LastUpdateDate & MaxSignalDmb if
            // the network is present in the DB
            else if (dbContext.WirelessNetworks.Any(wn =>
                         wn.Bssid == wirelessNetwork.Bssid))
            {
                var dbNetwork = dbContext.WirelessNetworks.First(wn =>
                        wn.Bssid == wirelessNetwork.Bssid);

                if (dbNetwork.MaxSignalDbm < wirelessNetwork.MaxSignalDbm)
                {
                    dbNetwork.MaxLatitude = wirelessNetwork.MaxLatitude;
                    dbNetwork.MaxLongitude = wirelessNetwork.MaxLongitude;
                    dbNetwork.MaxAltitude = wirelessNetwork.MaxAltitude;
                    dbNetwork.MaxSignalDbm = wirelessNetwork.MaxSignalDbm;
                }

                if (dbNetwork.LastUpdateDate < wirelessNetwork.FirstSeenDate)
                {
                    dbNetwork.LastUpdateDate = wirelessNetwork.FirstSeenDate;
                }
            }
        }
        
        dbContext.SaveChanges();
    }
    
    private static void FilterWirelessNetworksInMemory(
        ref WirelessNetwork[] wirelessNetworks, string sqlQuery)
    {
        using var dbContext = new DatabaseContext("InMemoryFiltering.sqlite3.db");
        
        dbContext.WirelessNetworks.AddRange(wirelessNetworks);
        dbContext.SaveChanges();
        wirelessNetworks = dbContext.WirelessNetworks.FromSqlRaw(sqlQuery).ToArray();
        
        dbContext.Database.EnsureDeleted();
    }
    
    private static WirelessNetwork[] FilterWirelessNetworksFromDatabase(string sqlQuery)
    {
        using var dbContext = new DatabaseContext();
        return dbContext.WirelessNetworks.FromSqlRaw(sqlQuery).ToArray();
    }
}