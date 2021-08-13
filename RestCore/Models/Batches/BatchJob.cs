using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestCore.Models
{
     public class BatchJob
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int BatchSize { get; set; }
        [Required]
        public List<Workpiece> Workpieces { get; set; }
        public int NextFreeWPos { get; set; }
        public int AmountFinishedW { get; set; }
        public int NextNotProcessedIdx { get; set; }
        [Required]
        public ExecutionMode Mode { get; set; }
        public BatchJobState state { get; set; }
    }
}
