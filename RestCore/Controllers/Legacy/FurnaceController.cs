using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RestCore.Controllers.Legacy
{
    [Produces("application/json")]
    [Route("api/Furnace")]
    [ApiExplorerSettings(GroupName = "v2")]
    public class FurnaceController : Controller
    {
        /*
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="startBurning_value"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("startBurning")]
        public IActionResult startBurning(bool startBurning_value)
        {
            //Program.client.Write_node("FUR.mRaClockwise", mRaClockwise_value);
            return new NoContentResult();
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="stopBurning_value"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("stopBurning")]
        public IActionResult stopBurning(bool stopBurning_value)
        {
            //Program.client.Write_node("FUR.mRaClockwise", mRaClockwise_value);
            return new NoContentResult();
        }


    */





        // GET: api/Furnace
        /// <summary>
        /// Referenzschalter Drehkranz (Position Sauger)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sRaPosSucker")]
        public IActionResult GetsRaPosSucker()
        {
            return new ObjectResult(Program.furnance.sRaPosSucker);
        }
        // GET: api/Furnace
        /// <summary>
        /// Referenzschalter Drehkranz (Position Förderband)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sRaPosConveyorBelt")]
        public IActionResult GetsRaPosConveyorBelt()
        {
            return new ObjectResult(Program.furnance.sRaPosConveyorBelt);
        }
        // GET: api/Furnace
        /// <summary>
        /// Lichtschranke Ende Förderband
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("lbConveyorBelt")]
        public IActionResult GetlbConveyorBelt()
        {
            return new ObjectResult(Program.furnance.lbConveyorBelt);
        }
        // GET: api/Furnace
        /// <summary>
        /// Referenzschalter Drehkranz (Position Säge)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sRaPosSaw")]
        public IActionResult GetsRaPosSaw()
        {
            return new ObjectResult(Program.furnance.sRaPosSaw);
        }
        // GET: api/Furnace
        /// <summary>
        /// Referenzschalter Sauger (Position Drehkranz)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sSuckerPosRa")]
        public IActionResult GetsSuckerPosRa()
        {
            return new ObjectResult(Program.furnance.sSuckerPosRa);
        }
        // GET: api/Furnace
        /// <summary>
        /// Referenzschalter Ofenschieber innen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sFurnanceSliderInside")]
        public IActionResult GetsFurnanceSliderInside()
        {
            return new ObjectResult(Program.furnance.sFurnanceSliderInside);
        }
        // GET: api/Furnace
        /// <summary>
        /// Referenzschalter Ofenschieber außen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sFurnanceSliderOutside")]
        public IActionResult GetsFurnanceSliderOutside()
        {
            return new ObjectResult(Program.furnance.sFurnanceSliderOutside);
        }
        // GET: api/Furnace
        /// <summary>
        /// Referenzschalter Sauger (Position Brennofen)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("sSuckerPosFF")]
        public IActionResult GetsSuckerPosFF()
        {
            return new ObjectResult(Program.furnance.sSuckerPosFF);
        }
        // GET: api/Furnace
        /// <summary>
        /// Lichtschranke Brennofen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("lbFurnance")]
        public IActionResult GetlbFurnance()
        {
            return new ObjectResult(Program.furnance.lbFurnance);
        }

        // GET: api/Furnace
        /// <summary>
        /// Motor Drehkranz im Uhrzeigersinn
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mRaClockwise")]
        public IActionResult GetmRaClockwise()
        {
            return new ObjectResult(Program.furnance.mRaClockwise);
        }
        /// <summary>
        /// Motor Drehkranz im Uhrzeigersinn
        /// </summary>
        /// <param name="mRaClockwise_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mRaClockwise")]
        public IActionResult SetmRaClockwise(bool mRaClockwise_value)
        {
            Program.client.Write_node("FUR.mRaClockwise", mRaClockwise_value);
            return new NoContentResult();
        }
        // GET: api/Furnace
        /// <summary>
        /// Motor Drehkranz gegen Uhrzeigersinn
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mRaCClockwise")]
        public IActionResult GetmRaCClockwise()
        {
            return new ObjectResult(Program.furnance.mRaCClockwise);
        }
        /// <summary>
        /// Motor Drehkranz gegen Uhrzeigersinn
        /// </summary>
        /// <param name="mRaCClockwise_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mRaCClockwise")]
        public IActionResult SetmRaCClockwise(bool mRaCClockwise_value)
        {
            Program.client.Write_node("FUR.mRaCClockwise", mRaCClockwise_value);
            return new NoContentResult();
        }
        // GET: api/Furnace
        /// <summary>
        /// Motor Förderband vorwärts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mConveyorBeltForward")]
        public IActionResult GetmConveyorBeltForward()
        {
            return new ObjectResult(Program.furnance.mConveyorBeltForward);
        }
        /// <summary>
        /// Motor Förderband vorwärts
        /// </summary>
        /// <param name="mConveyorBeltForward_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mConveyorBeltForward")]
        public IActionResult SetmConveyorBeltForward(bool mConveyorBeltForward_value)
        {
            Program.client.Write_node("FUR.mConveyorBeltForward", mConveyorBeltForward_value);
            return new NoContentResult();
        }
        // GET: api/Furnace
        /// <summary>
        /// Motor Säge
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mSaw")]
        public IActionResult GetmSaw()
        {
            return new ObjectResult(Program.furnance.mSaw);
        }
        /// <summary>
        /// Motor Säge
        /// </summary>
        /// <param name="mSaw_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mSaw")]
        public IActionResult SetmSaw(bool mSaw_value)
        {
            Program.client.Write_node("FUR.mSaw", mSaw_value);
            return new NoContentResult();
        }
        // GET: api/Furnace
        /// <summary>
        /// Motor Ofenschieber einfahren
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mFurnanceSliderIn")]
        public IActionResult GetmFurnanceSliderIn()
        {
            return new ObjectResult(Program.furnance.mFurnanceSliderIn);
        }
        /// <summary>
        /// Motor Ofenschieber einfahren
        /// </summary>
        /// <param name="mFurnanceSliderIn_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mFurnanceSliderIn")]
        public IActionResult SetmFurnanceSliderIn(bool mFurnanceSliderIn_value)
        {
            Program.client.Write_node("FUR.mFurnanceSliderIn", mFurnanceSliderIn_value);
            return new NoContentResult();
        }
        // GET: api/Furnace
        /// <summary>
        /// Motor Ofenschieber ausfahren
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mFurnanceSliderOut")]
        public IActionResult GetmFurnanceSliderOut()
        {
            return new ObjectResult(Program.furnance.mFurnanceSliderOut);
        }
        /// <summary>
        /// Motor Ofenschieber ausfahren
        /// </summary>
        /// <param name="mFurnanceSliderOut_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mFurnanceSliderOut")]
        public IActionResult SetmFurnanceSliderOut(bool mFurnanceSliderOut_value)
        {
            Program.client.Write_node("FUR.mFurnanceSliderOut", mFurnanceSliderOut_value);
            return new NoContentResult();
        }
        // GET: api/Furnace
        /// <summary>
        /// Motor Sauger zum Ofen
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mSuckertoFurnance")]
        public IActionResult GetmSuckertoFurnance()
        {
            return new ObjectResult(Program.furnance.mSuckertoFurnance);
        }
        /// <summary>
        /// Motor Sauger zum Ofen
        /// </summary>
        /// <param name="mSuckertoFurnance_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mSuckertoFurnance")]
        public IActionResult SetmSuckertoFurnance(bool mSuckertoFurnance_value)
        {
            Program.client.Write_node("FUR.mSuckertoFurnance", mSuckertoFurnance_value);
            return new NoContentResult();
        }
        // GET: api/Furnace
        /// <summary>
        /// Motor Sauger zum Drehkranz
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("mSuckertoRa")]
        public IActionResult GetmSuckertoRa()
        {
            return new ObjectResult(Program.furnance.mSuckertoRa);
        }
        /// <summary>
        /// Motor Sauger zum Drehkranz
        /// </summary>
        /// <param name="mSuckertoRa_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("mSuckertoRa")]
        public IActionResult SetmSuckertoRa(bool mSuckertoRa_value)
        {
            Program.client.Write_node("FUR.mSuckertoRa", mSuckertoRa_value);
            return new NoContentResult();
        }
        // GET: api/Furnace
        /// <summary>
        /// Kompressor
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("compressor")]
        public IActionResult Getcompressor()
        {
            return new ObjectResult(Program.furnance.compressor);
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
            Program.client.Write_node("FUR.compressor", compressor_value);
            return new NoContentResult();
        }
        // GET: api/Furnace
        /// <summary>
        /// Ventil Vakuum
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("valveVacuum")]
        public IActionResult GetvalveVacuum()
        {
            return new ObjectResult(Program.furnance.valveVacuum);
        }
        /// <summary>
        /// Ventil Vakuum
        /// </summary>
        /// <param name="valveVacuum_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("valveVacuum")]
        public IActionResult SetvalveVacuum(bool valveVacuum_value)
        {
            Program.client.Write_node("FUR.valveVacuum", valveVacuum_value);
            return new NoContentResult();
        }
        // GET: api/Furnace
        /// <summary>
        /// Ventil Senken
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("valveLowering")]
        public IActionResult GetvalveLowering()
        {
            return new ObjectResult(Program.furnance.valveLowering);
        }
        /// <summary>
        /// Ventil Senken
        /// </summary>
        /// <param name="valveLowering_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("valveLowering")]
        public IActionResult SetvalveLowering(bool valveLowering_value)
        {
            Program.client.Write_node("FUR.valveLowering", valveLowering_value);
            return new NoContentResult();
        }
        // GET: api/Furnace
        /// <summary>
        /// Ventil Ofentür
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("valveFurnanceDoor")]
        public IActionResult GetvalveFurnanceDoor()
        {
            return new ObjectResult(Program.furnance.valveFurnanceDoor);
        }
        /// <summary>
        /// Ventil Ofentür
        /// </summary>
        /// <param name="valveFurnanceDoor_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("valveFurnanceDoor")]
        public IActionResult SetvalveFurnanceDoor(bool valveFurnanceDoor_value)
        {
            Program.client.Write_node("FUR.valveFurnanceDoor", valveFurnanceDoor_value);
            return new NoContentResult();
        }
        // GET: api/Furnace
        /// <summary>
        /// Ventil Schieber
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("valveSlider")]
        public IActionResult GetvalveSlider()
        {
            return new ObjectResult(Program.furnance.valveSlider);
        }
        /// <summary>
        /// Ventil Schieber
        /// </summary>
        /// <param name="valveSlider_value">true = ein, false = aus</param>
        /// <returns></returns>
        [HttpPut]
        [Route("valveSlider")]
        public IActionResult SetvalveSlider(bool valveSlider_value)
        {
            Program.client.Write_node("FUR.valveSlider", valveSlider_value);
            return new NoContentResult();
        }
        // GET: api/Furnace
        [HttpGet]
        [Route("any_motor_running")]
        public IActionResult Getany_motor_running()
        {
            return new ObjectResult(Program.furnance.any_motor_running);
        }
        // GET: api/Furnace
        [HttpGet]
        [Route("state")]
        public IActionResult Getstate()
        {
            return new ObjectResult(Program.furnance.state);
        }
        // GET: api/Furnace
        [HttpGet]
        [Route("bIdxBurner")]
        public IActionResult GetbIdxBurner()
        {
            return new ObjectResult(Program.furnance.bIdxBurner);
        }
        // GET: api/Furnace
        [HttpGet]
        [Route("bIdxSuccer")]
        public IActionResult GetbIdxSuccer()
        {
            return new ObjectResult(Program.furnance.bIdxSuccer);
        }
        // GET: api/Furnace
        [HttpGet]
        [Route("bIdxSaw")]
        public IActionResult GetbIdxSaw()
        {
            return new ObjectResult(Program.furnance.bIdxSaw);
        }
        // GET: api/Furnace
        [HttpGet]
        [Route("wIdxBurner")]
        public IActionResult GetwIdxBurner()
        {
            return new ObjectResult(Program.furnance.wIdxBurner);
        }
        // GET: api/Furnace
        [HttpGet]
        [Route("wIdxSuccer")]
        public IActionResult GetwIdxSuccer()
        {
            return new ObjectResult(Program.furnance.wIdxSuccer);
        }
        // GET: api/Furnace
        [HttpGet]
        [Route("wIdxSaw")]
        public IActionResult GetwIdxSaw()
        {
            return new ObjectResult(Program.furnance.wIdxSaw);
        }
    }
}