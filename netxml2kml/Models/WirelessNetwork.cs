namespace netxml2kml.Models;

public class WirelessNetwork
{
    public int Id { get; set; }
    
    public string Ssid { get; set; } = null!;
    public bool IsCloaked { get; set; }
    public string Encryption { get; set; } = null!;
    
    public string Bssid { get; set; } = null!;

    public string Manufacturer { get; set; } = null!;
    
    public int Channel { get; set; }
    public int FrequencyMhz { get; set; }
    
    public int MaxSignalDbm { get; set; }
    
    public double MaxLatitude { get; set; }
    public double MaxLongitude { get; set;  }
    public double MaxAltitude { get; set; }
    
    public DateTime FirstSeen { get; set; }
    public DateTime LastUpdate { get; set; }
    
    public int TotalPackets { get; set; }
    
    public WirelessClient WirelessClient { get; set; } = null!;
}