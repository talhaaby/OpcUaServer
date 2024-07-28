using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedAutomation.MachineDemoServer.Models.Jobs
{
    namespace UnifiedAutomation.MachineDemoServer
    {
        public enum JobStatus
        {
            Queued,
            Running,
            Completed,
            Failed
        }

        public class JobInstance
        {
            public string Id { get; set; }
            public JobDefinition Definition { get; set; }
            public JobStatus Status { get; set; }
            // Add other properties as needed

            public JobInstance(JobDefinition definition)
            {
                Definition = definition;
                Id = Guid.NewGuid().ToString();
                Status = JobStatus.Queued;
            }
        }
    }

}
