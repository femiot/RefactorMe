using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;

namespace RefactorMe
{
    internal static class StringHelper
    {
        public static string GetNamingPattern(this string name)
        {
            // Example Templates
            return name switch
            {
                "Order" => @"ORD-{date:ddMMyyyy}-{increment:order}",// ORD-12122022-01
                "Site" => @"ST-{entity:location.address.postalOrZipCode}-{increment:site}",// ST-0042-01
                "Product" => @"PRD-{increment:product}",// PRD-01
                _ => "",
            };
        }

        public static string GetDate(this string format)
        {
            return DateTime.Now.ToString(format);
        }

        // TODO: Implement redis server that can be consumed by the application
        public static string GetIncrement(this string type, IDatabase redisService)
        {
            var value = redisService.StringGet(type);
            int currentValue = 1;
            if (!value.HasValue)
                redisService.StringSet(type, currentValue);
            else
            {
                currentValue = ((int)value);
                redisService.StringSet(type, ++currentValue);
            }

            return currentValue.ToString().PadLeft(2, '0');
        }

        public static string GetValueFromEntityObject(this string attribute, object entity)
        {
            var jsonObject = JObject.Parse(JsonConvert.SerializeObject(entity));
            var entityValue = jsonObject.SelectToken(attribute);

            return entityValue?.ToString();
        }
    }
}
