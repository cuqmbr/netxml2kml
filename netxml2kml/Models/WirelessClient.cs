namespace netxml2kml.Models;

public class WirelessClient
{
    public int Id { get; set; }
    
    public string Mac { get; set; } = null!;
    public string Manufacturer { get; set; } = null!;
    public int TotalPackets { get; set; }
}