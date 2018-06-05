using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using StackExchange.Redis;

namespace API_SERVER.Controllers
{
    [EnableCors("AllowSameDomain")]
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5/6
        [HttpGet("{id}")]
        public string Get(string id)
        {
            



            return id;
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post([FromBody]string value)
        {
            var re = Request;
            var headers = re.Headers;

            if (headers.ContainsKey("token") && headers.ContainsKey("userid"))
            {
                string token = headers["token"].ToString();
                string userid = headers["userid"].ToString();
                using (var client = StackExchange.Redis.ConnectionMultiplexer.Connect("118.190.125.175:16379"))
                {
                    var db = client.GetDatabase(0);

                    var tokenRedis = db.StringGet(userid);
                    Console.WriteLine(tokenRedis.ToString());
                }
                var rp = Response;
                var rpHeader = rp.Headers;
                rpHeader.Add("msg", "test");
                rpHeader.Add("code", "1");

            }

            return Json(new { re = "123" });
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
