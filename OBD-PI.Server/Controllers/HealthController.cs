using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace OBDPI.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : Controller
    {
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
