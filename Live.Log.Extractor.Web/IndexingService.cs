﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Live.Log.Extractor.IndexerService
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="IndexInformation", Namespace="http://schemas.datacontract.org/2004/07/Live.Log.Extractor.IndexerService")]
    public partial class IndexInformation : object, System.Runtime.Serialization.IExtensibleDataObject
    {
        
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private string DateField;
        
        private string FilePathField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Date
        {
            get
            {
                return this.DateField;
            }
            set
            {
                this.DateField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FilePath
        {
            get
            {
                return this.FilePathField;
            }
            set
            {
                this.FilePathField = value;
            }
        }
    }
}


[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="IService")]
public interface IService
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/CreateIndexFiles", ReplyAction="http://tempuri.org/IService/CreateIndexFilesResponse")]
    bool CreateIndexFiles(Live.Log.Extractor.Domain.ProductType product);
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/SearchIndex", ReplyAction="http://tempuri.org/IService/SearchIndexResponse")]
    Live.Log.Extractor.IndexerService.IndexInformation[] SearchIndex(Live.Log.Extractor.Domain.ProductType product, string searchText);
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface IServiceChannel : IService, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class ServiceClient : System.ServiceModel.ClientBase<IService>, IService
{
    
    public ServiceClient()
    {
    }
    
    public ServiceClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public ServiceClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public ServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public ServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public bool CreateIndexFiles(Live.Log.Extractor.Domain.ProductType product)
    {
        return base.Channel.CreateIndexFiles(product);
    }
    
    public Live.Log.Extractor.IndexerService.IndexInformation[] SearchIndex(Live.Log.Extractor.Domain.ProductType product, string searchText)
    {
        return base.Channel.SearchIndex(product, searchText);
    }
}