using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestCore.Controllers.Legacy
{
    [Produces("application/json")]
    [Route("api/TaskConfigurator")]
    [ApiExplorerSettings(GroupName = "v2")]
    public class TaskConfiguratorController : Controller
    {
        // GET: api/TaskConfigurator
        /// <summary>
        /// Start BatchJobQueue
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("started")]
        public IActionResult Getstarted()
        {
            return new ObjectResult(Program.taskConfigurator.started);
        }

        /// <summary>
        /// Start BatchJobQueue
        /// </summary>
        /// <param name="started_value">true = start, false = stop</param>
        /// <returns></returns>
        [HttpPut]
        [Route("started")]
        public IActionResult Setstarted(bool started_value)
        {

            Program.client.Write_node("TC_POU.started", started_value);
            return new NoContentResult();
        }

        // GET: api/TaskConfigurator
        /// <summary>
        /// BatchJobQueue finished
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("fin")]
        public IActionResult Getfin()
        {
            return new ObjectResult(Program.taskConfigurator.fin);
        }

        // GET: api/TaskConfigurator
        /// <summary>
        /// Is BatchJob in BatchJobQueue
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("batch_job_available")]
        public IActionResult Getbatch_job_available()
        {
            return new ObjectResult(Program.taskConfigurator.batch_job_available);
        }

        // GET: api/TaskConfigurator
        [HttpGet]
        [Route("mode_active")]
        public IActionResult Getmode_active()
        {
            return new ObjectResult(Program.taskConfigurator.mode_active);
        }
        // GET: api/TaskConfigurator
        /// <summary>
        /// PLC running
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("is_plc_running")]
        public IActionResult Getis_plc_running()
        {
            return new ObjectResult(Program.taskConfigurator.is_plc_running);
        }
        // GET: api/TaskConfigurator
        /// <summary>
        /// Hardware Stop Switch
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("instant_shutdown")]
        public IActionResult Getinstant_shutdown()
        {
            return new ObjectResult(Program.taskConfigurator.instant_shutdown);
        }
        
        // GET: api/TaskConfigurator
        [HttpGet]
        [Route("currBatchJobIdx")]
        public IActionResult GetcurrBatchJobIdx()
        {
            return new ObjectResult(Program.taskConfigurator.currBatchJobIdx);
        }
        // GET: api/TaskConfigurator
        [HttpGet]
        [Route("state")]
        public IActionResult Getstate()
        {
            return new ObjectResult(Program.taskConfigurator.state);
        }

    }
}
