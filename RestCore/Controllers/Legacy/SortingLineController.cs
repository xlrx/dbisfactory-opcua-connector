using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestCore.Models;

namespace RestCore.Controllers
{
    [Produces("application/json")]
    [Route("api/SortingLine")]
    [ApiExplorerSettings(GroupName = "v2")]
    public class SortingLineController : Controller
    {
        // GET: api/SortingLine
        /// <summary>
        /// Farbsensor
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("colorSensor")]
        public IActionResult GetcolorSensor()
        {
            return new ObjectResult(Program.sortingLine.colorSensor);
        }
        // GET: api/SortingLine
        /// <summary>
        /// Impulstaster
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("iFeeler")]
        public IActionResult GetiFeeler()
        {
            return new ObjectResult(Program.sortingLine.iFeeler);
        }
        // GET: api/SortingLine
        /// <summary>
        /// Lichtschranke Eingang
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("lbEntry")]
        public IActionResult GetlbEntry()
        {
            return new ObjectResult(Program.sortingLine.lbEntry);
        }
        // GET: api/SortingLine
        /// <summary>
        /// Lichtschranke nach Farbsensor
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("lbAfterColorSensor")]
        public IActionResult GetlbAfterColorSensor()
        {
            return new ObjectResult(Program.sortingLine.lbAfterColorSensor);
        }
        // GET: api/SortingLine
        /// <summary>
        /// Lichtschranke weiß
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("lbWhite")]
        public IActionResult GetlbWhite()
        {
            return new ObjectResult(Program.sortingLine.lbWhite);
        }
        // GET: api/SortingLine
        /// <summary>
        /// Lichtschranke rot
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("lbRed")]
        public IActionResult GetlbRed()
        {
            return new ObjectResult(Program.sortingLine.lbRed);
        }
        // GET: api/SortingLine
        /// <summary>
        /// Lichtschranke blau
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("lbBlue")]
        public IActionResult GetlbBlue()
        {
            return new ObjectResult(Program.sortingLine.lbBlue);
        }
        // GET: api/SortingLine
        /// <summary>
        /// Motor Förderband
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mConveyorBelt")]
        public IActionResult GetmConveyorBelt()
        {
            return new ObjectResult(Program.sortingLine.mConveyorBelt);
        }
        /// <summary>
        /// Motor Förderband
        /// </summary>
        /// <param name="mConveyorBelt_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mConveyorBelt")]
        public IActionResult SetmConveyorBelt(bool mConveyorBelt_value)
        {
            Program.client.Write_node("SL.mConveyorBelt", mConveyorBelt_value);
            return new NoContentResult();
        }

        // GET: api/SortingLine
        /// <summary>
        /// Kompressor
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("compressor")]
        public IActionResult Getcompressor()
        {
            return new ObjectResult(Program.sortingLine.compressor);
        }
        /// <summary>
        /// Kompressor
        /// </summary>
        /// <param name="compressor_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("compressor")]
        public IActionResult Setcompressor(bool compressor_value)
        {
            Program.client.Write_node("SL.compressor", compressor_value);
            return new NoContentResult();
        }

        // GET: api/SortingLine
        /// <summary>
        /// Ventil erster Auswurf (weiß)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("valveFirstEject")]
        public IActionResult GetvalveFirstEject()
        {
            return new ObjectResult(Program.sortingLine.valveFirstEject);
        }
        /// <summary>
        /// Ventil erster Auswurf (weiß)
        /// </summary>
        /// <param name="valveFirstEject_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("valveFirstEject")]
        public IActionResult SetvalveFirstEject(bool valveFirstEject_value)
        {
            Program.client.Write_node("SL.valveFirstEject", valveFirstEject_value);
            return new NoContentResult();
        }

        // GET: api/SortingLine
        /// <summary>
        /// Ventil zweiter Auswurf (rot)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("valveSecEject")]
        public IActionResult GetvalveSecEject()
        {
            return new ObjectResult(Program.sortingLine.valveSecEject);
        }
        /// <summary>
        /// Ventil zweiter Auswurf (rot)
        /// </summary>
        /// <param name="valveSecEject_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("valveSecEject")]
        public IActionResult SetvalveSecEject(bool valveSecEject_value)
        {
            Program.client.Write_node("SL.valveSecEject", valveSecEject_value);
            return new NoContentResult();
        }

        // GET: api/SortingLine
        /// <summary>
        /// Ventil dritter Auswurf (blau)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("valveThirdEject")]
        public IActionResult GetvalveThirdEject()
        {
            return new ObjectResult(Program.sortingLine.valveThirdEject);
        }
        /// <summary>
        /// Ventil dritter Auswurf (blau)
        /// </summary>
        /// <param name="valveThirdEject_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("valveThirdEject")]
        public IActionResult SetvalveThirdEject(bool valveThirdEject_value)
        {
            Program.client.Write_node("SL.valveThirdEject", valveThirdEject_value);
            return new NoContentResult();
        }

        // GET: api/SortingLine
        [HttpGet]
        [Route("any_motor_running")]
        public IActionResult Getany_motor_running()
        {
            return new ObjectResult(Program.sortingLine.any_motor_running);
        }

        // GET: api/SortingLine
        [HttpGet]
        [Route("state")]
        public IActionResult Getstate()
        {
            return new ObjectResult(Program.sortingLine.state);
        }

        // GET: api/SortingLine
        [HttpGet]
        [Route("bIdx")]
        public IActionResult bIdx()
        {
            return new ObjectResult(Program.sortingLine.bIdx);
        }

        // GET: api/SortingLine
        [HttpGet]
        [Route("wIdx")]
        public IActionResult wIdx()
        {
            return new ObjectResult(Program.sortingLine.wIdx);
        }
    }
}
