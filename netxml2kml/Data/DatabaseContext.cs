using Microsoft.EntityFrameworkCore;
using netxml2kml.Models;

namespace netxml2kml.Data;

public sealed class DatabaseContext : DbContext
{
    public DbSet<WirelessNetwork> WirelessNetworks { get; set; } = null!;
    public DbSet<WirelessClient> WirelessClients { get; set; } = null!;

    private string DbPath { get; }

    public DatabaseContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Path.Join(Environment.GetFolderPath(folder), "netxml2kml");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        DbPath = Path.Join(path, "netxml2kml.sqlite3.db");
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options) 
        => options.UseSqlite($"Data Source={DbPath}");
}