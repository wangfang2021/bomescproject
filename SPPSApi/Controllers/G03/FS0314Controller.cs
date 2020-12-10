using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0314/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0314Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0314";
        FS0314_Logic fs0314_logic = new FS0314_Logic();

        public FS0314Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
    }
}