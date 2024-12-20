using RasperryInflux.Data.InfluxDB;
using RasperryInflux.Entities;

namespace RasperryInflux.Service
{
    public class DataService(IInfluxRepository _repository) : IDataService
    {
        public async Task<List<Telemetry>> GetDataService()
        {
            return await _repository.QuereDbAsync("All");
        }
    }
}
