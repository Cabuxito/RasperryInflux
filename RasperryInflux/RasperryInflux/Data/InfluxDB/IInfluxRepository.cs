
using RasperryInflux.Entities;

namespace RasperryInflux.Data.InfluxDB
{
    public interface IInfluxRepository
    {
        Task<List<Telemetry>> QuereDbAsync(string option);
        Task WriteTelemetry(Telemetry telemetry);
    }
}