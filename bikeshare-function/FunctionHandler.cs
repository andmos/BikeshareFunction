using System;
using System.Text;
using BikeshareClient;
using BikeshareClient.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Function
{
    public class FunctionHandler
    {
        private IBikeshareClient bikeshareClient;
        public FunctionHandler()
        {
            var gbfsAddress = System.Environment.GetEnvironmentVariable("GBFSAddress");
            bikeshareClient = new Client(gbfsAddress);
        }
        public async Task<string> Handle(string input) 
        {
            if(string.IsNullOrEmpty(input))
            {
                return "Requires BikeShare station name";
            }
            var stationId =  GetStationId(input, await bikeshareClient.GetStationsAsync());
            var stationStatus = await GetStationStatus(stationId);
            dynamic returnableStationStatus = new JObject();
            returnableStationStatus.BikesAvailable = stationStatus.BikesAvailable;
            returnableStationStatus.LocksAvailable = stationStatus.DocksAvailable;

            return JsonConvert.SerializeObject(returnableStationStatus);
        }

        private string GetStationId(string stationName, IEnumerable<Station> stations)
        {
            return stations.SingleOrDefault(s => s.Name.ToLower().Equals(stationName.ToLower().Trim())).Id; 
        }

        private async Task<StationStatus> GetStationStatus(string stationId)
        {
            var stations = await bikeshareClient.GetStationsStatusAsync();
            return stations.FirstOrDefault(s => s.Id.Equals(stationId));
        }
    }
}
