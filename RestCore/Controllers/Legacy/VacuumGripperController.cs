using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestCore.Controllers.Legacy
{
    [Produces("application/json")]
    [Route("api/VacuumGripper")]
    [ApiExplorerSettings(GroupName = "v2")]
    public class VacuumGripperController : Controller
    {
        /*
        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("toPosition")]
        public IActionResult toPosition()
        {
            ///Program.client.Write_node("VG.mVerticalDown", mVerticalDown_value);
            return new NoContentResult();
        }




    */




        // GET: api/VacuumGripper
        /// <summary>
        /// Referenzschalter vertikal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sVertical")]
        public IActionResult GetsVertical()
        {
            return new ObjectResult(Program.vacuumGripper.sVertical);
        }

        // GET: api/VacuumGripper
        /// <summary>
        /// Referenzschalter horizontal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sHorizontal")]
        public IActionResult GetsHorizontal()
        {
            return new ObjectResult(Program.vacuumGripper.sHorizontal);
        }

        // GET: api/VacuumGripper
        /// <summary>
        /// Referenzschalter drehen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sRotation")]
        public IActionResult GetsRotation()
        {
            return new ObjectResult(Program.vacuumGripper.sRotation);
        }

        // GET: api/VacuumGripper
        /// <summary>
        /// Encoder vertikal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("encVertical")]
        public IActionResult GetencVertical()
        {
            return new ObjectResult(Program.vacuumGripper.encVertical);
        }

        // GET: api/VacuumGripper
        /// <summary>
        /// Encoder horizontal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("encHorizontal")]
        public IActionResult GetencHorizontal()
        {
            return new ObjectResult(Program.vacuumGripper.encHorizontal);
        }

        // GET: api/VacuumGripper
        /// <summary>
        /// Encoderdrehen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("encRotation")]
        public IActionResult GetencRotation()
        {
            return new ObjectResult(Program.vacuumGripper.encRotation);
        }

        // GET: api/VacuumGripper
        /// <summary>
        /// Motor vertikal hoch
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mVerticalUp")]
        public IActionResult GetmVerticalUp()
        {
            return new ObjectResult(Program.vacuumGripper.mVerticalUp);
        }
        /// <summary>
        /// Motor vertikal hoch
        /// </summary>
        /// <param name="mVerticalUp_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mVerticalUp")]
        public IActionResult SetmVerticalUp(bool mVerticalUp_value)
        {
            Program.client.Write_node("VG.mVerticalUp", mVerticalUp_value);
            return new NoContentResult();
        }

        // GET: api/VacuumGripper
        /// <summary>
        /// Motor vertical runter
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mVerticalDown")]
        public IActionResult GetmVerticalDown()
        {
            return new ObjectResult(Program.vacuumGripper.mVerticalDown);
        }
        /// <summary>
        /// Motor vertical runter
        /// </summary>
        /// <param name="mVerticalDown_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mVerticalDown")]
        public IActionResult SetmVerticalDown(bool mVerticalDown_value)
        {
            Program.client.Write_node("VG.mVerticalDown", mVerticalDown_value);
            return new NoContentResult();
        }

        // GET: api/VacuumGripper
        /// <summary>
        /// Motor horizontal rückwärts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mHorizontalBackward")]
        public IActionResult GetmHorizontalBackward()
        {
            return new ObjectResult(Program.vacuumGripper.mHorizontalBackward);
        }
        /// <summary>
        /// Motor horizontal rückwärts
        /// </summary>
        /// <param name="mHorizontalBackward_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mHorizontalBackward")]
        public IActionResult SetmHorizontalBackward(bool mHorizontalBackward_value)
        {
            Program.client.Write_node("VG.mHorizontalBackward", mHorizontalBackward_value);
            return new NoContentResult();
        }

        // GET: api/VacuumGripper
        /// <summary>
        /// Motor horizontal vorwärts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mHorizontalForward")]
        public IActionResult GetmHorizontalForward()
        {
            return new ObjectResult(Program.vacuumGripper.mHorizontalForward);
        }
        /// <summary>
        /// Motor horizontal vorwärts
        /// </summary>
        /// <param name="mHorizontalForward_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mHorizontalForward")]
        public IActionResult SetmHorizontalForward(bool mHorizontalForward_value)
        {
            Program.client.Write_node("VG.mHorizontalForward", mHorizontalForward_value);
            return new NoContentResult();
        }

        // GET: api/VacuumGripper
        /// <summary>
        /// Motor drehen im Uhrzeigersinn
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mRotationClockwise")]
        public IActionResult GetmRotationClockwise()
        {
            return new ObjectResult(Program.vacuumGripper.mRotationClockwise);
        }
        /// <summary>
        /// Motor drehen im Uhrzeigersinn
        /// </summary>
        /// <param name="mRotationClockwise_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mRotationClockwise")]
        public IActionResult SetmRotationClockwise(bool mRotationClockwise_value)
        {
            Program.client.Write_node("VG.mRotationClockwise", mRotationClockwise_value);
            return new NoContentResult();
        }

        // GET: api/VacuumGripper
        /// <summary>
        /// Motor drehen gegen Uhrzeigersinn
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mRotationCClockwise")]
        public IActionResult GetmRotationCClockwise()
        {
            return new ObjectResult(Program.vacuumGripper.mRotationCClockwise);
        }
        /// <summary>
        /// Motor drehen gegen Uhrzeigersinn
        /// </summary>
        /// <param name="mRotationCClockwise_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mRotationCClockwise")]
        public IActionResult SetmRotationCClockwise(bool mRotationCClockwise_value)
        {
            Program.client.Write_node("VG.mRotationCClockwise", mRotationCClockwise_value);
            return new NoContentResult();
        }

        // GET: api/VacuumGripper
        /// <summary>
        /// Kompressor
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("compressor")]
        public IActionResult Getcompressor()
        {
            return new ObjectResult(Program.vacuumGripper.compressor);
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
            Program.client.Write_node("VG.compressor", compressor_value);
            return new NoContentResult();
        }

        // GET: api/VacuumGripper
        /// <summary>
        /// Ventil Vakuum
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("valve")]
        public IActionResult Getvalve()
        {
            return new ObjectResult(Program.vacuumGripper.valve);
        }
        /// <summary>
        /// Ventil Vakuum
        /// </summary>
        /// <param name="valve_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("valve")]
        public IActionResult Setvalve(bool valve_value)
        {
            Program.client.Write_node("VG.valve", valve_value);
            return new NoContentResult();
        }

        // GET: api/VacuumGripper
        [HttpGet]
        [Route("state")]
        public IActionResult Getstate()
        {
            return new ObjectResult(Program.vacuumGripper.state);
        }

        // GET: api/VacuumGripper
        [HttpGet]
        [Route("any_motor_running")]
        public IActionResult Getany_motor_running()
        {
            return new ObjectResult(Program.vacuumGripper.any_motor_running);
        }

        // GET: api/VacuumGripper
        [HttpGet]
        [Route("cX")]
        public IActionResult GetcX()
        {
            return new ObjectResult(Program.vacuumGripper.cX);
        }

        // GET: api/VacuumGripper
        [HttpGet]
        [Route("cY")]
        public IActionResult GetcY()
        {
            return new ObjectResult(Program.vacuumGripper.cY);
        }

        // GET: api/VacuumGripper
        [HttpGet]
        [Route("cZ")]
        public IActionResult GetcZ()
        {
            return new ObjectResult(Program.vacuumGripper.cZ);
        }

        // GET: api/VacuumGripper
        [HttpGet]
        [Route("bIdx")]
        public IActionResult GetbIdx()
        {
            return new ObjectResult(Program.vacuumGripper.bIdx);
        }

        [HttpGet]
        [Route("wIdx")]
        public IActionResult GetwIdx()
        {
            return new ObjectResult(Program.vacuumGripper.wIdx);
        }
    }
}