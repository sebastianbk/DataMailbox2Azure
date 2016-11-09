using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DataMailbox2Azure
{
    public class Talk2MService
    {
        private HttpClient talk2mHttpClient;
        private HttpClient iotHubHttpClient;

        public Talk2MService()
        {
            talk2mHttpClient = new HttpClient();
            talk2mHttpClient.BaseAddress = new Uri("https://data.talk2m.com/");

            iotHubHttpClient = new HttpClient();
            iotHubHttpClient.BaseAddress = new Uri(string.Format("https://{0}/", Configuration.Instance.IoTHubHostName));
            var sasTokenArray = Configuration.Instance.IoTHubSharedAccessSignature.Split(' ');
            iotHubHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    sasTokenArray[0],
                    sasTokenArray[1]
                );
        }

        public void SyncData()
        {
            var jsonBody = RetrieveSyncDataFromTalk2M();

            if ((bool)jsonBody.success)
            {
                Configuration.Instance.LastTransactionId = jsonBody.transactionId;
                
                var ewons = ((JArray)jsonBody.ewons).Children();

                foreach (var jsonEwon in ewons)
                {
                    var deviceId = jsonEwon.Value<string>("name").Replace(' ', '-');
                    CreateDeviceIfItDoesNotExist(deviceId);
                    var tags = jsonEwon.Value<JArray>("tags");

                    foreach (var tag in tags.Children())
                    {
                        var tagId = tag.Value<int>("id");
                        var tagName = tag.Value<string>("name");

                        var tokens = tag.Children();

                        foreach (var token in tokens)
                        {
                            var historyArray = token.First as JArray;

                            if (historyArray != null)
                            {
                                foreach (var historyItem in historyArray.Children())
                                {
                                    CreateMessage(deviceId, tagId, tagName, historyItem);
                                }
                            }
                        }
                    }
                }
            }
        }

        private dynamic RetrieveSyncDataFromTalk2M()
        {
            var url = string.Format("syncdata?t2maccount={0}&t2musername={1}&t2mpassword={2}&t2mdevid={3}&createTransaction",
                Configuration.Instance.Talk2MAccount,
                Configuration.Instance.Talk2MUsername,
                Configuration.Instance.Talk2MPassword,
                Configuration.Instance.Talk2MDeveloperId);

            if (Configuration.Instance.LastTransactionId > 0)
            {
                url = string.Format("{0}&lastTransactionId={1}", url, Configuration.Instance.LastTransactionId);
            }

            var body = talk2mHttpClient.GetStringAsync(url).Result;
            dynamic jsonBody = JObject.Parse(body);

            return jsonBody;
        }

        private void CreateMessage(string deviceId, int tagId, string tagName, object record)
        {
            var message = new
            {
                deviceId = deviceId,
                tagId = tagId,
                tagName = tagName,
                record = record
            };
            var jsonMessage = JsonConvert.SerializeObject(message);

            PostEventToIoTHub(deviceId, jsonMessage);

            #if DEBUG
            Console.WriteLine(jsonMessage);
            #endif
        }

        private void CreateDeviceIfItDoesNotExist(string deviceId)
        {
            var url = string.Format("devices/{0}?api-version=2016-02-03",
                deviceId);
            var getResult = iotHubHttpClient.GetAsync(url).Result;
            var deviceExists = getResult.IsSuccessStatusCode;

            if (!deviceExists)
            {
                var device = new
                {
                    deviceId = deviceId
                };
                var body = JsonConvert.SerializeObject(device);
                var stringContent = new StringContent(body, Encoding.UTF8, "application/json");
                var putResult = iotHubHttpClient.PutAsync(url, stringContent).Result;
            }
        }

        private void PostEventToIoTHub(string deviceId, string message)
        {
            var stringContent = new StringContent(message, Encoding.UTF8, "application/json");
            var url = string.Format("devices/{0}/messages/events?api-version=2016-02-03",
                deviceId);
            var result = iotHubHttpClient.PostAsync(url, stringContent).Result;
        }
    }
}