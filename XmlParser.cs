using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnifiedAutomation.UaBase;
using UnifiedAutomation.UaServer;
using System.Reflection;
using OpcUa.Server;
using System.IO;
using Org.BouncyCastle.Asn1.Mozilla;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;




namespace Opc.Server
{

    public class XmlParser
    {
        
        

        private readonly ServerNodeManager _nodeManager; // Reference to ServerNodeManager
        private readonly ushort _typeNamespaceIndex;
        private static uint FolderIndex = 787;



        public XmlParser(ServerNodeManager nodeManager, ushort typeNamespaceIndex)
        {
            _nodeManager = nodeManager;
            _typeNamespaceIndex = typeNamespaceIndex;
        }

        public void ParseXmlFile(string xmlFilePath)
        {
            //CREATE FOLDER
            CreateObjectSettings FolderSettings = new CreateObjectSettings()
            {
                ParentNodeId = UnifiedAutomation.UaBase.ObjectIds.ObjectsFolder,
                ReferenceTypeId = ReferenceTypeIds.Organizes,
                RequestedNodeId = new NodeId(FolderIndex, _typeNamespaceIndex),
                BrowseName = new QualifiedName("JobControl", _typeNamespaceIndex),
                TypeDefinitionId = UnifiedAutomation.UaBase.ObjectTypeIds.FolderType
            };

            ObjectNode FolderNode = _nodeManager.CreateObject(_nodeManager.Server.DefaultRequestContext, FolderSettings);

            try
            {


                //CREATE OBJECT TYPE
                UAObjectTypeNodeCreator(FolderNode.NodeId, xmlFilePath);

                //CREATE DATA TYPE
                UADataTypeNodeCreator(UnifiedAutomation.UaBase.ObjectIds.DataTypesFolder , xmlFilePath);

                //CREATE METHODS
                UaMethodNodeCreator(xmlFilePath);

                //CREATE OBJECTS
                UaObjectNodeCreator(FolderNode.NodeId, xmlFilePath);

                //CREATE VARIABLES
                UaVariableNodeCreator(xmlFilePath);

                //ADD REFERENCES
                ElementToReference(xmlFilePath);


                CreateObjectSettings settings = new CreateObjectSettings()
                {
                    ParentNodeId = UnifiedAutomation.UaBase.ObjectIds.ObjectsFolder,
                    ReferenceTypeId = ReferenceTypeIds.Organizes,
                    RequestedNodeId = new NodeId("Controllers", _typeNamespaceIndex),
                    BrowseName = new QualifiedName("Controllers", _typeNamespaceIndex),
                    TypeDefinitionId = UnifiedAutomation.UaBase.ObjectTypeIds.FolderType
                };
                _nodeManager.CreateObject(_nodeManager.Server.DefaultRequestContext, settings);

            }
            catch (XmlException xmlEx)
            {
                Console.WriteLine($"XML Error: {xmlEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
            }
        }
        

        private void UAObjectTypeNodeCreator(NodeId parentNode, string xmlFilePath)
        {
            using (FileStream ObjectType = new FileStream(xmlFilePath, FileMode.Open))
            {
                using (XmlReader ObjectTypeReader = XmlReader.Create(ObjectType))
                {
                    while (ObjectTypeReader.Read())
                    {

                        if (ObjectTypeReader.IsStartElement("UAObjectType"))
                        {
                            CreateObjectTypeSettings ObjectTypeSettings = new CreateObjectTypeSettings
                            {
                                ParentNodeId = parentNode,
                                ReferenceTypeId = ReferenceTypeIds.HasSubtype,
                                IsAbstract = true,
                            };

                            if (ObjectTypeReader.MoveToAttribute("NodeId"))
                                //ObjectTypeSettings.RequestedNodeId = new NodeId(ObjectTypeReader.Value, _typeNamespaceIndex);
                                ObjectTypeSettings.RequestedNodeId = new NodeId(GetNodeId(ObjectTypeReader.Value), _typeNamespaceIndex);

                            if (ObjectTypeReader.MoveToAttribute("BrowseName"))
                                ObjectTypeSettings.BrowseName = new QualifiedName(ObjectTypeReader.Value, _typeNamespaceIndex);
                            //ObjectTypeSettings.IsAbstract = true;
                            if (ObjectTypeReader.ReadToFollowing("DisplayName"))
                            {
                                ObjectTypeReader.Read();
                                ObjectTypeSettings.DisplayName = ObjectTypeReader.Value;
                            }

                            ObjectTypeNode ObjectTypeNode = _nodeManager.CreateObjectTypeNode(_nodeManager.Server.DefaultRequestContext, ObjectTypeSettings);
                        }

                    }

                }

            }
        }

        private void UADataTypeNodeCreator(NodeId parentNode, string xmlFilePath)
        {
            using (FileStream DataType = new FileStream(xmlFilePath, FileMode.Open))
            {
                using (XmlReader DataTypeReader = XmlReader.Create(DataType))
                {
                    while (DataTypeReader.Read())
                    {


                        if (DataTypeReader.IsStartElement("UADataType"))
                        {
                            CreateDataTypeSettings DataTypeSettings = new CreateDataTypeSettings
                            {
                                ParentNodeId = parentNode,
                                ReferenceTypeId = ReferenceTypeIds.HasSubtype,

                            };

                            // Read attributes and set properties of objectSettings
                            if (DataTypeReader.MoveToAttribute("NodeId"))
                                DataTypeSettings.RequestedNodeId = new NodeId(GetNodeId(DataTypeReader.Value), _typeNamespaceIndex);
                            if (DataTypeReader.MoveToAttribute("BrowseName"))
                                DataTypeSettings.BrowseName = new QualifiedName(DataTypeReader.Value, _typeNamespaceIndex);
                            if (DataTypeReader.ReadToFollowing("DisplayName"))
                            {
                                DataTypeReader.Read();
                                DataTypeSettings.DisplayName = DataTypeReader.Value;
                            }

                            // ... (handle other attributes and create ObjectNode)
                            DataTypeNode DataTypeNode = _nodeManager.CreateDataTypeNode(_nodeManager.Server.DefaultRequestContext, DataTypeSettings);
                        }

                    }

                }

            }
        }

        private void UaMethodNodeCreator(string xmlFilePath)
        {
            using (FileStream Method = new FileStream(xmlFilePath, FileMode.Open))
            {
                using (XmlReader MethodReader = XmlReader.Create(Method))
                {
                    while (MethodReader.Read())
                    {

                        if (MethodReader.IsStartElement("UAMethod"))
                        {
                            CreateMethodSettings MethodSettings = new CreateMethodSettings
                            {
                                //     ParentNodeId = new NodeId("JobControl", _typeNamespaceIndex),
                                ReferenceTypeId = ReferenceTypeIds.HasComponent
                            };

                            // Read attributes and set properties of objectSettings
                            if (MethodReader.MoveToAttribute("ParentNodeId"))
                                MethodSettings.ParentNodeId = new NodeId(GetNodeId(MethodReader.Value), _typeNamespaceIndex);
                            if (MethodReader.MoveToAttribute("NodeId"))
                                MethodSettings.RequestedNodeId = new NodeId(GetNodeId(MethodReader.Value), _typeNamespaceIndex);
                            if (MethodReader.MoveToAttribute("BrowseName"))
                                MethodSettings.BrowseName = new QualifiedName(MethodReader.Value, _typeNamespaceIndex);
                            if (MethodReader.ReadToFollowing("DisplayName"))
                            {
                                MethodReader.Read();
                                MethodSettings.DisplayName = MethodReader.Value;
                            }

                                // ... (handle other attributes and create ObjectNode)
                                MethodNode MethodNode = _nodeManager.CreateMethod(_nodeManager.Server.DefaultRequestContext, MethodSettings);
                             _nodeManager.SetChildUserData(MethodSettings.ParentNodeId, MethodSettings.BrowseName, new ServerNodeManager.SystemFunction() { BrowseName = MethodReader.Value });


                        }

                    }

                }

            }
        }





        private void UaObjectNodeCreator(NodeId parentNode, string xmlFilePath)
        {
            using (FileStream Object = new FileStream(xmlFilePath, FileMode.Open))
            {
                using (XmlReader ObjectReader = XmlReader.Create(Object))
                {
                    while (ObjectReader.Read())
                    {
                        if (ObjectReader.IsStartElement("UAObject"))
                        {
                            CreateObjectSettings objectSettings = new CreateObjectSettings { };


                            // Read attributes and set properties of objectSettings
                            if (ObjectReader.MoveToAttribute("NodeId"))
                                objectSettings.RequestedNodeId = new NodeId(GetNodeId(ObjectReader.Value), _typeNamespaceIndex);
                            if (ObjectReader.MoveToAttribute("BrowseName"))
                                objectSettings.BrowseName = new QualifiedName(ObjectReader.Value, _typeNamespaceIndex);

                            if (ObjectReader.MoveToAttribute("ParentNodeId"))
                            {
                                objectSettings.ParentNodeId = new NodeId(GetNodeId(ObjectReader.Value), _typeNamespaceIndex);
                                objectSettings.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                            }
                            if (ObjectReader.ReadToFollowing("DisplayName"))
                            {
                                ObjectReader.Read();
                                objectSettings.DisplayName = ObjectReader.Value;
                            }

                            // ... (handle other attributes and create ObjectNode)
                            ObjectNode objectNode = _nodeManager.CreateObject(_nodeManager.Server.DefaultRequestContext, objectSettings);

                        }
                    }
                }
            }
        }
        private void UaVariableNodeCreator(string xmlFilePath)
        {
            using (FileStream Variable = new FileStream(xmlFilePath, FileMode.Open))
            {
                using (XmlReader VariableReader = XmlReader.Create(Variable))
                {
                    while (VariableReader.Read())
                    {
                        if (VariableReader.IsStartElement("UAVariable"))
                        {

                            CreateVariableSettings VariableSettings = new CreateVariableSettings 
                            {
                                ReferenceTypeId = ReferenceTypeIds.HasProperty
                            };
                            if (VariableReader.MoveToAttribute("NodeId"))
                                VariableSettings.RequestedNodeId = new NodeId(GetNodeId(VariableReader.Value), _typeNamespaceIndex);
                            if (VariableReader.MoveToAttribute("BrowseName"))
                                VariableSettings.BrowseName = new QualifiedName(VariableReader.Value, _typeNamespaceIndex);
                            if (VariableReader.MoveToAttribute("DataType"))
                                if (VariableReader.Value.Contains("ns="))
                                    VariableSettings.DataType = new NodeId(GetNodeId(VariableReader.Value), _typeNamespaceIndex);
                            if (VariableReader.MoveToAttribute("ParentNodeId"))
                                VariableSettings.ParentNodeId = new NodeId(GetNodeId(VariableReader.Value), _typeNamespaceIndex);
                            if (VariableReader.MoveToAttribute("AccessLevel"))
                            {
                                byte.TryParse(VariableReader.Value, out byte accessLevel);
                                VariableSettings.AccessLevel = accessLevel;
                            }
                            if (VariableReader.MoveToAttribute("ValueRank"))
                            {
                                int.TryParse(VariableReader.Value, out int valueRank);
                                VariableSettings.ValueRank = valueRank;
                            }
                            if (VariableReader.MoveToAttribute("ArrayDimensions"))
                            {
                                uint.TryParse(VariableReader.Value, out uint dimensionValue);
                                VariableSettings.ArrayDimensions = new uint[] { dimensionValue };
                            }
                            if (VariableReader.ReadToFollowing("DisplayName"))
                            {
                                VariableReader.Read();
                                VariableSettings.DisplayName = VariableReader.Value;
                            }

                            if (VariableReader.Read() && !(VariableReader.NodeType == XmlNodeType.EndElement && VariableReader.Name == "UAVariable"))
                            {
                                try
                                {
                                    // Attempt to read the Value element
                                    if (VariableReader.ReadToNextSibling ("Value")) // This throws an exception if the element is not found
                                    {
                                        Variant value = ReadValueFromXml(VariableReader);
                                        VariableSettings.Value = value;
                                    }
                                }
                                catch (XmlException ex)
                                {
                                    // Handle the case where the Value element is missing
                                    Console.WriteLine("Warning: Value element not found in UAVariable. Exception: " + ex.Message);

                                    // Optional: Set a default value for VariableSettings.Value
                                    // VariableSettings.Value = new Variant( ... ); 
                                }
                            }






                            VariableNode VariableNode = _nodeManager.CreateVariable(_nodeManager.Server.DefaultRequestContext, VariableSettings);
                        }
                    }

                }

            }
        }


        private Variant ReadValueFromXml(XmlReader reader)
        {
            List<Argument> arguments = new List<Argument>();

            //reader.ReadToFollowing("Value");
            reader.ReadToDescendant("ListOfExtensionObject");

            while (reader.Read() && reader.IsStartElement("ExtensionObject"))
            {
                Argument argument = new Argument();
                int extensionObjectDepth = 1; // Track the depth within the ExtensionObject

                while (reader.Read() && extensionObjectDepth > 0) // Loop until we exit the current ExtensionObject
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Name == "ExtensionObject")
                        {
                            extensionObjectDepth++; // Increment depth for nested elements
                        }
                        else if (reader.Name == "Name")
                        {
                            argument.Name = reader.ReadElementContentAsString();
                        }
                        else if (reader.Name == "DataType")
                        {
                            reader.ReadToDescendant("Identifier");
                            argument.DataType = new NodeId(GetNodeId(reader.ReadElementContentAsString()), 0);
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (reader.Name == "ExtensionObject")
                        {
                            extensionObjectDepth--; // Decrement depth for closing tags
                            if (extensionObjectDepth == 0) // If we're back at the top level
                            {
                                arguments.Add(argument); // Add argument and break inner loop
                                break;
                            }
                        }
                    }
                }
            }

