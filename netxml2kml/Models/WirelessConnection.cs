using System.ComponentModel.DataAnnotations.Schema;

namespace netxml2kml.Models;

public class WirelessConnection
{
    [ForeignKey("WirelessNetwork")]
    public string WirelessNetworkBssid { get; set; } = null!;
    public WirelessNetwork WirelessNetwork { get; set; } = null!;

    [ForeignKey("WirelessClient")]
    public string WirelessClientMac { get; set; } = null!;
    public WirelessClient WirelessClient { get; set; } = null!;
    
    public DateTime FirstSeenDate { get; set; }
    public DateTime LastUpdateDate { get; set; }
}