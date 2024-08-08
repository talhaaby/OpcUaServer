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
/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;

namespace OpcUa.Server
{
    /// <summary>
    /// A class that provides access to the underlying system.
    /// </summary>
    public class UnderlyingSystem : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UnderlyingSystem"/> class.
        /// </summary>
        public UnderlyingSystem()
        {
            m_registers = new byte[4096];
            m_blocks = new Dictionary<int, BlockConfiguration>();
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_simulationTimer != null)
                {
                    m_simulationTimer.Dispose();
                    m_simulationTimer = null;
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            // load the configuration file.
            Load();

            // start the simulation timer.
            m_simulationTimer = new Timer(DoSimulation, null, 1000, 1000);
        }

        /// <summary>
        /// Gets the blockAddress configurations.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BlockConfiguration> GetBlocks()
        {
            return m_blocks.Values;
        }

        /// <summary>
        /// Reads the tag value.
        /// </summary>
        /// <param name="blockAddress">The blockAddress.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>The value. null if no value exists.</returns>
        public object Read(int blockAddress, int tag)
        {
            lock (m_lock)
            {
                if (blockAddress < 0 || tag < 0)
                {
                    return null;
                }

                if (blockAddress + tag > m_position - sizeof(int))
                {
                    return null;
                }

                BlockConfiguration controller = null;

                if (!m_blocks.TryGetValue(blockAddress, out controller))
                {
                    return null;
                }

                foreach (BlockProperty property in controller.Properties)
                {
                    if (property.Offset == tag)
                    {
                        if (property.DataType == UnifiedAutomation.UaBase.DataTypeIds.Double)
                        {
                            return (double)BitConverter.ToSingle(m_registers, blockAddress + tag);
                        }

                        if (property.DataType == UnifiedAutomation.UaBase.DataTypeIds.Int32)
                        {
                            return BitConverter.ToInt32(m_registers, blockAddress + tag);
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Writes the tag value.
        /// </summary>
        /// <param name="blockAddress">The blockAddress.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// True if the write was successful.
        /// </returns>
        public bool Write(int blockAddress, int tag, object value)
        {
            lock (m_lock)
            {
                if (blockAddress < 0 || tag < 0)
                {
                    return false;
                }

                if (blockAddress + tag > m_position - sizeof(int))
                {
                    return false;
                }

                BlockConfiguration controller = null;

                if (!m_blocks.TryGetValue(blockAddress, out controller))
                {
                    return false;
                }

                foreach (BlockProperty property in controller.Properties)
                {
                    if (property.Offset == tag)
                    {
                        if (!property.Writeable)
                        {
                            return false;
                        }

                        if (property.DataType == UnifiedAutomation.UaBase.DataTypeIds.Double)
                        {
                            Write(blockAddress, tag, (double)value);
                            return true;
                        }

                        if (property.DataType == UnifiedAutomation.UaBase.DataTypeIds.Int32)
                        {
                            Write(blockAddress, tag, (int)value);
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Starts the specified object id.
        /// </summary>
        /// <param name="blockAddress">The blockAddress.</param>
        /// <returns></returns>
        public StatusCode Start(int blockAddress)
        {
            lock (m_lock)
            {
                BlockConfiguration controller = null;

                if (!m_blocks.TryGetValue(blockAddress, out controller))
                {
                    return StatusCodes.BadNodeIdUnknown;
                }

                foreach (BlockProperty property in controller.Properties)
                {
                    if (property.Name == "State")
                    {
                        Write(blockAddress, property.Offset, (int)1);
                        break;
                    }
                }

                return StatusCodes.Good;
            }
        }

        /// <summary>
        /// Stops the specified object id.
        /// </summary>
        /// <param name="blockAddress">The blockAddress.</param>
        /// <returns></returns>
        public StatusCode Stop(int blockAddress)
        {
            lock (m_lock)
            {
                BlockConfiguration controller = null;

                if (!m_blocks.TryGetValue(blockAddress, out controller))
                {
                    return StatusCodes.BadNodeIdUnknown;
                }

                foreach (BlockProperty property in controller.Properties)
                {
                    if (property.Name == "State")
                    {
                        Write(blockAddress, property.Offset, (int)0);
                        break;
                    }
                }

                return StatusCodes.Good;
            }
        }

        /// <summary>
        /// Called when to start the simulation with a set point.
        /// </summary>
        /// <param name="blockAddress">The blockAddress.</param>
        /// <param name="temperatureSetPoint">The temperature set point.</param>
        /// <param name="humditySetPoint">The humidity set point.</param>
        /// <returns></returns>
        public StatusCode StartWithSetPoint(int blockAddress, double temperatureSetPoint, double humditySetPoint)
        {
            lock (m_lock)
            {
                BlockConfiguration controller = null;

                if (!m_blocks.TryGetValue(blockAddress, out controller))
                {
                    return StatusCodes.BadNodeIdUnknown;
                }

                foreach (BlockProperty property in controller.Properties)
                {
                    if (property.Name == "TemperatureSetPoint")
                    {
                        Write(blockAddress, property.Offset, temperatureSetPoint);
                    }

                    else if (property.Name == "HumiditySetPoint")
                    {
                        Write(blockAddress, property.Offset, humditySetPoint);
                    }

                    else if (property.Name == "State")
                    {
                        Write(blockAddress, property.Offset, (int)1);
                    }
                }

                return StatusCodes.Good;
            }
        }
        #endregion

        #region Private Method
        /// <summary>
        /// Loads the configuration for the system.
        /// </summary>
        private void Load()
        {
            var assembly = typeof(UnderlyingSystem).Assembly;
            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                if (resourceName.EndsWith(".SystemConfiguration.xml"))
                {
                    using (Stream istrm = assembly.GetManifestResourceStream(resourceName))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                        m_configuration = (Configuration)serializer.Deserialize(istrm);
                    }
                }
            }

            if (m_configuration.Controllers != null)
            {
                for (int ii = 0; ii < m_configuration.Controllers.Length; ii++)
                {
                    ControllerConfiguration controller = m_configuration.Controllers[ii];

                    int blockAddress = m_position;
                    int offset = m_position - blockAddress;

                    BlockConfiguration data = new BlockConfiguration()
                    {
                        Address = blockAddress,
                        Name = controller.Name,
                        Type = controller.Type,
                        Properties = new List<BlockProperty>()
                    };

                    if (controller.Properties != null)
                    {
                        for (int jj = 0; jj < controller.Properties.Length; jj++)
                        {
                            ControllerProperty property = controller.Properties[jj];
                            NodeId dataTypeId = NodeId.Parse(property.DataType);
                            string value = property.Value;
                            UnifiedAutomation.UaBase.Range range = null;

                            if (!String.IsNullOrEmpty(property.Range))
                            {
                                try
                                {
                                    NumericRange nr = NumericRange.Parse(property.Range);
                                    range = new UnifiedAutomation.UaBase.Range() { High = nr.End, Low = nr.Begin };
                                }
                                catch (Exception)
                                {
                                    range = null;
                                }
                            }

                            data.Properties.Add(new BlockProperty()
                            {
                                Offset = offset,
                                Name = controller.Properties[jj].Name,
                                DataType = dataTypeId,
                                Writeable = controller.Properties[jj].Writeable,
                                Range = range
                            });

                            switch ((uint)dataTypeId.Identifier)
                            {
                                case UnifiedAutomation.UaBase.DataTypes.Int32:
                                {
                                    Write(blockAddress, offset, (int)TypeUtils.Cast(value, BuiltInType.Int32));
                                    offset += 4;
                                    break;
                                }

                                case UnifiedAutomation.UaBase.DataTypes.Double:
                                {
                                    Write(blockAddress, offset, (double)TypeUtils.Cast(value, BuiltInType.Double));
                                    offset += 4;
                                    break;
                                }
                            }
                        }
                    }

                    m_position += offset;
                    m_blocks[blockAddress] = data;
                }
            }
        }

        /// <summary>
        /// Writes the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        private void Write(int blockAddress, int offset, int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            Array.Copy(bytes, 0, m_registers, blockAddress + offset, bytes.Length);
        }

        /// <summary>
        /// Writes the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="value">The value.</param>
        private void Write(int blockAddress, int offset, double value)
        {
            byte[] bytes = BitConverter.GetBytes((float)value);
            Array.Copy(bytes, 0, m_registers, blockAddress + offset, bytes.Length);
        }

        /// <summary>
        /// Does the simulation.
        /// </summary>
        /// <param name="state">The state.</param>
        private void DoSimulation(object state)
        {
            try
            {
                lock (m_lock)
                {
                    foreach (var blockAddress in m_blocks)
                    {
                        int active = 1;

                        for (int ii = 0; ii < blockAddress.Value.Properties.Count; ii++)
                        {
                            if (blockAddress.Value.Properties[ii].Name == "State")
                            {
                                active = (int)Read(blockAddress.Value.Address, blockAddress.Value.Properties[ii].Offset);
                                continue;
                            }
                        }

                        if (active != 0)
                        {
                            for (int ii = 0; ii < blockAddress.Value.Properties.Count - 1; ii++)
                            {
                                string firstName = blockAddress.Value.Properties[ii].Name;
                                string secondName = blockAddress.Value.Properties[ii + 1].Name;

                                if (!secondName.StartsWith(firstName) || !secondName.EndsWith("SetPoint"))
                                {
                                    continue;
                                }

                                int valueOffset = blockAddress.Value.Properties[ii].Offset;
                                int setpointOffset = blockAddress.Value.Properties[ii + 1].Offset;

                                double value = (double)Read(blockAddress.Key, valueOffset);
                                double setpoint = (double)Read(blockAddress.Key, setpointOffset);

                                Write(blockAddress.Key, valueOffset, Adjust(value, setpoint));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                TraceServer.Error(e, "Failed run simulation.");
            }
        }

        /// <summary>
        /// Adjusts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="setPoint">The set point.</param>
        /// <returns></returns>
        private double Adjust(double value, double setPoint)
        {
            Random random = new Random();

            if (Math.Abs(setPoint - value) > 1)
            {
                double delta = (Math.Abs(setPoint - value) + 1) * random.NextDouble();
                return value + ((setPoint > value) ? delta : -delta);
            }
            else
            {
                double delta = Math.Abs(setPoint - value) + 1.1;
                return value + ((random.Next()%2 == 0) ? delta : -delta);
            }
        }
        #endregion

        #region Configuration File Classes
        [XmlType(TypeName = "UnderlyingSystem.ControllerProperty", Namespace = "http://yourcompany.com/underlyingsystem")]
        public class ControllerProperty
        {
            [XmlElement(Order = 1)]
            public string Name { get; set; }

            [XmlElement(Order = 2)]
            public string DataType { get; set; }

            [XmlElement(Order = 3)]
            public string Value { get; set; }

            [XmlElement(Order = 4)]
            public bool Writeable { get; set; }

            [XmlElement(Order = 5)]
            public string Range { get; set; }
        }

        [XmlType(TypeName = "UnderlyingSystem.ControllerConfiguration", Namespace = "http://yourcompany.com/underlyingsystem")]
        public class ControllerConfiguration
        {
            [XmlElement(Order = 1)]
            public string Name { get; set; }

            [XmlElement(Order = 2)]
            public int Type { get; set; }

            [XmlElement(Order = 3)]
            public ControllerProperty[] Properties;
        }

        [XmlRoot(ElementName = "UnderlyingSystem.Configuration", Namespace = "http://yourcompany.com/underlyingsystem")]
        public class Configuration
        {
            [XmlElement(Order = 1)]
            public ControllerConfiguration[] Controllers;
        }
        #endregion

        #region Private Fields
        private object m_lock = new object();
        private byte[] m_registers;
        private int m_position;
        private Dictionary<int, BlockConfiguration> m_blocks;
        private Configuration m_configuration;
        private Timer m_simulationTimer;
        #endregion
    }

    #region BlockProperty Class
    /// <summary>
    /// The configuration for a property of a blockAddress.
    /// </summary>
    public class BlockProperty
    {
        public int Offset;
        public string Name;
        public NodeId DataType;
        public bool Writeable;
        public UnifiedAutomation.UaBase.Range Range;
    }
    #endregion

    #region BlockConfiguration Class
    /// <summary>
    /// The configuration for a blockAddress.
    /// </summary>
    public class BlockConfiguration
    {
        public int Address;
        public string Name;
        public int Type;
        public List<BlockProperty> Properties;
    }
    #endregion

    #region BlockType Class
    public static class BlockType
    {
        public const int AirConditioner = 1;
        public const int Furnace = 2;
    }
    #endregion
}
*/