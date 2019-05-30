using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Slackbot;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Function
{
    public class FunctionHandler
    {
        private Bot _bikeshareBot;
        private const string BikeUsername = "bikesharebot";
        private const string BikeShareFunctionEndpoint = "function/bikeshare-function";
        private HttpClient _bikeshareFunctionHttpClient;
        
        public FunctionHandler()
        {
            var openFaasGateway = System.Environment.GetEnvironmentVariable("gateway_hostname");
            
            if(string.IsNullOrWhiteSpace(openFaasGateway))
            {
                throw new ArgumentNullException("gateway_hostname env variable must be set to call bikeshare-function");
            }

            _bikeshareFunctionHttpClient = new HttpClient 
            {
                BaseAddress = new Uri(openFaasGateway)
            };

            var botToken = System.Environment.GetEnvironmentVariable("bikeBotToken");
            if(string.IsNullOrWhiteSpace(botToken))
            {
                throw new ArgumentNullException("bikeBotToken env variable must be set" );
            }
            _bikeshareBot = new Bot(botToken, BikeUsername);
            
        }

        public async Task<string> Handle(string input)
        {
            await SetupStationListener();

            return await Task.FromResult("Bot initializing");      
        }

        private async Task SetupStationListener()
        {
            _bikeshareBot.OnMessage += async (sender, message) =>
            {
                if (message.MentionedUsers.Any(user => user == BikeUsername))
                {
                    var stationStatusResponse = await _bikeshareFunctionHttpClient.PostAsync(BikeShareFunctionEndpoint, new StringContent(ParseTextFromChannel(message.Text)));
                    var stationStatus = await stationStatusResponse.Content.ReadAsStringAsync();
                    _bikeshareBot.SendMessage(message.Channel, $"{FormatBikeStationStatus(stationStatus)}");
                    
                }
            };
        }
        
        private string ParseTextFromChannel(string text)
        {
            var words = text.Split(' ');
            if(words.Count() > 0)
            {
                return words.Last();
            }
            return text; 
        }

        private string FormatBikeStationStatus(string rawJson)
        {
            var jObject = JObject.Parse(rawJson);
            var bikeToken = jObject.GetValue("BikesAvailable");
            var lockToken = jObject.GetValue("LocksAvailable");

            return $"🚲: {bikeToken.ToString()} 🔓: {lockToken.ToString()}";
        }

    }
}
