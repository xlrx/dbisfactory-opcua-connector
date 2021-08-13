using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestCore.Models
{
    public class SortingLine
    {
        public int colorSensor { get; set; }
        public bool compressor { get; set; }
        public bool iFeeler { get; set; }
        public bool lbAfterColorSensor { get; set; }
        public bool lbEntry { get; set; }
        public bool lbBlue { get; set; }
        public bool lbRed { get; set; }
        public bool lbWhite { get; set; }
        public bool mConveyorBelt { get; set; }
        public bool valveFirstEject { get; set; }
        public bool valveSecEject { get; set; }
        public bool valveThirdEject { get; set; }
        public bool any_motor_running { get; set; }
        public SL_State state { get; set; }
        public int bIdx { get; set; }
        public int wIdx { get; set; }
    }
}