using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestCore.Models
{
    public class VacuumGripper
    {
        public bool sVertical { get; set; }
        public bool sHorizontal { get; set; }
        public bool sRotation { get; set; }
        public bool encVertical { get; set; }
        public bool encHorizontal { get; set; }
        public bool encRotation { get; set; }
        public bool mVerticalUp { get; set; }
        public bool mVerticalDown { get; set; }
        public bool mHorizontalBackward { get; set; }
        public bool mHorizontalForward { get; set; }
        public bool mRotationClockwise { get; set; }
        public bool mRotationCClockwise { get; set; }
        public bool compressor { get; set; }
        public bool valve { get; set; }
        public VG_State state { get; set; }
        public bool any_motor_running { get; set; }
        public int cX { get; set; }
        public int cY { get; set; }
        public int cZ { get; set; }
        public int bIdx { get; set; }
        public int wIdx { get; set; }

    }
}
