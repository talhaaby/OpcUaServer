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
using UnifiedAutomation.UaServer;

namespace UnifiedAutomation.MachineDemoServer
{
    //! [Class declaration]
    internal partial class MachineDemoServerNodeManager : BaseNodeManager
    //! [Class declaration]
    {
        //! [Enable]
        public override StatusCode Enable(RequestContext context, ConditionModel model)
        {
            return StatusCodes.BadUserAccessDenied;
        }
        //! [Enable]

        public override StatusCode Disable(RequestContext context, ConditionModel model)
        {
            return StatusCodes.BadUserAccessDenied;
        }

        //! [Acknowledge]
        public override StatusCode Acknowledge(RequestContext context, AcknowledgeableConditionModel model, byte[] eventId, LocalizedText comment)
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Acknowledge");
            return model.Acknowledge(eventId, comment, context.UserIdentity.UserName);
        }
        //! [Acknowledge]

        public override StatusCode AddComment(RequestContext context, ConditionModel model, byte[] eventId, LocalizedText comment)
        {
            return model.AddComment(eventId, comment, context.UserIdentity.UserName);
        }

        public override StatusCode Confirm(RequestContext context, AcknowledgeableConditionModel model, byte[] eventId, LocalizedText comment)
        {
            return model.Confirm(eventId, comment, context.UserIdentity.UserName);
        }

        public override StatusCode PlaceInService(RequestContext context, AlarmConditionModel model)
        {
            return model.PlaceInService();
        }

        public override StatusCode PlaceInService2(RequestContext context, AlarmConditionModel model, LocalizedText Comment)
        {
            using (model.MergeTransitions())
            {
                var status = model.PlaceInService();
                if (status.IsGood())
                {
                    model.AddComment(model.EventId, Comment, context.UserIdentity.UserName);
                }
                return status;
            }
        }

        public override StatusCode RemoveFromService(RequestContext context, AlarmConditionModel model)
        {
            return model.RemoveFromService();
        }

        public override StatusCode RemoveFromService2(RequestContext context, AlarmConditionModel model, LocalizedText Comment)
        {
            using (model.MergeTransitions())
            {
                var status = model.RemoveFromService();
                if (status.IsGood() && !LocalizedText.IsNullOrEmpty(Comment))
                {
                    model.AddComment(model.EventId, Comment, context.UserIdentity.UserName);
                }
                return status;
            }
        }

        public override StatusCode Reset(RequestContext context, AlarmConditionModel model)
        {
            return model.Reset();
        }

        public override StatusCode Reset2(RequestContext context, AlarmConditionModel model, LocalizedText Comment)
        {
            using (model.MergeTransitions())
            {
                var status = model.Reset();
                if (status.IsGood() && !LocalizedText.IsNullOrEmpty(Comment))
                {
                    model.AddComment(model.EventId, Comment, context.UserIdentity.UserName);
                }
                return status;
            }
        }

        public override StatusCode Silence(RequestContext context, AlarmConditionModel model)
        {
            return model.Silence();
        }

        //! [Suppress]
        public override StatusCode Suppress(RequestContext context, AlarmConditionModel model)
        {
            return model.Suppress();
        }

        public override StatusCode Suppress2(RequestContext context, AlarmConditionModel model, LocalizedText Comment)
        {
            using (model.MergeTransitions())
            {
                var status = model.Suppress();
                if (status.IsGood() && !LocalizedText.IsNullOrEmpty(Comment))
                {
                    model.AddComment(model.EventId, Comment, context.UserIdentity.UserName);
                }
                return status;
            }
        }
        //! [Suppress]

        public override StatusCode Unsuppress(RequestContext context, AlarmConditionModel model)
        {
            return model.Unsuppress();
        }

        public override StatusCode Unsuppress2(RequestContext context, AlarmConditionModel model, LocalizedText Comment)
        {
            using (model.MergeTransitions())
            {
                var status = model.Unsuppress();
                if (status.IsGood() && !LocalizedText.IsNullOrEmpty(Comment))
                {
                    model.AddComment(model.EventId, Comment, context.UserIdentity.UserName);
                }
                return status;
            }
        }

        //! [OneShotShelve]
        public override StatusCode OneShotShelve(RequestContext context, ShelvedStateMachineModel model)
        {
            var alarm = model.ParentAlarm;
            if (alarm != null)
            {
                return alarm.OneShotShelve();
            }

            return StatusCodes.BadNotSupported;
        }
        //! [OneShotShelve]

        public override StatusCode OneShotShelve2(RequestContext context, ShelvedStateMachineModel model, LocalizedText Comment)
        {
            var alarm = model.ParentAlarm;
            if (alarm != null)
            {
                using (alarm.MergeTransitions())
                {
                    var status = alarm.OneShotShelve();
                    if (status.IsGood() && !LocalizedText.IsNullOrEmpty(Comment))
                    {
                        alarm.AddComment(alarm.EventId, Comment, context.UserIdentity.UserName);
                    }
                    return status;
                }
            }

            return StatusCodes.BadNotSupported;
        }

        public override StatusCode TimedShelve(RequestContext context, ShelvedStateMachineModel model, double shelvingTime)
        {
            var alarm = model.ParentAlarm;
            if (alarm != null)
            {
                return alarm.TimedShelve();
            }

            return StatusCodes.BadNotSupported;
        }

        public override StatusCode TimedShelve2(RequestContext context, ShelvedStateMachineModel model, double ShelvingTime, LocalizedText Comment)
        {
            var alarm = model.ParentAlarm;
            if (alarm != null)
            {
                using (alarm.MergeTransitions())
                {
                    var status = alarm.TimedShelve();
                    if (status.IsGood() && !LocalizedText.IsNullOrEmpty(Comment))
                    {
                        alarm.AddComment(alarm.EventId, Comment, context.UserIdentity.UserName);
                    }
                    return status;
                }
            }

            return StatusCodes.BadNotSupported;
        }

        public override StatusCode Unshelve(RequestContext context, ShelvedStateMachineModel model)
        {
            var alarm = model.ParentAlarm;
            if (alarm != null)
            {
                return alarm.Unshelve();
            }

            return StatusCodes.BadNotSupported;
        }

        public override StatusCode Unshelve2(RequestContext context, ShelvedStateMachineModel model, LocalizedText Comment)
        {
            var alarm = model.ParentAlarm;
            if (alarm != null)
            {
                using (alarm.MergeTransitions())
                {
                    var status = alarm.Unshelve();
                    if (status.IsGood() && !LocalizedText.IsNullOrEmpty(Comment))
                    {
                        alarm.AddComment(alarm.EventId, Comment, context.UserIdentity.UserName);
                    }
                    return status;
                }
            }

            return StatusCodes.BadNotSupported;
        }
    }
}

