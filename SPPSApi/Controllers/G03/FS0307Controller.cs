using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0307/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0307Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0307";
        FS0307_Logic fs0307_logic = new FS0307_Logic();

        public FS0307Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
    }
}