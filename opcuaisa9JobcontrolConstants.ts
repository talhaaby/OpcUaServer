
export enum DataTypeIds {
   ISA95EquipmentDataType = 'i=3005',
   ISA95JobOrderAndStateDataType = 'i=3015',
   ISA95JobOrderDataType = 'i=3008',
   ISA95JobResponseDataType = 'i=3013',
   ISA95MaterialDataType = 'i=3010',
   ISA95ParameterDataType = 'i=3003',
   ISA95PersonnelDataType = 'i=3011',
   ISA95PhysicalAssetDataType = 'i=3012',
   ISA95PropertyDataType = 'i=3002',
   ISA95StateDataType = 'i=3006',
   ISA95WorkMasterDataType = 'i=3007'
}

export enum MethodIds {
   ISA95JobResponseProviderObjectType_RequestJobResponseByJobOrderID = 'i=7002',
   ISA95JobResponseProviderObjectType_RequestJobResponseByJobOrderState = 'i=7014',
   ISA95JobResponseReceiverObjectType_ReceiveJobResponse = 'i=7003',
   ISA95JobOrderReceiverObjectType_Abort = 'i=7010',
   ISA95JobOrderReceiverObjectType_Cancel = 'i=7011',
   ISA95JobOrderReceiverObjectType_Clear = 'i=7012',
   ISA95JobOrderReceiverObjectType_Pause = 'i=7007',
   ISA95JobOrderReceiverObjectType_Resume = 'i=7008',
   ISA95JobOrderReceiverObjectType_RevokeStart = 'i=7013',
   ISA95JobOrderReceiverObjectType_Start = 'i=7005',
   ISA95JobOrderReceiverObjectType_Stop = 'i=7006',
   ISA95JobOrderReceiverObjectType_Store = 'i=7001',
   ISA95JobOrderReceiverObjectType_StoreAndStart = 'i=7004',
   ISA95JobOrderReceiverObjectType_Update = 'i=7009',
}

export enum ObjectIds {
   http___opcfoundation_org_UA_ISA95_JOBCONTROL_V2_ = 'i=5001'
}

export enum ObjectTypeIds {
   ISA95JobOrderStatusEventType = 'i=1006',
   ISA95JobResponseProviderObjectType = 'i=1003',
   ISA95JobResponseReceiverObjectType = 'i=1004',
   ISA95EndedStateMachineType = 'i=1005',
   ISA95InterruptedStateMachineType = 'i=1007',
   ISA95JobOrderReceiverObjectType = 'i=1002',
   ISA95JobOrderReceiverSubStatesType = 'i=1008',
   ISA95PrepareStateMachineType = 'i=1001'
}

export enum VariableIds {
   http___opcfoundation_org_UA_ISA95_JOBCONTROL_V2__NamespaceUri = 'i=6025',
   http___opcfoundation_org_UA_ISA95_JOBCONTROL_V2__NamespaceVersion = 'i=6026',
   http___opcfoundation_org_UA_ISA95_JOBCONTROL_V2__NamespacePublicationDate = 'i=6024',
   http___opcfoundation_org_UA_ISA95_JOBCONTROL_V2__IsNamespaceSubset = 'i=6023',
   http___opcfoundation_org_UA_ISA95_JOBCONTROL_V2__StaticNodeIdTypes = 'i=6027',
   http___opcfoundation_org_UA_ISA95_JOBCONTROL_V2__StaticNumericNodeIdRange = 'i=6028',
   http___opcfoundation_org_UA_ISA95_JOBCONTROL_V2__StaticStringNodeIdPattern = 'i=6029'
}

export class StatusCode {
   public static isGood(code?: number): boolean {
      return !code || (code & 0xD0000000) === 0;
   }
   public static isUncertain(code?: number): boolean {
      return (code ?? 0 & 0x40000000) !== 0;
   }
   public static isBad(code?: number): boolean {
      return (code ?? 0 & 0x80000000) !== 0;
   }
}