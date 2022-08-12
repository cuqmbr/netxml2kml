using System.ComponentModel.DataAnnotations;

namespace netxml2kml.Models;

public class WirelessClient
{
    [Key]    
    public string Mac { get; set; } = null!;
    public string Manufacturer { get; set; } = null!;
    
    public DateTime FirstSeenDate { get; set; }
    public DateTime LastUpdateDate { get; set; }

    public List<WirelessConnection> WirelessConnections { get; set; } = null!;
}