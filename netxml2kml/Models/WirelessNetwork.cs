using System.ComponentModel.DataAnnotations;

namespace netxml2kml.Models;

public class WirelessNetwork
{
    [Key]
    public string Bssid { get; set; } = null!;
    
    public string? Essid { get; set; }
    public string Manufacturer { get; set; } = null!;
    public string? Encryption { get; set; }
    public double FrequencyMhz { get; set; }
    
    public int MaxSignalDbm { get; set; }
    
    public double MaxLatitude { get; set; }
    public double MaxLongitude { get; set;  }
    public double MaxAltitude { get; set; }
    
    public DateTime FirstSeenDate { get; set; }
    public DateTime LastUpdateDate { get; set; }

    public List<WirelessConnection>? WirelessConnections { get; set; }

    public bool IsCloaked => Essid == null;
}