using System;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace DataMailbox2Azure
{
    public sealed class Configuration
    {
        private static readonly Lazy<Configuration> _instance = new Lazy<Configuration>(() => new Configuration());

        public static Configuration Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public string Talk2MUsername { get; private set; }
        public string Talk2MPassword { get; private set; }
        public string Talk2MAccount { get; private set; }
        public string Talk2MDeveloperId { get; private set; }
        public string IoTHubKeyName { get; private set; }
        public string IoTHubKeyValue { get; private set; }
        public string IoTHubHostName { get; private set; }
        public string IoTHubSharedAccessSignature { get; set; }
        public int IoTHubTimeToLive {get; private set; }
        public int LastTransactionId { get; set; }
        public int IntervalInSeconds { get; private set; }

        private Configuration()
        {
            var path = Path.Combine(System.AppContext.BaseDirectory, "settings.json");
            var json = File.ReadAllText(path, Encoding.UTF8);
            
            dynamic config = JObject.Parse(json);
            
            this.Talk2MUsername = config.Talk2M.Username;
            this.Talk2MPassword = config.Talk2M.Password;
            this.Talk2MAccount = config.Talk2M.Account;
            this.Talk2MDeveloperId = config.Talk2M.DeveloperId;
            this.IoTHubKeyName = config.IoTHub.KeyName;
            this.IoTHubKeyValue = config.IoTHub.KeyValue;
            this.IoTHubHostName = config.IoTHub.HostName;
            this.IoTHubTimeToLive = config.IoTHub.TimeToLive;
            this.IntervalInSeconds = config.IntervalInSeconds ?? 5;

            if (config.LastTransactionId != null)
            {
                this.LastTransactionId = (int)config.LastTransactionId;
            }
        }
    }
}