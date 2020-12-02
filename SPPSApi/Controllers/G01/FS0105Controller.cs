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
 


namespace SPPSApi.Controllers.G00
{
    [Route("api/FS0105/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0105Controller : BaseController
    {
        FS0105_Logic fs0105_Logic = new FS0105_Logic();
        private readonly string FunctionID = "FS0105";
        
        private readonly IWebHostEnvironment _webHostEnvironment;
        

        public FS0105Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

       

    }
}
