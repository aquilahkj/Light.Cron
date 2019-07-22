using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Light.Cron.Sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrontabController : ControllerBase
    {
        private readonly CrontabService crontabService;

        public CrontabController(CrontabService crontabService)
        {
            this.crontabService = crontabService;
        }

        // GET api/crontab
        [HttpGet("list")]
        public ActionResult<IEnumerable<CrontabExecutor>> Get()
        {
            return this.crontabService.GetExecutors();
        }

        // GET api/crontab/5
        [HttpGet("info/{name}")]
        public ActionResult<CrontabExecutor> Get(string name)
        {
            return this.crontabService.GetExecutor(name);
        }

        // GET api/crontab/5
        [HttpGet("{enable}")]
        public ActionResult<dynamic> Enable(string name, string action)
        {
            var executor = this.crontabService.GetExecutor(name);
            int result = 0;
            if (executor == null) {
                result = -1;
            }
            if (action == "on") {
                if (executor.Enable()) {
                    result = 1;
                }
            }
            else if (action == "off") {
                if (executor.Disable()) {
                    result = 1;
                }
            }
            else {
                result = -2;
            }
            return new { result };
        }


    }
}
