using System;
using System.Text;
using BikeshareClient;
using BikeshareClient.Models;
using System.Linq;
using System.Collections.Generic;

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
        public string Handle(string input) 
        {
            if(string.IsNullOrEmpty(input))
            {
                return "Requires BikeShare station name";
            }
            var stationId = GetStationId(input, bikeshareClient.GetStationsAsync().Result);
            var stationStatus = GetStationStatus(stationId);

            return $"Bikes available: {stationStatus.BikesAvailable}, Locks available: {stationStatus.DocksAvailable}";
        }

        private string GetStationId(string stationName, IEnumerable<Station> stations)
        {
            return stations.SingleOrDefault(s => s.Name.ToLower().Equals(stationName.ToLower().Trim())).Id; 
        }

        private StationStatus GetStationStatus(string stationId)
        {
            var stations = bikeshareClient.GetStationsStatusAsync().Result;
            return stations.FirstOrDefault(s => s.Id.Equals(stationId));
        }
    }
}
