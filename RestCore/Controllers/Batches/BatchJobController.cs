using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestCore.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestCore.Controllers
{
    [Route("api/batchqueue/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class BatchJobController : Controller
    {
        /// <summary>
        /// Get all BatchJobs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(200, typeof(BatchJob), "Successfully get all BatchJobs")]
        [SwaggerResponse(404, Description = "BatchJobs doesn't exist")]
        public IActionResult GetAll()
        {
            List<BatchJob> batchJob = new List<BatchJob>();
            try
            {
                for (int i = 0; i < Program.maxBatchJobs; i++)
                {
                    //add all batchjobs in list
                    batchJob.Add(Program.batchJobQueue.BatchJobs[i]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            if (batchJob == null)
            {
                return NotFound();
            }

            return new ObjectResult(batchJob);

        }

        /// <summary>
        /// Get BatchJob by ID
        /// </summary>
        /// <param name="id">ID of BatchJob</param>
        /// <returns></returns>
        // GET api/<controller>/5
        [HttpGet("{id}", Name = "GetBatchJob")]
        [SwaggerResponse(200, typeof(BatchJob), "Successfully get BatchJob by ID")]
        [SwaggerResponse(404, Description = "BatchJob doesn't exist")]
        public IActionResult GetById(int id)
        {
            BatchJob batchJob = null;
            try
            {
                //get the batchjob from id,  -1 because codesys array starts with 1
                batchJob = Program.batchJobQueue.BatchJobs[id - 1];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            if (batchJob == null)
            {
                return NotFound();
            }
            return new ObjectResult(batchJob);
        }



        // POST api/<controller>
        /// <summary>
        /// Add BatchJob to BatchJobQueue
        /// </summary>
        /// <param name="batchJob">Add to Queue</param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerResponse(400, Description = "BatchJobQueue is full")]
        [SwaggerResponse(201, typeof(BatchJob), "Successfully add BatchJob to BatchJobQueue")]
        public IActionResult Create([FromBody] BatchJob batchJob)
        {
            if (Convert.ToBoolean(Program.client.Read_node("TC.batchJobQueue.Full")) == true)
            {
                //read full flag
                return BadRequest("BatchJobQueue is full");
            }

            try
            {
                int amount = Convert.ToInt16(Program.client.Read_node("TC.batchJobQueue.AmountBatchJobs"));
                Program.client.Write_node("TC.batchJobQueue.AmountBatchJobs", Convert.ToInt16(amount + 1)); //amountbatchjobs + 1
                if (amount + 1 >= Program.maxBatchJobs)
                {
                    //when queue full, set flag
                    Program.client.Write_node("TC.batchJobQueue.Full", true); //full
                }
                Program.client.Write_node("TC.batchJobQueue.NextFreePos", Convert.ToInt16(Convert.ToInt16(Program.client.Read_node("TC.batchJobQueue.NextFreePos")) + 1)); //nextfreepos + 1

                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].BatchSize", Convert.ToInt16(batchJob.BatchSize));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].AmountFinishedW", Convert.ToInt16(0));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].NextNotProcessedIdx", Convert.ToInt16(-1));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].Mode", Convert.ToInt16(batchJob.Mode));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].state", Convert.ToInt16(BatchJobState.WAITING));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].NextFreeWPos", Convert.ToInt16(0));

                //workpieces
                for (int i = 1; i <= batchJob.BatchSize; i++)
                {
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].Workpieces[" + i + "].Color", Convert.ToString(batchJob.Workpieces[i-1].Color)); //color
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].Workpieces[" + i + "].LinePos", Convert.ToInt16(0));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].Workpieces[" + i + "].SortingLinePos", Convert.ToInt16(-1));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].Workpieces[" + i + "].State", Convert.ToInt16(batchJob.Workpieces[i - 1].State));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].Workpieces[" + i + "].StartPos", Convert.ToInt16(batchJob.Workpieces[i - 1].StartPos));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].Workpieces[" + i + "].EndPos", Convert.ToInt16(batchJob.Workpieces[i - 1].EndPos));


                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].Workpieces[" + i + "].Format.FurEnabled", Convert.ToBoolean(batchJob.Workpieces[i - 1].Formate[0].FurEnabled));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].Workpieces[" + i + "].Format.SawEnabled", Convert.ToBoolean(batchJob.Workpieces[i - 1].Formate[0].SawEnabled));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].Workpieces[" + i + "].Format.ColorCheckEnabled", Convert.ToBoolean(batchJob.Workpieces[i-1].Formate[0].ColorCheckEnabled));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].Workpieces[" + i + "].Format.EjectEnabled", Convert.ToBoolean(batchJob.Workpieces[i - 1].Formate[0].EjectEnabled));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].Workpieces[" + i + "].Format.FurDuration", Convert.ToUInt32(batchJob.Workpieces[i - 1].Formate[0].FurDuration));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + (amount + 1) + "].Workpieces[" + i + "].Format.SawDuration", Convert.ToUInt32(batchJob.Workpieces[i - 1].Formate[0].SawDuration));
                }

                return CreatedAtRoute("GetBatchJob",new {id = amount + 1 }  ,batchJob);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return BadRequest();
        }

        // PUT api/<controller>/5
        /// <summary>
        /// Update the BatchJob
        /// </summary>
        /// <param name="id">ID of BatchJob</param>
        /// <param name="batchJob">Update the Queue</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [SwaggerResponse(204, Description = "Successfully Update the BatchJob ")]
        public IActionResult Update(int id, [FromBody] BatchJob batchJob)
        {
            //if (batchJob.Id != id) //batchJob == null || 
            //{
            //    Console.WriteLine("errrrrrror");
            //    return BadRequest();
            //}

            //get the id
            
            try
            {

                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].BatchSize", Convert.ToInt16(batchJob.BatchSize));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].AmountFinishedW", Convert.ToInt16(batchJob.AmountFinishedW));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].NextNotProcessedIdx", Convert.ToInt16(batchJob.NextNotProcessedIdx));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Mode", Convert.ToInt16(batchJob.Mode));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].state", Convert.ToInt16(batchJob.state));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].NextFreeWPos", Convert.ToInt16(batchJob.NextFreeWPos));

                int size = Convert.ToInt16(Program.client.Read_node("TC.batchJobQueue.batchJobs[" + id + "].BatchSize"));

                for (int i = 1; i <= size; i++)
                {
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].Color", Convert.ToString(batchJob.Workpieces[i - 1].Color)); //color
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].LinePos", Convert.ToInt16(batchJob.Workpieces[i - 1].LinePos));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].SortingLinePos", Convert.ToInt16(batchJob.Workpieces[i - 1].SortingLinePos));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].State", Convert.ToInt16(batchJob.Workpieces[i - 1].State));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].StartPos", Convert.ToInt16(batchJob.Workpieces[i - 1].StartPos));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].EndPos", Convert.ToInt16(batchJob.Workpieces[i - 1].EndPos));


                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].Format.FurEnabled", Convert.ToBoolean(batchJob.Workpieces[i - 1].Formate[0].FurEnabled));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].Format.SawEnabled", Convert.ToBoolean(batchJob.Workpieces[i - 1].Formate[0].SawEnabled));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].Format.ColorCheckEnabled", Convert.ToBoolean(batchJob.Workpieces[i - 1].Formate[0].ColorCheckEnabled));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].Format.EjectEnabled", Convert.ToBoolean(batchJob.Workpieces[i - 1].Formate[0].EjectEnabled));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].Format.FurDuration", Convert.ToUInt32(batchJob.Workpieces[i - 1].Formate[0].FurDuration));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].Format.SawDuration", Convert.ToUInt32(batchJob.Workpieces[i - 1].Formate[0].SawDuration));
                }




                //BatchJob bj = Program.batchJobQueue.BatchJobs[id - 1];
                //if (bj == null)
                //{
                //    return NotFound();
                //}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
            

            //Program.batchJobQueue.BatchJobs[id - 1] = batchJob;

            return new NoContentResult();
        }

        // DELETE api/<controller>/5
        /// <summary>
        /// Reset the BatchJob
        /// </summary>
        /// <param name="id">ID of BatchJob</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [SwaggerResponse(204, Description = "Successfully Reset the BatchJob")]
        public IActionResult Delete(int id)
        {
            try
            {
                int size = Convert.ToInt16(Program.client.Read_node("TC.batchJobQueue.batchJobs[" + id + "].BatchSize"));

                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].BatchSize", Convert.ToInt16(0));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].AmountFinishedW", Convert.ToInt16(0));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].NextNotProcessedIdx", Convert.ToInt16(0));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Mode", Convert.ToInt16(0));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].state", Convert.ToInt16(0));
                Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].NextFreeWPos", Convert.ToInt16(1));

               

                for (int i = 1; i <= size; i++)
                {
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].Color", Convert.ToString("")); //color
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].LinePos", Convert.ToInt16(0));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].SortingLinePos", Convert.ToInt16(0));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].State", Convert.ToInt16(0));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].StartPos", Convert.ToInt16(0));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].EndPos", Convert.ToInt16(0));


                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].Format.FurEnabled", Convert.ToBoolean(false));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].Format.SawEnabled", Convert.ToBoolean(false));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].Format.ColorCheckEnabled", Convert.ToBoolean(false));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].Format.EjectEnabled", Convert.ToBoolean(false));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].Format.FurDuration", Convert.ToUInt32(0));
                    Program.client.Write_node("TC.batchJobQueue.batchJobs[" + id + "].Workpieces[" + i + "].Format.SawDuration", Convert.ToUInt32(0));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            //var bj = _contextbj.BatchJobs.FirstOrDefault(t => t.Id == id);
            //if (bj == null)
            //{
            //    return NotFound();
            //}
            //_contextbj.BatchJobs.Remove(bj);
            //_contextbj.SaveChanges();
            return new NoContentResult();
        }
    }
}
