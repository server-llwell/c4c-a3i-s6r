using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace API_SERVER.Common
{
    public class Utils
    {
        

        public static bool SetCache(string key, object value, int hours, int minutes, int seconds)
        {
            using (var client = ConnectionMultiplexer.Connect(Global.REDIS))
            {
                var db = client.GetDatabase(Global.REDIS_NO);
                var expiry = new TimeSpan(hours, minutes, seconds);
                string valueStr = JsonConvert.SerializeObject(value);
                return db.StringSet(key, valueStr, expiry);
            }                
        }

        public static dynamic GetCache<T>(string key)
        {
            using (var client = ConnectionMultiplexer.Connect(Global.REDIS))
            {
                var db = client.GetDatabase(Global.REDIS_NO);
                if (db.StringGet(key).HasValue)
                {
                    return JsonConvert.DeserializeObject<T>(db.StringGet(key));
                }
                return null;
            }           
        }

        public static bool DeleteCache(string key)
        {
            using (var client = ConnectionMultiplexer.Connect(Global.REDIS))
            {
                var db = client.GetDatabase(Global.REDIS_NO);
                if (db.StringGet(key).HasValue)
                {
                    return db.KeyDelete(key);
                }
                return true;
            }                                       
        }
    }
}
