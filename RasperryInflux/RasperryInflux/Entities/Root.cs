namespace RasperryInflux.Entities
{
    public class Root
    {
            public int battery { get; set; }
            public bool battery_low { get; set; }
            public bool contact { get; set; }
            public int linkquality { get; set; }
            public bool tamper { get; set; }
            public int voltage { get; set; }
    }
}
