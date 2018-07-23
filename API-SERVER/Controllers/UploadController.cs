using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API_SERVER.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API_SERVER.Controllers
{
    [Produces("application/json")]
    [Consumes("multipart/form-data")]
    [Route(Global.ROUTE_PX + "/[controller]/[action]")]
    public class UploadController : Controller
    {
        [HttpPost]
        [ActionName("Temp")]
        public ActionResult Temp(IFormCollection param)
        {
            return Json(Global.BUSS.BussResults(this, ApiType.UploadApi, param));
        }

    }
}