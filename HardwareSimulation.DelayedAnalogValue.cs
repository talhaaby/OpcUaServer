using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedAutomation.MachineDemoServer
{
    public partial class HardwareSimulation
    {
        private class DelayedAnalogValue
        {
            private readonly List<double> m_values;
            private readonly int m_delay;

            public DelayedAnalogValue(double initialValue, int delay)
            {
                m_delay = delay;
                m_values = new List<double>(delay + 1);

                for (int i = 0; i < delay; i++)
                {
                    m_values.Add(initialValue);
                }
            }

            public void Increment(double delta)
            {
                var last = m_values.Last();
                m_values.Add(last + delta);

            }

            public double PopCurrent()
            {
                var mean = GetWeightedMean();

                if (m_values.Count > m_delay)
                {
                    m_values.RemoveAt(0);
                }

                return mean;
            }

            private double GetWeightedMean()
            {
                double total = 0;
                int weight = 1;
                int totalWeight = 0;

                foreach (var value in m_values.Reverse<double>())
                {
                    totalWeight += weight;
                    total += value * weight;
                    weight++;
                }

                return total / totalWeight;
            }
        }
    }
}
