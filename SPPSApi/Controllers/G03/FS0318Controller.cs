using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0318/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0318Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0318";
        FS0318_Logic fs0318_logic = new FS0318_Logic();

        public FS0318Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
    }
}