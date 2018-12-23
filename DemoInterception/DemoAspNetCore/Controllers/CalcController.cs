using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DemoAspNetCore.Controllers
{
    //[Route("api/[controller]/[action]")]
    [Route("api/[controller]")]
    [ApiController]
    public class CalcController : ControllerBase
    {
        Interfaces.ICalcService doubleService;
        Interfaces.ICalcService tripleService;
        public CalcController(Services.DoubleService doubleService, Interfaces.ICalcService tripleService)
        {
            this.doubleService = doubleService;
            this.tripleService = tripleService;
        }

        // GET api/calc/double
        //[Route("[action]")]
        [Route("double")]
        [HttpGet]
        public ActionResult<int> Double(int val = 2)
        {
            var result = doubleService.Calc(val);
            return result;
        }

        // GET api/calc/triple
        //[Route("[action]")]
        [Route("triple")]
        [HttpGet]
        public ActionResult<int> Triple(int val = 2)
        {
            var result = tripleService.Calc(val);
            return result;
        }
    }
}
