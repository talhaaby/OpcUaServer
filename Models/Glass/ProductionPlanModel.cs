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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;

namespace OpcUa.Glass
{
    //! [INotifyCollectionChanged]
    public partial class ProductionPlanModel : INotifyCollectionChanged, IEnumerable<ProductionJobModel>
    {
        //! [INotifyCollectionChanged]
        private readonly List<ProductionJobModel> m_jobs = new List<ProductionJobModel>();

        //! [INotifyCollectionChanged event]
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        //! [INotifyCollectionChanged event]

        public int Count => m_jobs.Count;

        //! [INPC property]
        public int Version
        {
            get => m_version;
            set => SetField(ref m_version, value);
        }
        private int m_version;
        //! [INPC property]

        //! [ModelAnnotator]
        public static ModelAnnotator ModelAnnotator
        {
            get
            {
                var annotator = new ModelAnnotator();
                annotator.Instance(nameof(NodeVersion))
                    .HasInitMode(InitMode.FromModelToNode);

                return annotator;
            }
        }
        //! [ModelAnnotator]

        //! [Version]
        public ProductionPlanModel(ProductionPlanModel template) : this(template, default(DummyArgument))
        {
            PropertyChanged += (o, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(Version):
                        NodeVersion = Version.ToString();
                        break;
                }
            };

            Version++;
        }
        //! [Version]

        //! [Add]
        public void Add(ProductionJobModel item)
        {
            m_jobs.Add(item);
            Version++;
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));

            item.NumberInList = ushort.MaxValue;
            item.State.SwitchToState(ProductionStateMachineModel.State.Initializing);
            ReorderList();
        }
        //! [Add]

        public IEnumerator<ProductionJobModel> GetEnumerator()
        {
            return m_jobs.GetEnumerator();
        }

        public bool Remove(ProductionJobModel item)
        {
            if (m_jobs.Remove(item))
            {
                Version++;
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));

                ReorderList();
                return true;
            }

            return false;
        }

        public void Move(ProductionJobModel targetJob, ProductionJobModel sourceJob, bool Before)
        {
            var targetNumber = targetJob.NumberInList;
            var sourceNumber = sourceJob.NumberInList;

            if (!Before)
            {
                targetNumber++;
            }

            foreach (var item in this)
            {
                if (item.NumberInList == sourceNumber)
                {
                    item.NumberInList = targetNumber;
                }
                else if (item.NumberInList >= targetNumber)
                {
                    item.NumberInList++;
                }
            }

            ReorderList();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_jobs).GetEnumerator();
        }
        
        private void ReorderList()
        {
            var ordered = m_jobs.OrderBy(x => x.NumberInList).ToList();

            ushort i = 1;
            foreach (var item in ordered)
            {
                item.NumberInList = i;
                i++;
            }
        }
    }
}
