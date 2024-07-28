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

using OpcUa.Glass;
using OpcUa.Machinery;
using System;
using System.Collections.Generic;
using System.Linq;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;

namespace UnifiedAutomation.MachineDemoServer
{
    [UaNotifier]
    //! [Auto node version]
    [UaAutoNodeVersion]
    public partial class GlassTemperingMachineModel : IGlassTemperingMachineMethods
    {
    //! [Auto node version]
        public TemperingJobModel ActiveJob
        {
            get => m_activeJob;
            private set => SetField(ref m_activeJob, value);
        }
        private TemperingJobModel m_activeJob;

        public bool IsMaterialInPlace { get; set; }

        public bool MaterialEnterRequest { get; set; }
        public bool MaterialLeaveRequest { get; set; }

        //! [Declare event]
        [UaEvent]
        public event EventHandler<MaterialReceivedEventModel> MaterialReceivedEvent;
        //! [Declare event]

        [UaEvent]
        public event EventHandler<MaterialExitEventModel> MaterialExitEvent;

        //! [Declare reference]
        [UaReference(ReferenceTypeId = ReferenceTypes.HasActiveJob, NamespaceUri = Namespaces.MachineDemoServer)]
        public NodeId ActiveJobId
        {
            get => m_activeJobId;
            private set => SetField(ref m_activeJobId, value);
        }
        private NodeId m_activeJobId;
        //! [Declare reference]

