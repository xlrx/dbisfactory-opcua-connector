using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestCore.Models
{
    public class Furnance
    {
        public bool sRaPosSucker { get; set; }
        public bool sRaPosConveyorBelt { get; set; }
        public bool lbConveyorBelt { get; set; }
        public bool sRaPosSaw { get; set; }
        public bool sSuckerPosRa { get; set; }
        public bool sFurnanceSliderInside { get; set; }
        public bool sFurnanceSliderOutside { get; set; }
        public bool sSuckerPosFF { get; set; }
        public bool lbFurnance { get; set; }

        public bool mRaClockwise { get; set; }
        public bool mRaCClockwise { get; set; }
        public bool mConveyorBeltForward { get; set; }
        public bool mSaw { get; set; }
        public bool mFurnanceSliderIn { get; set; }
        public bool mFurnanceSliderOut { get; set; }
        public bool mSuckertoFurnance { get; set; }
        public bool mSuckertoRa { get; set; }
        public bool compressor { get; set; }
        public bool valveVacuum { get; set; }
        public bool valveLowering { get; set; }
        public bool valveFurnanceDoor { get; set; }
        public bool valveSlider { get; set; }
        public bool any_motor_running { get; set; }
        public FUR_State state { get; set; }

        public int bIdxBurner { get; set; }
        public int bIdxSuccer { get; set; }
        public int bIdxSaw { get; set; }
        public int wIdxBurner { get; set; }
        public int wIdxSuccer { get; set; }
        public int wIdxSaw { get; set; }

    }
}
