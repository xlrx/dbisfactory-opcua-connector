using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestCore.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestCore.Controllers
{
    [Route("api/batchqueue/batchjob/workpiece/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class FormatController : Controller
    {
        /// <summary>
        /// Get all Format
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(200, typeof(Format))]
        public IActionResult GetAll()
        {
            List<Format> formats = new List<Format>();

            for (int i = 0; i < Program.batchJobQueue.BatchJobs.Count; i++)
            {
                for (int j = 0; j < Program.batchJobQueue.BatchJobs[i].Workpieces.Count; j++)
                {
                    formats.Add(Program.batchJobQueue.BatchJobs[i].Workpieces[j].Formate[0]);
                }
            }

            if (formats == null)
            {
                return NotFound();
            }

            return new ObjectResult(formats);

        }

        // PUT api/<controller>/5
        /// <summary>
        /// Update the Format
        /// </summary>
        /// <param name="id_bj">ID of BatchJob</param>
        /// <param name="id_w">ID of Workpieces</param>
        /// <param name="format"></param>
        /// <returns></returns>
        [HttpPut]//("{id}")]
        [SwaggerResponse(204)]
        public IActionResult Update([FromBody] Format format, int id_bj, int id_w)
        {
            if (format == null)// || format.Id != id)
            {
                return BadRequest();
            }

            try
            {
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.FurEnabled", Convert.ToBoolean(format.FurEnabled));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.SawEnabled", Convert.ToBoolean(format.SawEnabled));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.ColorCheckEnabled", Convert.ToBoolean(format.ColorCheckEnabled));                                                                          
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.EjectEnabled", Convert.ToBoolean(format.EjectEnabled));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.FurDuration", Convert.ToUInt32(format.FurDuration));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.SawDuration", Convert.ToUInt32(format.SawDuration));
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return new NoContentResult();
        }

        // DELETE api/<controller>/5
        /// <summary>
        /// Reset the Format
        /// </summary>
        /// <param name="id_bj">ID of BatchJob</param>
        /// <param name="id_w">ID of Workpieces</param>
        /// <returns></returns>
        [HttpDelete]//("{id}")]
        [SwaggerResponse(204)]
        public IActionResult Delete(int id_bj, int id_w)
        {
            try
            {
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.FurEnabled", Convert.ToBoolean(false));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.SawEnabled", Convert.ToBoolean(false));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.ColorCheckEnabled", Convert.ToBoolean(false));                                                                           
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.EjectEnabled", Convert.ToBoolean(false));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.FurDuration", Convert.ToUInt32(0));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.SawDuration", Convert.ToUInt32(0));

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return new NoContentResult();
        }
    }
}
