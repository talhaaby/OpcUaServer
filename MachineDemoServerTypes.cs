/******************************************************************************
**
** <auto-generated>
**     This code was generated by a tool: UaModeler
**     Runtime Version: 1.6.9, using .NET Server 3.3.0 template (version 0)
**
**     Changes to this file may cause incorrect behavior and will be lost if
**     the code is regenerated.
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
using System.Text;
using System.Reflection;
using System.Xml;
using System.Linq;
using System.Runtime.Serialization;
using UnifiedAutomation.UaBase;
using System.Diagnostics;

namespace UnifiedAutomation.MachineDemoServer
{
    #region GlassTemperingPlan Class
    /// <summary>
    /// </summary>
    [DataContract(Namespace = UnifiedAutomation.MachineDemoServer.Namespaces.MachineDemoServerXsd)]
    public partial class GlassTemperingPlan : IEncodeable
    {
        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public GlassTemperingPlan()
        {
            Initialize();
        }

        [OnDeserializing]
        private void Initialize(StreamingContext context)
        {
            Initialize();
        }

        private void Initialize()
        {
            m_Name = null;
            m_ProcessTime = 0.0;
            m_MinTemperature = 0.0;
            m_MaxTemperature = 0.0;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// </summary>
        [DataMember(Name = "Name", IsRequired = false, Order = 1)]
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        /// <summary>
        /// </summary>
        [DataMember(Name = "ProcessTime", IsRequired = false, Order = 2)]
        public double ProcessTime
        {
            get
            {
                return m_ProcessTime;
            }
            set
            {
                m_ProcessTime = value;
            }
        }

        /// <summary>
        /// </summary>
        [DataMember(Name = "MinTemperature", IsRequired = false, Order = 3)]
        public double MinTemperature
        {
            get
            {
                return m_MinTemperature;
            }
            set
            {
                m_MinTemperature = value;
            }
        }

        /// <summary>
        /// </summary>
        [DataMember(Name = "MaxTemperature", IsRequired = false, Order = 4)]
        public double MaxTemperature
        {
            get
            {
                return m_MaxTemperature;
            }
            set
            {
                m_MaxTemperature = value;
            }
        }

        #endregion

        #region IEncodeable Members
        /// <summary cref="IEncodeable.TypeId" />
        public virtual ExpandedNodeId TypeId
        {
            get { return DataTypeIds.GlassTemperingPlan; }
        }

        /// <summary cref="IEncodeable.BinaryEncodingId" />
        public virtual ExpandedNodeId BinaryEncodingId
        {
            get { return ObjectIds.GlassTemperingPlan_Encoding_DefaultBinary; }
        }
        /// <summary cref="IEncodeable.XmlEncodingId" />
        public virtual ExpandedNodeId XmlEncodingId
        {
            get { return ObjectIds.GlassTemperingPlan_Encoding_DefaultXml; }
        }

        /// <summary cref="IEncodeable.Encode(IEncoder)" />
        public virtual void Encode(IEncoder encoder)
        {
            encoder.PushNamespace(Namespaces.MachineDemoServerXsd);

            encoder.WriteString("Name", Name);
            encoder.WriteDouble("ProcessTime", ProcessTime);
            encoder.WriteDouble("MinTemperature", MinTemperature);
            encoder.WriteDouble("MaxTemperature", MaxTemperature);

            encoder.PopNamespace();
        }

        /// <summary cref="IEncodeable.Decode(IDecoder)" />
        public virtual void Decode(IDecoder decoder)
        {
            decoder.PushNamespace(Namespaces.MachineDemoServerXsd);
            Name = decoder.ReadString("Name");
            ProcessTime = decoder.ReadDouble("ProcessTime");
            MinTemperature = decoder.ReadDouble("MinTemperature");
            MaxTemperature = decoder.ReadDouble("MaxTemperature");

            decoder.PopNamespace();
        }

        /// <summary cref="EncodeableObject.IsEqual(IEncodeable)" />
        public virtual bool IsEqual(IEncodeable encodeable)
        {
            if (Object.ReferenceEquals(this, encodeable))
            {
                return true;
            }

            GlassTemperingPlan value = encodeable as GlassTemperingPlan;

            if (value == null)
            {
                return false;
            }
            if (!TypeUtils.IsEqual(m_Name, value.m_Name)) return false;
            if (!TypeUtils.IsEqual(m_ProcessTime, value.m_ProcessTime)) return false;
            if (!TypeUtils.IsEqual(m_MinTemperature, value.m_MinTemperature)) return false;
            if (!TypeUtils.IsEqual(m_MaxTemperature, value.m_MaxTemperature)) return false;

            return true;
        }

        /// <summary cref="ICloneable.Clone" />
        public virtual object Clone()
        {
            GlassTemperingPlan clone = (GlassTemperingPlan)this.MemberwiseClone();

            clone.m_Name = (string)TypeUtils.Clone(this.m_Name);
            clone.m_ProcessTime = (double)TypeUtils.Clone(this.m_ProcessTime);
            clone.m_MinTemperature = (double)TypeUtils.Clone(this.m_MinTemperature);
            clone.m_MaxTemperature = (double)TypeUtils.Clone(this.m_MaxTemperature);

            return clone;
        }
        #endregion

        #region Private Fields
        private string m_Name;
        private double m_ProcessTime;
        private double m_MinTemperature;
        private double m_MaxTemperature;
        #endregion
    }

    #region GlassTemperingPlanCollection class
    /// <summary>
    /// A collection of GlassTemperingPlan objects.
    /// </summary>
    [CollectionDataContract(Name = "ListOfGlassTemperingPlan", Namespace = UnifiedAutomation.MachineDemoServer.Namespaces.MachineDemoServer, ItemName = "GlassTemperingPlan")]
    public partial class GlassTemperingPlanCollection : List<GlassTemperingPlan>, ICloneable
    {
        #region Constructors
        /// <summary>
        /// Initializes the collection with default values.
        /// </summary>
        public GlassTemperingPlanCollection() { }

        /// <summary>
        /// Initializes the collection with an initial capacity.
        /// </summary>
        public GlassTemperingPlanCollection(int capacity) : base(capacity) { }

        /// <summary>
        /// Initializes the collection with another collection.
        /// </summary>
        public GlassTemperingPlanCollection(IEnumerable<GlassTemperingPlan> collection) : base(collection) { }
        #endregion

        #region Static Operators
        /// <summary>
        /// Converts an array to a collection.
        /// </summary>
        public static implicit operator GlassTemperingPlanCollection(GlassTemperingPlan[] values)
        {
            if (values != null)
            {
                return new GlassTemperingPlanCollection(values);
            }

            return new GlassTemperingPlanCollection();
        }

        /// <summary>
        /// Converts a collection to an array.
        /// </summary>
        public static explicit operator GlassTemperingPlan[](GlassTemperingPlanCollection values)
        {
            if (values != null)
            {
                return values.ToArray();
            }

            return null;
        }
        #endregion

        #region ICloneable Methods
        /// <summary>
        /// Creates a deep copy of the collection.
        /// </summary>
        public object Clone()
        {
            GlassTemperingPlanCollection clone = new GlassTemperingPlanCollection(this.Count);

            for (int ii = 0; ii < this.Count; ii++)
            {
                clone.Add((GlassTemperingPlan)TypeUtils.Clone(this[ii]));
            }

            return clone;
        }
        #endregion
    }
    #endregion
    #endregion


    #region EncodeableTypes
    /// <summary>
    /// Contains a method for registering all encodeable types of the namespace.
    /// </summary>
    public class EncodeableTypes
    {
        /// <summary>
        /// Register all encodeable types of the namespace at the communication stack.
        /// The Decoder will decode the registered types.
        /// </summary>
        public static void RegisterEncodeableTypes(MessageContext context)
        {
            context.Factory.AddEncodeableType(typeof(UnifiedAutomation.MachineDemoServer.GlassTemperingPlan));
        }
    }
    #endregion
}
