using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestCore.Models
{
    public class TaskConfigurator
    {
        public bool started { get; set; }
        public bool fin { get; set; }
        public bool batch_job_available { get; set; }
        public ExecutionMode mode_active { get; set; }
        public bool is_plc_running { get; set; }
        public bool instant_shutdown { get; set; }
        public int currBatchJobIdx { get; set; }
        public TC_State state { get; set; }

    }
}
