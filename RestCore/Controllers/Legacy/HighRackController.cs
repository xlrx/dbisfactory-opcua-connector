using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestCore.Controllers.Legacy
{
    [Produces("application/json")]
    [Route("api/HighRack")]
    [ApiExplorerSettings(GroupName = "v2")]
    public class HighRackController : Controller
    {
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rackPos"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("drivetoRack")]
        public IActionResult drivetoRack(RackPos rackPos)
        {
            //Program.client.Write_node("HR.mCbBackward", mCbBackward_value);
            return new NoContentResult();
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="rackPos"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("getWorkpiece")]
        public IActionResult getWorkpiece(RackPos rackPos)
        {
            //Program.client.Write_node("HR.mCbBackward", mCbBackward_value);
            return new NoContentResult();
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("deliverytoconveyorbelt")]
        public IActionResult deliverytoconveyorbelt()
        {
            //Program.client.Write_node("HR.mCbBackward", mCbBackward_value);
            return new NoContentResult();
        }
        */

        // GET: api/HighRack
        /// <summary>
        /// Referenztaster horizontal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sHorizontal")]
        public IActionResult GetsHorizontal()
        {
            return new ObjectResult(Program.highRack.sHorizontal);
        }

        // GET: api/HighRack
        /// <summary>
        /// Lichtschranke innen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("lbIn")]
        public IActionResult GetlbIn()
        {
            return new ObjectResult(Program.highRack.lbIn);
        }

        // GET: api/HighRack
        /// <summary>
        /// Lichtschranke außen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("lbOut")]
        public IActionResult GetlbOut()
        {
            return new ObjectResult(Program.highRack.lbOut);
        }

        // GET: api/HighRack
        /// <summary>
        /// Referenztaster vertikal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sVertical")]
        public IActionResult GetsVertical()
        {
            return new ObjectResult(Program.highRack.sVertical);
        }

        // GET: api/HighRack
        /// <summary>
        /// Spursensor(Signal 1, unten)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sCFront")]
        public IActionResult GetsCFront()
        {
            return new ObjectResult(Program.highRack.sCFront);
        }

        // GET: api/HighRack
        /// <summary>
        /// Spursensor (Signal 2, oben)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sCBack")]
        public IActionResult GetsCBack()
        {
            return new ObjectResult(Program.highRack.sCBack);
        }

        // GET: api/HighRack
        /// <summary>
        /// Encoder horizontal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("encHorizontal")]
        public IActionResult GetencHorizontal()
        {
            return new ObjectResult(Program.highRack.encHorizontal);
        }

        // GET: api/HighRack
        /// <summary>
        /// Encoder vertikal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("encVertical")]
        public IActionResult GetencVertical()
        {
            return new ObjectResult(Program.highRack.encVertical);
        }

        // GET: api/HighRack
        /// <summary>
        /// Motor Förderband vorwärts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mCbForward")]
        public IActionResult GetmCbForward()
        {
            return new ObjectResult(Program.highRack.mCbForward);
        }
        /// <summary>
        /// Motor Förderband vorwärts
        /// </summary>
        /// <param name="mCbForward_value">>true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mCbForward")]
        public IActionResult SetmCbForward(bool mCbForward_value)
        {
            Program.client.Write_node("HR.mCbForward", mCbForward_value);
            return new NoContentResult();
        }

        // GET: api/HighRack
        /// <summary>
        /// Motor Förderband rückwarts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mCbBackward")]
        public IActionResult GetmCbBackward()
        {
            return new ObjectResult(Program.highRack.mCbBackward);
        }
        /// <summary>
        /// Motor Förderband rückwarts
        /// </summary>
        /// <param name="mCbBackward_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mCbBackward")]
        public IActionResult SetmCbBackward(bool mCbBackward_value)
        {
            Program.client.Write_node("HR.mCbBackward", mCbBackward_value);
            return new NoContentResult();
        }

        // GET: api/HighRack
        /// <summary>
        /// Motor horizontal zum Regal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mHorizontalLeft")]
        public IActionResult GetmHorizontalLeft()
        {
            return new ObjectResult(Program.highRack.mHorizontalLeft);
        }
        /// <summary>
        /// Motor horizontal zum Regal
        /// </summary>
        /// <param name="mHorizontalLeft_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mHorizontalLeft")]
        public IActionResult SetmHorizontalLeft(bool mHorizontalLeft_value)
        {
            Program.client.Write_node("HR.mHorizontalLeft", mHorizontalLeft_value);
            return new NoContentResult();
        }

        // GET: api/HighRack
        /// <summary>
        /// Motor horizontal zum Förderband
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mHorizontalRight")]
        public IActionResult GetmHorizontalRight()
        {
            return new ObjectResult(Program.highRack.mHorizontalRight);
        }
        /// <summary>
        /// Motor horizontal zum Förderband
        /// </summary>
        /// <param name="mHorizontalRight_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mHorizontalRight")]
        public IActionResult SetmHorizontalRight(bool mHorizontalRight_value)
        {
            Program.client.Write_node("HR.mHorizontalRight", mHorizontalRight_value);
            return new NoContentResult();
        }

        // GET: api/HighRack
        /// <summary>
        /// Motor vertikal runter
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mVerticalDown")]
        public IActionResult GetmVerticalDown()
        {
            return new ObjectResult(Program.highRack.mVerticalDown);
        }
        /// <summary>
        /// Motor vertikal runter
        /// </summary>
        /// <param name="mVerticalDown_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mVerticalDown")]
        public IActionResult SetmVerticalDown(bool mVerticalDown_value)
        {
            Program.client.Write_node("HR.mVerticalDown", mVerticalDown_value);
            return new NoContentResult();
        }

        // GET: api/HighRack
        /// <summary>
        /// Motor vertikal hoch
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mVerticalUp")]
        public IActionResult GetmVerticalUp()
        {
            return new ObjectResult(Program.highRack.mVerticalUp);
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
            Program.client.Write_node("HR.mVerticalUp", mVerticalUp_value);
            return new NoContentResult();
        }

        // GET: api/HighRack
        /// <summary>
        /// Motor Ausleger vorwärts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mCForward")]
        public IActionResult GetmCForward()
        {
            return new ObjectResult(Program.highRack.mCForward);
        }
        /// <summary>
        /// Motor Ausleger vorwärts
        /// </summary>
        /// <param name="mCForward_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mCForward")]
        public IActionResult SetmCForward(bool mCForward_value)
        {
            Program.client.Write_node("HR.mCForward", mCForward_value);
            return new NoContentResult();
        }

        // GET: api/HighRack
        /// <summary>
        /// Motor Ausleger rückwärts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mCBackward")]
        public IActionResult GetmCBackward()
        {
            return new ObjectResult(Program.highRack.mCBackward);
        }
        /// <summary>
        /// Motor Ausleger rückwärts
        /// </summary>
        /// <param name="mCBackward_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mCBackward")]
        public IActionResult SetmCBackward(bool mCBackward_value)
        {
            Program.client.Write_node("HR.mCBackward", mCBackward_value);
            return new NoContentResult();
        }

        // GET: api/HighRack
        [HttpGet]
        [Route("state")]
        public IActionResult Getstate()
        {
            return new ObjectResult(Program.highRack.state);
        }

        // GET: api/HighRack
        [HttpGet]
        [Route("any_motor_running")]
        public IActionResult Getany_motor_running()
        {
            return new ObjectResult(Program.highRack.any_motor_running);
        }

        // GET: api/HighRack
        [HttpGet]
        [Route("cX")]
        public IActionResult GetcX()
        {
            return new ObjectResult(Program.highRack.cX);
        }

        // GET: api/HighRack
        [HttpGet]
        [Route("cY")]
        public IActionResult GetcY()
        {
            return new ObjectResult(Program.highRack.cY);
        }

        // GET: api/HighRack
        [HttpGet]
        [Route("bIdx")]
        public IActionResult GetbIdx()
        {
            return new ObjectResult(Program.highRack.bIdx);
        }

        // GET: api/HighRack
        [HttpGet]
        [Route("wIdx")]
        public IActionResult GetwIdx()
        {
            return new ObjectResult(Program.highRack.wIdx);
        }
    }
}
