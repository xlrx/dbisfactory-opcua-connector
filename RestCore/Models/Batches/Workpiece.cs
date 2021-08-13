using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestCore.Models
{
    public class Workpiece
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Color { get; set; }
        public int LinePos { get; set; }
        public int SortingLinePos { get; set; }
        [Required]
        public W_State State { get; set; }
        [Required]
        public RackPos StartPos { get; set; }
        [Required]
        public RackPos EndPos { get; set; }
        [Required]
        public List<Format> Formate { get; set; }
    }
}
