using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

namespace EventProcessor
{
    public class EventProcessorFunction
    {
        [FunctionName("EventProcessorFunction")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "EventHubConnectionString", ConsumerGroup="messages")]EventData message, ILogger log)
        {
            var inputMessage = JObject.Parse(Encoding.UTF8.GetString(message.Body.ToArray()));
            var eventDetails = JsonConvert.DeserializeObject<EventDetails>(inputMessage.ToString());
            log.LogInformation($"C# IoT Hub trigger function processed a message: {Encoding.UTF8.GetString(message.Body.Array)}");
            var storageConnectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
            var response = new Dictionary<string, string>();
            response.Add("StorageConnectionString", storageConnectionString);
            var directMethodPayload = JsonConvert.SerializeObject(response);
            var functionObj = new EventProcessorFunction();
            functionObj.InvokeDirectMethodAsync(eventDetails, directMethodPayload).GetAwaiter().GetResult();
        }

        public async Task<CloudToDeviceMethodResult> InvokeDirectMethodAsync(EventDetails eventDetails, string directmethodPayload)
        {
            var connectionString = Environment.GetEnvironmentVariable("IoTHubConnectionString"); ;
            var serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

            var cloudToDeviceMethod = new CloudToDeviceMethod(eventDetails.DirectMethod);

            cloudToDeviceMethod.SetPayloadJson(directmethodPayload);

            var response = await serviceClient.InvokeDeviceMethodAsync(eventDetails.DeviceId, eventDetails.ModuleId, cloudToDeviceMethod).ConfigureAwait(false);
            return response;
        }
    }
}