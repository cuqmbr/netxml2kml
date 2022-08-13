using Microsoft.EntityFrameworkCore;
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
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Path.Join(Environment.GetFolderPath(folder), "netxml2kml");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        DbPath = Path.Join(path, dbName);
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