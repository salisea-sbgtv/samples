using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Moq;

namespace EventGridReceiver.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private static Guid EntityId { get; set; } = new Guid("48811c0c-551c-4a1b-93d1-2e1018477fe0");
        private static string DisplayValue { get; set; } = "Test Entity";

        const string EVENT_TYPE = "Entity.Created";

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
        }

        [TestMethod]
        public void TestMethod1()
        {
            // Create mock logger that does nothing.
            Microsoft.Extensions.Logging.Abstractions.NullLogger<object> log = new Microsoft.Extensions.Logging.Abstractions.NullLogger<object>();

            // Create mock HttpRequest 
            Dictionary<string, StringValues> query = new Dictionary<string, StringValues>();

            var req = new Mock<HttpRequest>();

            req.Setup(r => r.Path).Returns(new PathString($"/api/onentitycreated"));
            req.Setup(r => r.Query).Returns(new QueryCollection(query));
            req.Setup(r => r.Headers).Returns(new HeaderDictionary());

            EventGridEventPublisher.Models.Entity entity = new EventGridEventPublisher.Models.Entity();
            entity.EntityId = EntityId;
            entity.DisplayValue = DisplayValue;

            EventGridEventReceiver.Models.EventData evt = new EventGridEventReceiver.Models.EventData();

            evt.id = Guid.NewGuid().ToString();
            evt.eventType = EVENT_TYPE;
            evt.eventTime = DateTime.Now.ToString();
            evt.data = entity;

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write($"[{JsonConvert.SerializeObject(evt)}]");
            writer.Flush();
            stream.Position = 0;
            req.Setup(r => r.Body).Returns(stream);

            var receiver = new EventGridEventReceiver.ReceiveEntityEvent();

            var response = receiver.Run(req.Object, log).Result;

            // Assert
            Assert.IsTrue(response.GetType() == typeof(OkObjectResult));
        }
    }
}