using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnifiedAutomation.MachineDemoServer.Models.Jobs.UnifiedAutomation.MachineDemoServer;

namespace UnifiedAutomation.MachineDemoServer.Models.Jobs
{
    public class JobManager
    {
        private List<JobDefinition> jobDefinitions;
        private List<JobInstance> jobInstances;

        public JobManager(string xmlFilePath)
        {
            jobDefinitions = new List<JobDefinition>();
            jobInstances = new List<JobInstance>();

            LoadJobDefinitions(xmlFilePath);
        }

        private void LoadJobDefinitions(string xmlFilePath)
        {
            XDocument doc = XDocument.Load(xmlFilePath);
            foreach (var jobElement in doc.Root.Elements("Job"))
            {
                jobDefinitions.Add(JobDefinition.FromXml(jobElement));
            }
        }

        public void CreateJob(string jobDefinitionId)
        {
            var definition = jobDefinitions.FirstOrDefault(d => d.Id == jobDefinitionId);
            if (definition != null)
            {
                var instance = new JobInstance(definition);
                jobInstances.Add(instance);
                // Add logic to start the job if needed
            }
        }

        // Other methods for starting jobs, managing job queue, etc.
    }
}
