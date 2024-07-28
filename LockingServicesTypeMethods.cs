/******************************************************************************
**
** <auto-generated>
**     This code was generated by a tool: UaModeler
**     Runtime Version: 1.6.9, using .NET Server 3.3.0 template (version 0)
**
**     This is a template file that was generated for your convenience.
**     This file will not be overwritten when generating code again.
**     ADD YOUR IMPLEMTATION HERE!
** </auto-generated>
**
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
** Project: .NET OPC UA SDK information model for namespace http://opcfoundation.org/UA/DI/
**
** Description: OPC Unified Architecture Software Development Kit.
**
** The complete license agreement can be found here:
** http://unifiedautomation.com/License/SLA/2.8/
**
******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;

namespace OpcUa.DI
{
    public partial class LockingServicesModel : BaseObjectModel, ILockingServicesMethods
    {
        /// <summary>
        /// https://reference.opcfoundation.org/DI/v104/docs/7.8
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="BreakLockStatus">out: </param>
        /// <returns></returns>
        public StatusCode BreakLock(
            RequestContext context,
            LockingServicesModel model,
            out int BreakLockStatus
            )
        {
            BreakLockStatus = (int)0;
            return StatusCodes.BadNotImplemented;
        }

        /// <summary>
        /// https://reference.opcfoundation.org/DI/v104/docs/7.6
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="ExitLockStatus">out: </param>
        /// <returns></returns>
        public StatusCode ExitLock(
            RequestContext context,
            LockingServicesModel model,
            out int ExitLockStatus
            )
        {
            ExitLockStatus = (int)0;
            return StatusCodes.BadNotImplemented;
        }

        /// <summary>
        /// https://reference.opcfoundation.org/DI/v104/docs/7.5
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="Context"></param>
        /// <param name="InitLockStatus">out: </param>
        /// <returns></returns>
        public StatusCode InitLock(
            RequestContext context,
            LockingServicesModel model,
            string Context,
            out int InitLockStatus
            )
        {
            InitLockStatus = (int)0;
            return StatusCodes.BadNotImplemented;
        }

        /// <summary>
        /// https://reference.opcfoundation.org/DI/v104/docs/7.7
        /// </summary>
        /// <param name="context"></param>
        /// <param name="model"></param>
        /// <param name="RenewLockStatus">out: </param>
        /// <returns></returns>
        public StatusCode RenewLock(
            RequestContext context,
            LockingServicesModel model,
            out int RenewLockStatus
            )
        {
            RenewLockStatus = (int)0;
            return StatusCodes.BadNotImplemented;
        }

    }
}

