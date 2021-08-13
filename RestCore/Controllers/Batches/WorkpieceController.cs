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
    [Route("api/batchqueue/batchjob/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class WorkpieceController : Controller
    {
        /// <summary>
        /// Get all Workpiece
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(200, typeof(Workpiece))]
        public IActionResult GetAll()
        {

            List<Workpiece> workpieces = new List<Workpiece>();

            for (int i = 0; i < Program.batchJobQueue.BatchJobs.Count; i++)
            {
                for (int j = 0; j < Program.batchJobQueue.BatchJobs[i].Workpieces.Count; j++)
                {
                    workpieces.Add(Program.batchJobQueue.BatchJobs[i].Workpieces[j]);
                }
            }

            if (workpieces == null)
            {
                return NotFound();
            }

            return new ObjectResult(workpieces);

        }

        /// <summary>
        /// Get Workpiece by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetWorkpiece")]
        [SwaggerResponse(200, typeof(Workpiece))]
        public IActionResult GetById(int id)
        {

            List<Workpiece> workpieces = new List<Workpiece>();
            Workpiece w = null;
            try
            {
                for (int i = 0; i < Program.batchJobQueue.BatchJobs.Count; i++)
                {
                    for (int j = 0; j < Program.batchJobQueue.BatchJobs[i].Workpieces.Count; j++)
                    {
                        workpieces.Add(Program.batchJobQueue.BatchJobs[i].Workpieces[j]);
                        
                    }
                }

                w = workpieces[id - 1];

            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            if (w == null)
            {
                return NotFound();
            }

            return new ObjectResult(w);

        }


        // POST api/<controller>
        /// <summary>
        /// Add Workpiece to BatchJob
        /// </summary>
        /// <param name="id_bj">ID of BatchJob</param>
        /// <param name="workpiece"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(400)]
        [SwaggerResponse(201, typeof(Workpiece))]
        public IActionResult Create([FromBody] Workpiece workpiece, int id_bj)
        {
            if (workpiece == null)
            {
                return BadRequest();
            }

            //get free position of workpiece
            int freepos = Convert.ToInt16(Program.client.Read_node("TC.batchJobQueue.batchJobs[" + id_bj + "].NextFreeWPos"));
            if(freepos < Program.maxWorkpieces && freepos != -1)
            {
                //there is a free position for workpiece
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].NextFreeWPos", Convert.ToInt16(freepos + 1));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].BatchSize", Convert.ToInt16(Convert.ToInt16(Program.client.Read_node("TC.batchJobQueue.batchJobs[" + id_bj + "].BatchSize")) + 1));
            }
            else if (freepos == Program.maxWorkpieces)
            {
                //set nextfreewpos to full value
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].NextFreeWPos", Convert.ToInt16(-1));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].BatchSize", Convert.ToInt16(Convert.ToInt16(Program.client.Read_node("TC.batchJobQueue.batchJobs[" + id_bj + "].BatchSize")) + 1));
            }
            else
            {
                return BadRequest("BatchJob Full");
            }
            
            try
            {
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + freepos + "].Color", Convert.ToString(workpiece.Color)); //color
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + freepos + "].LinePos", Convert.ToInt16(workpiece.LinePos));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + freepos + "].SortingLinePos", Convert.ToInt16(workpiece.SortingLinePos));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + freepos + "].State", Convert.ToInt16(workpiece.State));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + freepos + "].StartPos", Convert.ToInt16(workpiece.StartPos));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + freepos + "].EndPos", Convert.ToInt16(workpiece.EndPos));


                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + freepos + "].Format.FurEnabled", Convert.ToBoolean(workpiece.Formate[0].FurEnabled));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + freepos + "].Format.SawEnabled", Convert.ToBoolean(workpiece.Formate[0].SawEnabled));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + freepos + "].Format.ColorCheckEnabled", Convert.ToBoolean(workpiece.Formate[0].ColorCheckEnabled));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + freepos + "].Format.EjectEnabled", Convert.ToBoolean(workpiece.Formate[0].EjectEnabled));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + freepos + "].Format.FurDuration", Convert.ToUInt32(workpiece.Formate[0].FurDuration));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + freepos + "].Format.SawDuration", Convert.ToUInt32(workpiece.Formate[0].SawDuration));
                return CreatedAtRoute("GetWorkpiece",new {id = freepos }, workpiece);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return BadRequest();

        }

        // PUT api/<controller>/
        /// <summary>
        /// Update the Workpiece
        /// </summary>
        /// <param name="id_bj">ID of BatchJob</param>
        /// <param name="id_w">ID of Workpieces</param>
        /// <param name="workpiece"></param>
        /// <returns></returns>
        [HttpPut]//("{id}")]
        [SwaggerResponse(204)]
        public IActionResult Update(int id_w, [FromBody] Workpiece workpiece, int id_bj)
        {
            if (workpiece == null)// || workpiece.Id != id)
            {
                return BadRequest();
            }

            try
            {
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Color", Convert.ToString(workpiece.Color)); //color
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].LinePos", Convert.ToInt16(workpiece.LinePos));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].SortingLinePos", Convert.ToInt16(workpiece.SortingLinePos));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].State", Convert.ToInt16(workpiece.State));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].StartPos", Convert.ToInt16(workpiece.StartPos));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].EndPos", Convert.ToInt16(workpiece.EndPos));


                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.FurEnabled", Convert.ToBoolean(workpiece.Formate[0].FurEnabled));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.SawEnabled", Convert.ToBoolean(workpiece.Formate[0].SawEnabled));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.ColorCheckEnabled", Convert.ToBoolean(workpiece.Formate[0].ColorCheckEnabled));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.EjectEnabled", Convert.ToBoolean(workpiece.Formate[0].EjectEnabled));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.FurDuration", Convert.ToUInt32(workpiece.Formate[0].FurDuration));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Format.SawDuration", Convert.ToUInt32(workpiece.Formate[0].SawDuration));
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return new NoContentResult();
        }

        // DELETE api/<controller>/5
        /// <summary>
        /// Reset the Workpiece
        /// </summary>
        /// <param name="id_bj">ID of BatchJob</param>
        /// <param name="id_w">ID of Workpieces</param>
        /// <returns></returns>
        [HttpDelete]//("{id}")]
        [SwaggerResponse(204)]
        public IActionResult Delete(int id_w, int id_bj)
        {
            try
            {

                //default values
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].Color", Convert.ToString("")); //color
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].LinePos", Convert.ToInt16(0));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].SortingLinePos", Convert.ToInt16(0));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].State", Convert.ToInt16(0));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].StartPos", Convert.ToInt16(0));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id_bj + "].Workpieces[" + id_w + "].EndPos", Convert.ToInt16(0));
                                                                          
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
