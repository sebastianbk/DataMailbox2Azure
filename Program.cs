using System;

namespace DataMailbox2Azure
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Generate SAS token
            var builder = new SharedAccessSignatureBuilder()
            {
                KeyName = Configuration.Instance.IoTHubKeyName,
                Key = Configuration.Instance.IoTHubKeyValue,
                Target = Configuration.Instance.IoTHubHostName,
                TimeToLive = TimeSpan.FromDays(Configuration.Instance.IoTHubTimeToLive)
            };
            Configuration.Instance.IoTHubSharedAccessSignature = builder.ToSignature();

            // Create service Instance
            var talk2mService = new Talk2MService();

            // Continuously fetch new events from DataMailbox
            while (true)
            {
                talk2mService.SyncData();
                System.Threading.Thread.Sleep(Configuration.Instance.IntervalInSeconds * 1000);
            }
        }
    }
}
