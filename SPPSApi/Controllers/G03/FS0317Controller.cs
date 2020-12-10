using Logic;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace SPPSApi.Controllers.G03
{
    [Route("api/FS0317/[action]")]
    [EnableCors("any")]
    [ApiController]
    public class FS0317Controller : BaseController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string FunctionID = "FS0317";
        FS0317_Logic fs0317_logic = new FS0317_Logic();

        public FS0317Controller(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
    }
}