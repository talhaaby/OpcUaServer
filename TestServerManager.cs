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
** Project: .NET OPC UA SDK information model for namespace http://unified-automation.com/MachineDemoServer/
**
** Description: OPC Unified Architecture Software Development Kit.
**
** The complete license agreement can be found here:
** http://unifiedautomation.com/License/SLA/2.8/
**
******************************************************************************/

using OpcUa.JobControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnifiedAutomation.MachineDemoServer.Models.Jobs;
using UnifiedAutomation.MachineDemoServer.Models.Jobs.UnifiedAutomation.MachineDemoServer;
using UnifiedAutomation.UaServer;

namespace UnifiedAutomation.MachineDemoServer
{
    internal class TestServerManager : ServerManager
    {
        public TestServerManager()
        {
        }

        protected override void OnRootNodeManagerStarted(RootNodeManager nodeManager)
        {
            UnifiedAutomation.UaServer.BaseNodeManager nm;
            nm = new OpcUa.JobControl.JobControlManager(this);
            nm.Startup();
            nm = new OpcUa.DI.DINodeManager(this);
            nm.Startup();
            nm = new OpcUa.Machinery.MachineryNodeManager(this);
            nm.Startup();
            nm = new OpcUa.MachineryJobs.MachineryJobsNodeManager(this);
            nm.Startup();
            nm = new OpcUa.IA.IANodeManager(this);
            nm.Startup();
            nm = new OpcUa.Glass.GlassNodeManager(this);
            nm.Startup();
            nm = new UnifiedAutomation.MachineDemoServer.MachineDemoServerNodeManager(this);
            nm.Startup();
            
        }
    }
}