        public GlassTemperingMachineModel(GlassTemperingMachineModel template) : this(template, default(DummyArgument))
        {
            Production.ProductionPlan.CollectionChanged += (o, e) =>
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (TemperingJobModel item in e.NewItems)
                    {
                        item.State.Switched += State_Switched;
                        item.State.InitializingState.Switched += InitializingState_Switched;
                    }
                }
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    foreach (TemperingJobModel item in e.OldItems)
                    {
                        item.State.Switched -= State_Switched;
                        item.State.InitializingState.Switched -= InitializingState_Switched;
                    }
                }
            };

            MachineryBuildingBlocks.MachineryOperationMode.Switched += (o, e) =>
            {
                IsProcessExecutable = e.NewState != MachineryOperationModeStateMachineModel.State.Processing;
                IsSetupExecutable = e.NewState != MachineryOperationModeStateMachineModel.State.Setup;

                if (e.NewState == MachineryOperationModeStateMachineModel.State.Processing)
                {
                    TryStartNextJob();
                }
            };

            MachineryBuildingBlocks.MachineryOperationMode.SwitchToState(MachineryOperationModeStateMachineModel.State.Setup);
            MachineryBuildingBlocks.MachineryItemState.SwitchToState(MachineryItemState_StateMachineModel.State.NotExecuting);

            PropertyChanged += (o, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(ActiveJob):
                        ActiveJobId = ActiveJob?.JobNodeId;
                        break;
                }
            };
        }

        /// <summary>
        /// The method takes the input values, process them, and returns the output
        /// values. In the example the values originate from a simulation class, but they
        /// could come from a real hardware.
        /// </summary>
        /// <param name="inputs">The inputs of the machine.</param>
        /// <returns>The outputs of the machine.</returns>
        public Dictionary<string, int> ProcessIO(Dictionary<string, int> inputs)
        {
            var isMaterialInPlace = Convert.ToBoolean(inputs["MAIN.bMtrlPresent"]);

            // Change in the material presence 
            if (isMaterialInPlace != IsMaterialInPlace)
            {
                IsMaterialInPlace = isMaterialInPlace;

                if (IsMaterialInPlace)
                {
                    // Trigger material received event
                    //! [Raise event]
                    MaterialReceivedEvent?.Invoke(this, new MaterialReceivedEventModel
                    {
                        Message = "Material entered.",
                        JobdIdentifier = ActiveJob?.Identifier,
                        Identifier = Identification.SerialNumber,
                        Location = Identification.Location
                    });
                    //! [Raise event]
                    MaterialEnterRequest = false;

                    // Start the job
                    ActiveJob.Start();
                }
                else
                {
                    // Trigger material exit event
                    MaterialExitEvent?.Invoke(this, new MaterialExitEventModel
                    {
                        Message = "Material leaved.",
                        JobdIdentifier = ActiveJob?.Identifier,
                        Identifier = Identification.SerialNumber,
                        Location = Identification.Location
                    });
                    MaterialLeaveRequest = false;

                    ActiveJob = null;
                    TryStartNextJob();
                }
            }

            // Check if the job is due
            if (ActiveJob != null)
            {
                ActiveJob.CheckForEnd();
            }

            // Temperature
            Components.Heater.SetTemperature(Convert.ToDouble(inputs["MAIN.stEB01.nActTemperature"]) / 100.0);

            return new Dictionary<string, int>
            {
                ["MAIN.bMtrlEnterReq"] = Convert.ToInt32(MaterialEnterRequest),
                ["MAIN.bMtrlLeaveReq"] = Convert.ToInt32(MaterialLeaveRequest),
                ["MAIN.stEB01.nSetTemperature"] = Convert.ToInt32(Components.Heater.TemperatureSetpoint.Value * 100)
            };
        }

        private void InitializingState_Switched(object sender, StateMachineEventArgs<InitializingSubStateMachineModel.State?, InitializingSubStateMachineModel.Transition?> e)
        {
            if (ActiveJob == null && e.NewState == InitializingSubStateMachineModel.State.Released)
            {
                // There is no active job, but one job changed its state to release.
                // So try to start the next job.
                TryStartNextJob();
            }
        }
        
        private void State_Switched(object sender, StateMachineEventArgs<OpcUa.Glass.ProductionStateMachineModel.State?, OpcUa.Glass.ProductionStateMachineModel.Transition?> e)
        {
            if (ActiveJob?.State == sender && e.NewState == ProductionStateMachineModel.State.Ended)
            {
                MaterialLeaveRequest = true;
            }
        }

        private void TryStartNextJob()
        {
            // We only start a new job if we are in production mode
            if (MachineryBuildingBlocks.MachineryOperationMode.InternalState != MachineryOperationModeStateMachineModel.State.Processing)
            {
                return;
            }

            // Is there already an active job?
            if (ActiveJob != null)
            {
                return;
            }

            // Is there already/still a material there?
            if (IsMaterialInPlace)
            {
                return;
            }
            
            // Find next job
            var job = Production
                .ProductionPlan
                .Cast<TemperingJobModel>()
                .OrderBy(j => j.NumberInList)
                .Where(j => j.State.InitializingState.InternalState == InitializingSubStateMachineModel.State.Released)
                .FirstOrDefault();

            // No job found
            if (job == null)
            {
                Components.Heater.UnsetTemperatureAlarmLimits();
                if (MachineryBuildingBlocks.MachineryItemState.InternalState != MachineryItemState_StateMachineModel.State.NotExecuting)
                {
                    MachineryBuildingBlocks.MachineryItemState.SwitchToState(MachineryItemState_StateMachineModel.State.NotExecuting);
                }
                return;
            }

            var plan = job.Instruction.LoadedPlan;

            ActiveJob = job;
            MaterialEnterRequest = true;
            Components.Heater.SetTemperatureAlarmLimits(plan.MinTemperature, plan.MaxTemperature);
            if (MachineryBuildingBlocks.MachineryItemState.InternalState != MachineryItemState_StateMachineModel.State.Executing)
            {
                MachineryBuildingBlocks.MachineryItemState.SwitchToState(MachineryItemState_StateMachineModel.State.Executing);
            }
        }

        public StatusCode Process(RequestContext context, GlassTemperingMachineModel model)
        {
            if (MachineryBuildingBlocks.MachineryOperationMode.InternalState == MachineryOperationModeStateMachineModel.State.Processing)
            {
                return StatusCodes.BadInvalidState;
            }

            MachineryBuildingBlocks.MachineryOperationMode.SwitchToState(MachineryOperationModeStateMachineModel.State.Processing);
            return StatusCodes.Good;
        }

        public StatusCode Setup(RequestContext context, GlassTemperingMachineModel model)
        {
            if (MachineryBuildingBlocks.MachineryOperationMode.InternalState == MachineryOperationModeStateMachineModel.State.Setup)
            {
                return StatusCodes.BadInvalidState;
            }

            MachineryBuildingBlocks.MachineryOperationMode.SwitchToState(MachineryOperationModeStateMachineModel.State.Setup);
            return StatusCodes.Good;
        }

        [UaInstanceDeclaration(BrowseName = "Process", NamespaceUri = Namespaces.MachineDemoServer, AttributeId = Attributes.Executable)]
        public bool IsProcessExecutable
        {
            get => m_isProcessExecutable;
            set => SetField(ref m_isProcessExecutable, value);
        }
        private bool m_isProcessExecutable;

        [UaInstanceDeclaration(BrowseName = "Setup", NamespaceUri = Namespaces.MachineDemoServer, AttributeId = Attributes.Executable)]
        public bool IsSetupExecutable
        {
            get => m_isSetupExecutable;
            set => SetField(ref m_isSetupExecutable, value);
        }
        private bool m_isSetupExecutable;
    }
}
