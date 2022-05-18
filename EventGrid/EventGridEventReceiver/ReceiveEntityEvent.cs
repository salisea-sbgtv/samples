using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EventGridEventReceiver
{
    public class ReceiveEntityEvent
    {
        [FunctionName("ReceiveEntityEvent")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("ReceiveEntityEvent processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            log.LogInformation($"Request Body {requestBody}");

            JArray events = JsonConvert.DeserializeObject<JArray>(requestBody);

            // IMPORTANT!:  REQUIRED CODE...
            // ---------------------------------------------------------------------------------
            // If the request is for subscription validation, echo back the validation code.
            // Your Event Grid subscription will not be created unless this works.
            if (events.Count > 0 && string.Equals((string)events[0]["eventType"],
            "Microsoft.EventGrid.SubscriptionValidationEvent",
            System.StringComparison.OrdinalIgnoreCase))
            {
                log.LogInformation("Validate request received");
                return new OkObjectResult(new
                {
                    validationResponse = events[0]["data"]["validationCode"]
                });
            }
            // ---------------------------------------------------------------------------------

            try
            {
                // Deserialize event data
                Models.EventData evt = events[0].ToObject<Models.EventData>();

                if (evt == null)
                    return new BadRequestResult();

                if (evt.data == null)
                    return new BadRequestResult();

                switch (evt.eventType)
                {
                    case "Entity.Created":
                        await EntityCreated(evt);
                        break;
                    default:
                        return new StatusCodeResult(400);                        
                }                

                return new OkResult();
            }            
            catch (Exception e)
            {
                log.LogError(e.Message);

                return new StatusCodeResult(500);
            }            
        }

        #region Private

        private async Task EntityCreated(Models.EventData evt)
        {
            // Do something clever
        }        

        #endregion
    }
}
