using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestCore.Models
{
    public class Format
    {
        //[Key]
        //public int Id { get; set; }
        [Required]
        public bool FurEnabled { get; set; }
        [Required]
        public bool SawEnabled { get; set; }
        [Required]
        public bool ColorCheckEnabled { get; set; }
        [Required]
        public bool EjectEnabled { get; set; }
        [Required]
        public int FurDuration { get; set; }
        [Required]
        public int SawDuration { get; set; }
    }
}
