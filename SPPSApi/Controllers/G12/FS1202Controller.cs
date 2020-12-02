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
    [Route("api/FS1202/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS1202Controller : BaseController
    {
        FS0102_Logic fs0102_Logic = new FS0102_Logic();
        private readonly string FunctionID = "FS1202";
        
        private readonly IWebHostEnvironment _webHostEnvironment;
        

        public FS1202Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }


    }
}
