using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Opsi.Cloud.Core;
using Opsi.Cloud.Core.Model;
using StackExchange.Redis;

namespace RefactorMe
{
    class Program
    {
        // Only for debugging purposes.

        static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(
           new ConfigurationOptions
           {
               EndPoints = { "localhost:6379" }
           });
        static async Task Main(string[] args)
        {

            var redisService = redis.GetDatabase();

            var gen = new ExternalIdService(null, redisService);

            var entity1 = new Dictionary<string, object> {
        { "id", 1 },
        { "location", new Dictionary<string, object> {
          {"address", new Dictionary<string, object> { { "postalOrZipCode", "0042" } } }
        } }
      };


            var entity2 = new Dictionary<string, object> { { "id", 2 } };

            await gen.GenerateAsync(
              new List<Dictionary<string, object>> { entity1, entity2 },
              new TypeMetadata { Name = "Site" });

            Console.WriteLine(entity1["externalId"]);
            Console.WriteLine(entity2["externalId"]);
        }
    }
}
