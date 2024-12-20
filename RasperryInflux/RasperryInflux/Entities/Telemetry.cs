namespace RasperryInflux.Entities
{
    public class Telemetry
    {
        public int battery { get; set; }
        public double humidity { get; set; }
        public int linkquality { get; set; }
        public double temperature { get; set; }
        public int voltage { get; set; }
        public DateTime time { get; set; }
    }
}
