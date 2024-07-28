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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Reflection;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;
namespace UnifiedAutomation.MachineDemoServer
{
    internal partial class MachineDemoServerNodeManager : BaseNodeManager
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public MachineDemoServerNodeManager(ServerManager server) : base(server)
        {
        }
        #endregion

        #region IDisposable
        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TBD
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Called when the node manager is started.
        /// </summary>
        public override void Startup()
        {
            try
            {
                Console.WriteLine("Starting MachineDemoServerNodeManager.");

                DefaultNamespaceIndex = AddNamespaceUri("http://unified-automation.com/MachineDemoServer/");

                // Add structures.
                Server.MessageContext.Factory.AddEncodeableType(typeof(GlassTemperingPlan));

                Console.WriteLine("Loading the MachineDemoServer Model.");
                ImportUaNodeset(Assembly.GetEntryAssembly(), "machinedemoserver.xml");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to start MachineDemoServerNodeManager " + e.Message);
            }
        }

        /// <summary>
        /// Called when the node manager is stopped.
        /// </summary>
        public override void Shutdown()
        {
            try
            {
                Console.WriteLine("Stopping MachineDemoServerNodeManager.");

                // TBD
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to stop MachineDemoServerNodeManager " + e.Message);
            }
        }
        #endregion

        #region Private Methods
        #endregion

        #region Private Fields
        #endregion
    }
}

