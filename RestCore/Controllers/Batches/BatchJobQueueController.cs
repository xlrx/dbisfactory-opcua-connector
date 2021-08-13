using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using RestCore.Models;
using Swashbuckle.AspNetCore.SwaggerGen;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RestCore.Controllers
{
    
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class BatchJobQueueController : Controller
    {

        /// <summary>
        /// Show BatchQueues
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SwaggerResponse(200, typeof(BatchJobQueue), "Successfully get BatchJobQueue")]
        [SwaggerResponse(404, Description = "BatchJobQueue doesn't exist")]
        public IActionResult GetAll()
        {
            BatchJobQueue batchQueue = Program.batchJobQueue;
            if (batchQueue == null)
            {
                return NotFound();
            }
            return new ObjectResult(batchQueue);
        }

        ///// <summary>
        ///// Get BatchJobQueue by ID
        ///// </summary>
        ///// <param name="id">ID of BatchJobQueue</param>
        ///// <returns></returns>
        //// GET api/<controller>/5
        //[HttpGet("{id}", Name = "GetBatchQueue")]
        //[SwaggerResponse(200,typeof(BatchJobQueue), "Successfully get BatchJobQueue by ID")]
        //public IActionResult GetById(int id) 
        //{
        //    BatchJobQueue batchQueue = Program.batchJobQueue;
        //    if (batchQueue == null)
        //    {
        //        return NotFound();
        //    }
        //    return new ObjectResult(batchQueue);
        //}

        ///// <summary>
        ///// Get all BatchJobs from BatchJobQueue
        ///// </summary>
        ///// <returns></returns>
        //[Route("{id}/batchjob")]
        //[HttpGet]
        //[SwaggerResponse(200, typeof(BatchJob), "Successfully get all BatchJobs from BatchJobQueue")]
        //[SwaggerResponse(404, Description = "BatchJobs doesn't exist")]
        //public IActionResult GetByIdBatchJobs()
        //{
        //    List<BatchJob> batchJob = Program.batchJobQueue.BatchJobs;
        //    if (batchJob == null)
        //    {
        //        return NotFound();
        //    }
        //    return new ObjectResult(batchJob);
        //}


        ///// <summary>
        ///// Get BatchJob by ID
        ///// </summary>
        ///// <param name="id">ID of BatchJob</param>
        ///// <returns></returns>
        //[Route("{id}/batchjob/{id2}")]
        //[HttpGet]
        //[SwaggerResponse(200, typeof(BatchJob), "Successfully get BatchJob by ID")]
        //[SwaggerResponse(404, Description = "BatchJob doesn't exist")]
        //public IActionResult GetByIdBatchJob(int id)
        //{
        //    BatchJob batchJob = null;
        //    try
        //    {
        //        batchJob = Program.batchJobQueue.BatchJobs[id - 1];
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //    }
        //    if (batchJob == null)
        //    {
        //        return NotFound();
        //    }
        //    return new ObjectResult(batchJob);
        //}

        /// <summary>
        /// Get all Workpieces
        /// </summary>
        /// <param name="id">ID of BatchJob</param>
        /// <returns></returns>
        [Route("batchjob/{id}/workpiece")]
        [HttpGet]
        [SwaggerResponse(200, typeof(Workpiece), "Successfully get all Workpieces ")]
        [SwaggerResponse(404, Description = "Workpieces doesn't exist")]
        public IActionResult GetByIdWorkpieces(int id)
        {
            List<Workpiece> workpiece = null;
            try
            {
                workpiece = Program.batchJobQueue.BatchJobs[id - 1].Workpieces;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            if (workpiece == null)
            {
                return NotFound();
            }
            return new ObjectResult(workpiece);
        }

        /// <summary>
        /// Get Workpieces by ID
        /// </summary>
        /// <param name="id_bj">ID of BatchJob</param>
        /// <param name="id_w">ID of Workpieces</param>
        /// <returns></returns>
        [Route("batchjob/{id_bj}/workpiece/{id_w}")]
        [HttpGet]
        [SwaggerResponse(200, typeof(Workpiece), "Successfully get Workpieces by ID ")]
        [SwaggerResponse(404, Description = "Workpiece doesn't exist")]
        public IActionResult GetByIdWorkpiece(int id_bj, int id_w)
        {
            Workpiece workpiece = null;
            try
            {
                workpiece = Program.batchJobQueue.BatchJobs[id_bj - 1].Workpieces[id_w - 1];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            if (workpiece == null)
            {
                return NotFound();
            }
            return new ObjectResult(workpiece);
        }

        /// <summary>
        /// Get Formate
        /// </summary>
        /// <param name="id_bj">ID of BatchJob</param>
        /// <param name="id_w">ID of Workpieces</param>
        /// <returns></returns>
        [Route("batchjob/{id_bj}/workpiece/{id_w}/format")]
        [HttpGet]
        [SwaggerResponse(200, typeof(Format), "Successfully get Format")]
        [SwaggerResponse(404, Description = "Format doesn't exist")]
        public IActionResult GetByIdFormats(int id_bj, int id_w)
        {
            Format format = null;
            try
            {
                format = Program.batchJobQueue.BatchJobs[id_bj - 1].Workpieces[id_w - 1].Formate[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            if (format == null)
            {
                return NotFound();
            }
            return new ObjectResult(format);
        }

        //[Route("{id}/batchjob/{id2}/workpiece/{id3}/format/{id4}")]
        //[HttpGet]
        //[SwaggerResponse(200, typeof(Format))]
        //public IActionResult GetByIdFormat(int id, int id2, int id3, int id4)
        //{
        //    Format formate = null;
        //    try
        //    {
        //        formate = Program.batchJobQueue.BatchJobs[id2 - 1].Workpieces[id3 - 1].Formate[id4 - 1];
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //    }
        //    if (formate == null)
        //    {
        //        return NotFound();
        //    }
        //    return new ObjectResult(formate);
        //}
    }
}
