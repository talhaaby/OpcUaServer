using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UnifiedAutomation.MachineDemoServer
{
    internal class JobData
    {

        public class JobOrderContainer
        {
            public JobOrder JobOrder { get; set; }
        }

        public class JobOrder
        {
            public string ID { get; set; }
            public string Description { get; set; }
            public List<JobOrderParameter> JobOrderParameters { get; set; }
            public MaterialRequirement MaterialRequirement { get; set; }
        }

        public class JobOrderParameter
        {
            public string ID { get; set; }
            public string Value { get; set; }
            public List<Subparameter> Subparameters { get; set; }
        }

        public class Subparameter
        {
            public string ID { get; set; }
            public string uom { get; set; }
            public string value { get; set; }
        }

        public class MaterialRequirement
        {
            public string MaterialDefinitionID { get; set; }
            public int Quantity { get; set; }
        }
        public class JobControl
        {
            public void ProcessJobOrder(string fileName)
            {
                JobOrder job = JsonSerializer.Deserialize<JobOrder>(fileName); // Hypothetical method

                Console.WriteLine($"Job ID: {job.ID}");
                Console.WriteLine($"Description: {job.Description}");
            }
        }



    }
}
