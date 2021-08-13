using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestCore.Models
{
    public class HighRack
    {
        public bool sHorizontal { get; set; }
        public bool lbIn { get; set; }
        public bool lbOut { get; set; }
        public bool sVertical { get; set; }
        public bool sCFront { get; set; }
        public bool sCBack { get; set; }
        public bool encHorizontal { get; set; }
        public bool encVertical { get; set; }
        public bool mCbForward { get; set; }
        public bool mCbBackward { get; set; }
        public bool mHorizontalLeft{ get; set; }
        public bool mHorizontalRight { get; set; }
        public bool mVerticalDown{ get; set; }
        public bool mVerticalUp { get; set; }
        public bool mCForward{ get; set; }
        public bool mCBackward { get; set; }
        public HR_State state { get; set; }
        public bool any_motor_running { get; set; }
        public int cX { get; set; }
        public int cY { get; set; }
        public int bIdx { get; set; }
        public int wIdx { get; set; }
    }
}
