using System;
using System.Collections.Generic;
using System.IO;

using System.Net.Http.Headers;
using Common;
using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace WebApplication5.Controllers.G00
{
    [Route("api/[controller]/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0101Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FS0101Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
 

    }
}