            return new Variant(arguments.ToArray());
        }

        /*
         * private Variant ReadValueFromXml(XmlReader reader)
{
    List<Argument> arguments = new List<Argument>();

    // Check if we're at the start of the <Value> element
    if (!reader.IsStartElement("Value"))
    {
        reader.Read(); 
        return Variant.Null; 
    }

    reader.ReadToDescendant("ListOfExtensionObject");

    while (reader.ReadToFollowing("ExtensionObject")) // Read each ExtensionObject
    {
        Argument argument = new Argument();

        if (reader.ReadToDescendant("Body"))
        {
            while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement && reader.Name == "Body")) // Check if Body is finished
            {
                if (reader.IsStartElement("Argument"))
                {
                    if (reader.ReadToFollowing("Name"))
                    {
                        argument.Name = reader.ReadElementContentAsString();
                    }
                    if (reader.ReadToFollowing("DataType"))
                    {
                        if (reader.ReadToFollowing("Identifier"))
                        {
                            argument.DataType = new NodeId(GetNodeId(reader.ReadElementContentAsString()), _typeNamespaceIndex);
                        }
                    }
                    if (reader.ReadToFollowing("ValueRank"))
                    {
                        argument.ValueRank = reader.ReadElementContentAsInt();
                    }
                    // Read ArrayDimensions Correctly
                    if (reader.ReadToFollowing("ArrayDimensions"))
                    {
                        List<uint> arrayDimensions = new List<uint>();

                        // Check if the element is empty before proceeding
                        if (!reader.IsEmptyElement)
                        {
                            reader.ReadStartElement("ArrayDimensions"); // Move inside the element

                            // Read UInt32 Values
                            while (reader.Read() && !(reader.NodeType == XmlNodeType.EndElement && reader.Name == "ArrayDimensions"))
                            {
                                if (reader.IsStartElement("UInt32"))
                                {
                                    uint dimensionValue = 0;
                                    if (uint.TryParse(reader.ReadElementContentAsString(), out dimensionValue))
                                    {
                                        arrayDimensions.Add(dimensionValue);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Warning: Invalid UInt32 value in ArrayDimensions.");
                                    }
                                }
                            }
                        }

                        argument.ArrayDimensions = arrayDimensions.ToArray();
                    }
                    // Read description, if present
                    if (reader.ReadToFollowing("Description"))
                    {
                        if (reader.ReadToFollowing("Text"))
                        {
                            argument.Description = new LocalizedText(reader.ReadElementContentAsString());
                        }
                    }
                    // Read and set the Value property (replace with your actual logic)
                    if (reader.ReadToFollowing("Value"))
                    {
                        // Read the value based on DataType (replace with your actual logic)
                        if (argument.DataType == new NodeId("i=12"))
                        {
                            argument.Value = new Variant(reader.ReadElementContentAsString()); // String
                        }
                        // Add cases for other data types here 
                    }
                }
            }

            arguments.Add(argument);
        }

    }

    return new Variant(arguments.ToArray());
}

         * */











