/******************************************************************************
** Copyright (c) 2006-2023 Unified Automation GmbH All rights reserved.
**
** Software License Agreement ("SLA") Version 2.8
**
** Unless explicitly acquired and licensed from Licensor under another
** license, the contents of this file are subject to the Software License
** Agreement ("SLA") Version 2.8, or subsequent versions
** as allowed by the SLA, and You may not copy or use this file in either
** source code or executable form, except in compliance with the terms and
** conditions of the SLA.
**
** All software distributed under the SLA is provided strictly on an
** "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED,
** AND LICENSOR HEREBY DISCLAIMS ALL SUCH WARRANTIES, INCLUDING WITHOUT
** LIMITATION, ANY WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
** PURPOSE, QUIET ENJOYMENT, OR NON-INFRINGEMENT. See the SLA for specific
** language governing rights and limitations under the SLA.
**
** Project: .NET based OPC UA Client Server SDK
**
** Description: OPC Unified Architecture Software Development Kit.
**
** The complete license agreement can be found here:
** http://unifiedautomation.com/License/SLA/2.8/
******************************************************************************/

using System;
using System.Threading;
using UnifiedAutomation.UaBase;

namespace UnifiedAutomation.MachineDemoServer
{
    //! [Notifier]
    [UaNotifier]
    public partial class HeaterModel
    {
    //! [Notifier]
        public HeaterModel(HeaterModel template) : this(template, null)
        {
            //! [PropertyChangeRequested]
            TemperatureSetpoint.PropertyChangeRequested += (o, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(TemperatureSetpoint.Value):
                        var value = (double)e.Value;
                        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] TemperatureSetpoint change request to {value}");

                        if (double.IsNaN(value) || value > 500.0)
                        {
                            e.StatusCode = StatusCodes.BadOutOfRange;
                        }
                        else if (value < 0.0)
                        {
                            e.StatusCode = StatusCodes.GoodClamped;
                            e.Value = 0.0;
                        }
                        break;
                }
            };
            //! [PropertyChangeRequested]

            //! [PropertyChanged]
            TemperatureSetpoint.PropertyChanged += (o, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(TemperatureSetpoint.Value):
                        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] TemperatureSetpoint changed to {TemperatureSetpoint.Value}");
                        break;
                }
            };
            //! [PropertyChanged]
        }

        public void SetTemperature(double temperature)
        {
            //! [SetTemperature]
            Temperature.Value = temperature;
            //! [SetTemperature]
            CheckTemperatureCondition();
        }

        //! [SetTemperatureLimits]
        public void SetTemperatureAlarmLimits(double min, double max)
        {
            using (TemperatureCondition.MergeTransitions())
            {
                if (!TemperatureCondition.EnabledState.Id)
                {
                    TemperatureCondition.Enable();
                }
                TemperatureCondition.LowLimit = min;
                TemperatureCondition.HighLimit = max;
                CheckTemperatureCondition();
            }
        }
        //! [SetTemperatureLimits]

        public void UnsetTemperatureAlarmLimits()
        {
            using (TemperatureCondition.MergeTransitions())
            {
                TemperatureCondition.LowLimit = null;
                TemperatureCondition.HighLimit = null;
                CheckTemperatureCondition();
            }
        }

        //! [CheckTemperature]
        private void CheckTemperatureCondition()
        {
            TemperatureCondition.Evaluate(Temperature.Value);
        }
        //! [CheckTemperature]
    }
}
