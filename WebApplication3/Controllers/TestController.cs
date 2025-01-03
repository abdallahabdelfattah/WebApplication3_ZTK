using Microsoft.AspNetCore.Mvc;
using ZKTEco_Biometric_Device_FW;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // GET: api/<TestController>
        [HttpGet]
        public IActionResult Get(string iface_Ip, int port, int commKey, DateTime startDate, DateTime endDate)
        {
            ZkHelperTest _zkHelperTest   = new ZkHelperTest();
           var result =  _zkHelperTest.GetAttendanceLogs(iface_Ip, port, commKey, startDate, endDate);
           return  Ok(result);
        }



        // GET: api/<TestController>
        [HttpPost]
        public IActionResult DeleteLogsByTimeRange(string iface_Ip, int port, int commKey, DateTime startDate, DateTime endDate)
        {
            ZkHelperTest _zkHelperTest = new ZkHelperTest();
            var result = _zkHelperTest.DeleteAttendanceLogsByRange(iface_Ip, port, commKey, startDate, endDate);
            return Ok(result);
        }



    }
}