        private string GetNodeId(string nodeString)
        {
            // Split only if the string contains "ns=" 
            var parts = nodeString.Contains("ns=") ? nodeString.Split(';') : new[] { nodeString };

            foreach (var part in parts)
            {
                if (part.StartsWith("i="))
                {
                    string numericValue = part.Substring(2);
                    return numericValue;
                }
            }
            // Default return value if no match is found
            return "";
        }
        


        

        


        


        private void ElementToReference(string xmlFilePath)
        {
            string _referenceNodeId = "";
            string referenceTypeValue = "";
            using (FileStream referenceReaderFile = new FileStream(xmlFilePath, FileMode.Open))
            using (XmlReader referenceReader = XmlReader.Create(referenceReaderFile))

            {
                while (referenceReader.Read())
                {
                    if (referenceReader.IsStartElement("UAObjectType") ||
                        referenceReader.IsStartElement("UADataType") ||
                        referenceReader.IsStartElement("UAMethod") ||
                        referenceReader.IsStartElement("UAObject"))
                    {
                        if (referenceReader.MoveToAttribute("NodeId"))
                        {
                            NodeId _sourceNodeId = new NodeId(GetNodeId(referenceReader.Value), _typeNamespaceIndex);

                            // Read to the <References> start element
                            referenceReader.ReadToFollowing("References");

                            while (referenceReader.Read())
                            {
                                if (referenceReader.IsStartElement("Reference"))
                                {
                                    var MyReferenceNode = new ReferenceNode();

                                    if (referenceReader.MoveToAttribute("ReferenceType"))
                                    {
                                        referenceTypeValue = referenceReader.Value;
                                        
                                        switch (referenceTypeValue)
                                        {
                                            case "HasProperty":
                                                MyReferenceNode.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                                                break;
                                            case "ToState":
                                                MyReferenceNode.ReferenceTypeId = ReferenceTypeIds.ToState;
                                                break;
                                            case "FromState":
                                                MyReferenceNode.ReferenceTypeId = ReferenceTypeIds.FromState;
                                                break;
                                            case "HasTypeDefinition":
                                                MyReferenceNode.ReferenceTypeId = ReferenceTypeIds.HasTypeDefinition;
                                                break;
                                            case "HasComponent":
                                                MyReferenceNode.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                                                break;
                                            case "HasEncoding":
                                                MyReferenceNode.ReferenceTypeId = ReferenceTypeIds.HasEncoding;
                                                break;
                                            case "HasSubtype":
                                                MyReferenceNode.ReferenceTypeId = ReferenceTypeIds.HasSubtype;
                                                break;
                                            case "HasEffect":
                                                MyReferenceNode.ReferenceTypeId = ReferenceTypeIds.HasEffect;
                                                break;
                                            case "HasModellingRule":
                                                MyReferenceNode.ReferenceTypeId = ReferenceTypeIds.HasModellingRule;
                                                break;
                                            case "HasCause":
                                                MyReferenceNode.ReferenceTypeId = ReferenceTypeIds.HasCause ;
                                                break;
                                            case "HasDescription":
                                                MyReferenceNode.ReferenceTypeId = ReferenceTypeIds.HasDescription ;
                                                break;
                                            case "i=41":
                                                MyReferenceNode.ReferenceTypeId = ReferenceTypeIds.GeneratesEvent;
                                                break;
                                            case "i=117":
                                                MyReferenceNode.ReferenceTypeId = ReferenceTypeIds.HasSubStateMachine;
                                                break;
                                            default:
                                                break;
                                        }
                                    }

                                    referenceReader.Read();
                                    _referenceNodeId = referenceReader.Value;

                                    MyReferenceNode.TargetId = new NodeId(GetNodeId(_referenceNodeId), _typeNamespaceIndex);
                                    _nodeManager.AddReference(_nodeManager.Server.DefaultRequestContext, _sourceNodeId, MyReferenceNode.ReferenceTypeId, true,  MyReferenceNode.TargetId, false);
                                }
                                else if (referenceReader.NodeType == XmlNodeType.EndElement && referenceReader.Name == "References")
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }




        


    }
}

