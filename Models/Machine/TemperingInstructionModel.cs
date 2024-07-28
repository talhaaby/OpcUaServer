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
using System.IO;
using UnifiedAutomation.UaBase;

namespace UnifiedAutomation.MachineDemoServer
{
    public partial class TemperingInstructionModel
    {
        public TemperingInstructionModel(TemperingInstructionModel template = null) : this(template, dummy: null)
        {
            var name = Path.GetTempFileName();
            Plan.FileOnDisk = new System.IO.FileInfo(name);
        }

        public StatusCode LoadPlan()
        {
            var file = Plan.FileOnDisk;

            file.Refresh();
            if (!file.Exists || file.Length == 0)
            {
                return StatusCodes.Bad;
            }

            //    Plan.
            try
            {
                var loadedPlan = new GlassTemperingPlan();

                using (var stream = file.OpenRead())
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        var parts = line.Split(':');

                        switch (parts[0].Trim())
                        {
                            case nameof(GlassTemperingPlan.ProcessTime):
                                loadedPlan.ProcessTime = Convert.ToDouble(parts[1]);
                                break;
                            case nameof(GlassTemperingPlan.MinTemperature):
                                loadedPlan.MinTemperature = Convert.ToDouble(parts[1]);
                                break;
                            case nameof(GlassTemperingPlan.MaxTemperature):
                                loadedPlan.MaxTemperature = Convert.ToDouble(parts[1]);
                                break;
                            case nameof(GlassTemperingPlan.Name):
                                loadedPlan.Name = parts[1];
                                break;
                        }
                    }
                }

                LoadedPlan = loadedPlan;
            }
            catch
            {
                return StatusCodes.Bad;
            }

            return StatusCodes.Good;
        }
    }
}
