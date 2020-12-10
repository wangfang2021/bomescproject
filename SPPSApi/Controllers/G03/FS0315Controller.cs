using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0315/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0315Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0315";
        FS0315_Logic fs0315_logic = new FS0315_Logic();

        public FS0315Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
    }
}