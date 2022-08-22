using Microsoft.EntityFrameworkCore;
using netxml2kml.Methods;
using netxml2kml.Models;

namespace netxml2kml.Data;

public sealed class DatabaseContext : DbContext
{
    public DbSet<WirelessNetwork> WirelessNetworks { get; set; } = null!;
    public DbSet<WirelessClient> WirelessClients { get; set; } = null!;
    public DbSet<WirelessConnection> WirelessConnections { get; set; } = null!;

    private string DbPath { get; }

    public DatabaseContext(string dbName = "netxml2kml.sqlite3.db")
    {
        if (!Directory.Exists(RuntimeStorage.AppFolder))
        {
            Directory.CreateDirectory(RuntimeStorage.AppFolder);
        }
        
        DbPath = Path.Join(RuntimeStorage.AppFolder, dbName);
        Database.EnsureCreated();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WirelessConnection>().HasKey(wc =>
            new {wc.WirelessNetworkBssid, wc.WirelessClientMac});
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options) 
        => options.UseSqlite($"Data Source={DbPath}");
}