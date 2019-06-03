using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Slackbot;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.IO;

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

            var botToken = File.ReadAllText(@"/var/openfaas/secrets/bikeBotSlackToken");
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
                Console.WriteLine($"Info: Slack message: {message.Text}");
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
                words = words.Skip(1).ToArray();
                text = string.Join(" ", words);
            }
            Console.WriteLine($"Info: Parsed text: {text}");
            return text;
        }

        private string FormatBikeStationStatus(string rawJson)
        {
            string returnText;
            try
            {
                var jObject = JObject.Parse(rawJson);
                var bikeToken = jObject.GetValue("BikesAvailable");
                var lockToken = jObject.GetValue("LocksAvailable");
                returnText = $"🚲: {bikeToken.ToString()} 🔓: {lockToken.ToString()}";
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Handled exception: {ex}");
                returnText = "Something went wrong when parsing station status, was station name correct?";
            }
            return returnText;
            
        }

    }
}
