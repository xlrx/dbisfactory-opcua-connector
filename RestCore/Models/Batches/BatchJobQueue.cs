using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestCore.Models
{
    public class BatchJobQueue
    {
        //[Key]
        //public int Id { get; set; }
        public int AmountBatchJobs { get; set; }
        [Required]
        public List<BatchJob> BatchJobs { get; set; }
        public int NextFreePos { get; set; }
        public bool Full { get; set; }
    }
    
}
