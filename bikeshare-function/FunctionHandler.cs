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
        private IBikeshareClient _bikeshareClient;
        public FunctionHandler()
        {
            var gbfsAddress = System.Environment.GetEnvironmentVariable("GBFSAddress");
            _bikeshareClient = new Client(gbfsAddress);
        }
        public async Task<string> Handle(string input) 
        {
            if(string.IsNullOrEmpty(input))
            {
                return "Requires BikeShare station name";
            }
            
            var stationStatus = await GetAllBikeStationStatus();
            if(input.ToLower().Equals("all") || input.ToLower().Equals("list") || input.ToLower().Equals("stations"))
            {
                return JsonConvert.SerializeObject(stationStatus);
            }
            stationStatus = stationStatus.Where(station => station.Name.ToLower().Equals(input.ToLower()));
            return JsonConvert.SerializeObject(stationStatus);
        }

        private async Task<IEnumerable<BikeshareFunctionStationStatus>> GetAllBikeStationStatus()
        {
            var bikeFunctionStationStatus = new List<BikeshareFunctionStationStatus>();
            var stations = await _bikeshareClient.GetStationsAsync();
            var stationStatus =  await _bikeshareClient.GetStationsStatusAsync();
            
            foreach(var station in stations)
            {
                var singleStationStatus = GetStationStatus(GetStationId(station.Name, stations), stationStatus);
                {
                    bikeFunctionStationStatus.Add(new BikeshareFunctionStationStatus 
                    {
                        Name = station.Name,
                        BikesAvailable = singleStationStatus.BikesAvailable,
                        LocksAvailable = singleStationStatus.DocksAvailable
                        
                    });
                }
            }
            return bikeFunctionStationStatus;
        }
        private string GetStationId(string stationName, IEnumerable<Station> stations)
        {
            return stations.SingleOrDefault(s => s.Name.ToLower().Equals(stationName.ToLower().Trim())).Id; 
        }
        private StationStatus GetStationStatus(string stationId, IEnumerable<StationStatus> stationStatus)
        {
            return stationStatus.FirstOrDefault(s => s.Id.Equals(stationId));
        }

        private class BikeshareFunctionStationStatus
        {
            public string Name {get; set;}
            public int BikesAvailable {get; set;}
            public int LocksAvailable {get; set;}
        }

    }
}
