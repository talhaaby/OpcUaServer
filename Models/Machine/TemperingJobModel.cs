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
using System;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;

namespace UnifiedAutomation.MachineDemoServer
{
    public partial class TemperingJobModel : IProductionJobMethods
    {
        [UaInstanceDeclaration(AttributeId = Attributes.NodeId, IsAttribute = true)]
        public NodeId JobNodeId { get; set; }

        public static ModelAnnotator ModelAnnotator
        {
            get
            {
                var annotator = new ModelAnnotator();
                annotator.Instance(nameof(Identifier))
                    .HasInitMode(InitMode.FromModelToNode)
                    .HasBindMode(BindMode.None);
                annotator.Instance(nameof(Name))
                    .HasInitMode(InitMode.FromModelToNode)
                    .HasBindMode(BindMode.None);

                return annotator;
            }
        }

        internal void Initialize()
        {
            State.InitializingState.Switched += (o, e) =>
            {
                if (e.NewState == InitializingSubStateMachineModel.State.Idle)
                {
                    Instruction.Plan.Writable = true;
                    Instruction.Plan.UserWritable = true;
                }
                else if (e.OldState == InitializingSubStateMachineModel.State.Idle)
                {
                    Instruction.Plan.Writable = false;
                    Instruction.Plan.UserWritable = false;
                }
            };

            State.Switched += (o, e) =>
            {
                if (e.OldState == ProductionStateMachineModel.State.Initializing)
                {
                    IsQueueJobExecutable = true;
                    IsReleaseJobExecutable = true;
                }
                else
                {
                    IsQueueJobExecutable = false;
                    IsReleaseJobExecutable = false;
                }
            };
            
            State.SwitchToState(ProductionStateMachineModel.State.Initializing);
            State.InitializingState.SwitchToState(InitializingSubStateMachineModel.State.Idle);
        }

        public override StatusCode AbortJob(RequestContext context, ProductionJobModel model)
        {
            return StatusCodes.BadNotSupported;
        }

        [UaInstanceDeclaration(BrowseName = "QueueJob", NamespaceUri = Namespaces.Glass, AttributeId = Attributes.Executable)]
        public bool IsQueueJobExecutable
        {
            get => m_isQueueJobExecutable;
            set => SetField(ref m_isQueueJobExecutable, value);
        }
        private bool m_isQueueJobExecutable;

        public override StatusCode QueueJob(RequestContext context, ProductionJobModel job)
        {
            if (State.InitializingState.InternalState == InitializingSubStateMachineModel.State.Idle
                || State.InitializingState.InternalState == InitializingSubStateMachineModel.State.Released)
            {
                var ret = Instruction.LoadPlan();

                if (ret.IsGood())
                {
                    State.InitializingState.SwitchToState(InitializingSubStateMachineModel.State.Queued);
                }

                return ret;
            }

            return StatusCodes.BadInvalidState;
        }

        [UaInstanceDeclaration(BrowseName = "ReleaseJob", NamespaceUri = Namespaces.Glass, AttributeId = Attributes.Executable)]
        public bool IsReleaseJobExecutable
        {
            get => m_isReleaseJobExecutable;
            set => SetField(ref m_isReleaseJobExecutable, value);
        }
        private bool m_isReleaseJobExecutable;

        public override StatusCode ReleaseJob(RequestContext context, ProductionJobModel model)
        {
            if (State.InitializingState.InternalState == InitializingSubStateMachineModel.State.Queued)
            {
                State.InitializingState.SwitchToState(InitializingSubStateMachineModel.State.Released);

                return StatusCodes.Good;
            }

            return StatusCodes.BadInvalidState;
        }

        public override StatusCode SuspendJob(RequestContext context, ProductionJobModel model)
        {
            return StatusCodes.BadNotSupported;
        }

        public void Start()
        {
            if (State.InitializingState.InternalState == InitializingSubStateMachineModel.State.Released)
            {
                StartTime = DateTime.Now;
                State.SwitchToState(ProductionStateMachineModel.State.Running);
            }
        }

        public void CheckForEnd()
        {
            var processTime = TimeSpan.FromMilliseconds(Instruction.LoadedPlan.ProcessTime);

            if (State.InternalState == ProductionStateMachineModel.State.Running
                && DateTime.Now - StartTime > processTime)
            {
                EndTime = DateTime.Now;
                State.SwitchToState(ProductionStateMachineModel.State.Ended);
            }
        }
    }
}

