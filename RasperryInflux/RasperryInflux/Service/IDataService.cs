using RasperryInflux.Entities;

namespace RasperryInflux.Service
{
    public interface IDataService
    {
        Task<List<Telemetry>> GetDataService();
    }
}