using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SPPSApi.Controllers.G12
{
    [Route("api/FS1210/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1210Controller : BaseController
    {
        FS1202_Logic logic = new FS1202_Logic();
        private readonly string FunctionID = "FS1210";
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FS1210Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
    }
}
