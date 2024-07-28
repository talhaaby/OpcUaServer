using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnifiedAutomation.MachineDemoServer
{
    public partial class HardwareSimulation
    {
        private bool m_present;

        private Stopwatch m_delayEnter = new Stopwatch();
        private Stopwatch m_delayLeave = new Stopwatch();

        private DelayedAnalogValue m_temperature = new DelayedAnalogValue(20.0, 7);

        public Dictionary<string, int> ProcessIO(Dictionary<string, int> inputs)
        {
            bool enterRequest = false;
            bool leaveRequest = false;
            double tempSetpoint = 310.0;

            if (inputs != null)
            {
                enterRequest = Convert.ToBoolean(inputs["MAIN.bMtrlEnterReq"]);
                leaveRequest = Convert.ToBoolean(inputs["MAIN.bMtrlLeaveReq"]);
                tempSetpoint = Convert.ToDouble(inputs["MAIN.stEB01.nSetTemperature"]) / 100.0;
            }

            // Enter delay
            if (enterRequest
                && !m_present
                && !leaveRequest
                && !m_delayEnter.IsRunning)
            {
                m_delayEnter.Restart();
            }
            
            if (!enterRequest || m_present)
            {
                m_delayEnter.Reset();
            }

            if (m_delayEnter.Elapsed > TimeSpan.FromSeconds(2))
            {
                m_present = true;
            }

            // Leave delay
            if (leaveRequest
                && m_present
                && !enterRequest
                && !m_delayLeave.IsRunning)
            {
                m_delayLeave.Restart();
            }

            if (!leaveRequest || !m_present)
            {
                m_delayLeave.Reset();
            }

            if (m_delayLeave.Elapsed > TimeSpan.FromSeconds(2))
            {
                m_present = false;
            }

            // temperature
            var actualTemperature = m_temperature.PopCurrent();
            var delta = (tempSetpoint - actualTemperature) / 10.0;
            m_temperature.Increment(delta);

            return new Dictionary<string, int>
            {
                ["MAIN.bMtrlPresent"] = Convert.ToInt32(m_present),
                ["MAIN.stEB01.nActTemperature"] = Convert.ToInt32(actualTemperature * 100)
            };
        }
    }
}
