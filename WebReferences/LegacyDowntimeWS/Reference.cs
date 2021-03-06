﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 2.0.50727.4927.
// 
#pragma warning disable 1591

namespace SE.MESCC.DAL.WebReferences.LegacyDowntimeWS {
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    using SE.MESCC.Settings;
    
    
    /// <remarks/>
    // CODEGEN: The optional WSDL extension element 'method' from namespace 'http://www.w3.org/2000/wsdl/suds' was not handled.
    // CODEGEN: The optional WSDL extension element 'method' from namespace 'http://www.w3.org/2000/wsdl/suds' was not handled.
    // CODEGEN: The optional WSDL extension element 'method' from namespace 'http://www.w3.org/2000/wsdl/suds' was not handled.
    // CODEGEN: The optional WSDL extension element 'class' from namespace 'http://www.w3.org/2000/wsdl/suds' was not handled.
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="DowntimeWebServiceBinding", Namespace="http://schemas.microsoft.com/clr/nsassem/Citect.Ampla.Downtime.Server/Citect.Ampl" +
        "a.Downtime.Server%2C%20Version%3D4.2.4629.0%2C%20Culture%3Dneutral%2C%20PublicKe" +
        "yToken%3D13aaee2494f61799")]
    public partial class DowntimeWebServiceBinding : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback AboutOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetChartOperationCompleted;
        
        private System.Threading.SendOrPostCallback SplitRecordOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public DowntimeWebServiceBinding() {
            this.Url = AmplaSettings.legacyDowntimeWS_DowntimeWebServiceService_LOCALHOST;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event AboutCompletedEventHandler AboutCompleted;
        
        /// <remarks/>
        public event GetChartCompletedEventHandler GetChartCompleted;
        
        /// <remarks/>
        public event SplitRecordCompletedEventHandler SplitRecordCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://schemas.microsoft.com/clr/nsassem/Citect.Ampla.Downtime.Server.DowntimeWeb" +
            "Service/Citect.Ampla.Downtime.Server#About", RequestNamespace="http://schemas.microsoft.com/clr/nsassem/Citect.Ampla.Downtime.Server.DowntimeWeb" +
            "Service/Citect.Ampla.Downtime.Server", ResponseNamespace="http://schemas.microsoft.com/clr/nsassem/Citect.Ampla.Downtime.Server.DowntimeWeb" +
            "Service/Citect.Ampla.Downtime.Server")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public string About() {
            object[] results = this.Invoke("About", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void AboutAsync() {
            this.AboutAsync(null);
        }
        
        /// <remarks/>
        public void AboutAsync(object userState) {
            if ((this.AboutOperationCompleted == null)) {
                this.AboutOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAboutOperationCompleted);
            }
            this.InvokeAsync("About", new object[0], this.AboutOperationCompleted, userState);
        }
        
        private void OnAboutOperationCompleted(object arg) {
            if ((this.AboutCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AboutCompleted(this, new AboutCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://schemas.microsoft.com/clr/nsassem/Citect.Ampla.Downtime.Server.DowntimeWeb" +
            "Service/Citect.Ampla.Downtime.Server#GetChart", RequestNamespace="http://schemas.microsoft.com/clr/nsassem/Citect.Ampla.Downtime.Server.DowntimeWeb" +
            "Service/Citect.Ampla.Downtime.Server", ResponseNamespace="http://schemas.microsoft.com/clr/nsassem/Citect.Ampla.Downtime.Server.DowntimeWeb" +
            "Service/Citect.Ampla.Downtime.Server")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public byte[] GetChart(string guid, string view, string filterXml, string chartOptionsXml) {
            object[] results = this.Invoke("GetChart", new object[] {
                        guid,
                        view,
                        filterXml,
                        chartOptionsXml});
            return ((byte[])(results[0]));
        }
        
        /// <remarks/>
        public void GetChartAsync(string guid, string view, string filterXml, string chartOptionsXml) {
            this.GetChartAsync(guid, view, filterXml, chartOptionsXml, null);
        }
        
        /// <remarks/>
        public void GetChartAsync(string guid, string view, string filterXml, string chartOptionsXml, object userState) {
            if ((this.GetChartOperationCompleted == null)) {
                this.GetChartOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetChartOperationCompleted);
            }
            this.InvokeAsync("GetChart", new object[] {
                        guid,
                        view,
                        filterXml,
                        chartOptionsXml}, this.GetChartOperationCompleted, userState);
        }
        
        private void OnGetChartOperationCompleted(object arg) {
            if ((this.GetChartCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetChartCompleted(this, new GetChartCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://schemas.microsoft.com/clr/nsassem/Citect.Ampla.Downtime.Server.DowntimeWeb" +
            "Service/Citect.Ampla.Downtime.Server#SplitRecord", RequestNamespace="http://schemas.microsoft.com/clr/nsassem/Citect.Ampla.Downtime.Server.DowntimeWeb" +
            "Service/Citect.Ampla.Downtime.Server", ResponseNamespace="http://schemas.microsoft.com/clr/nsassem/Citect.Ampla.Downtime.Server.DowntimeWeb" +
            "Service/Citect.Ampla.Downtime.Server")]
        public void SplitRecord(string guid, string splitTime, string filter) {
            this.Invoke("SplitRecord", new object[] {
                        guid,
                        splitTime,
                        filter});
        }
        
        /// <remarks/>
        public void SplitRecordAsync(string guid, string splitTime, string filter) {
            this.SplitRecordAsync(guid, splitTime, filter, null);
        }
        
        /// <remarks/>
        public void SplitRecordAsync(string guid, string splitTime, string filter, object userState) {
            if ((this.SplitRecordOperationCompleted == null)) {
                this.SplitRecordOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSplitRecordOperationCompleted);
            }
            this.InvokeAsync("SplitRecord", new object[] {
                        guid,
                        splitTime,
                        filter}, this.SplitRecordOperationCompleted, userState);
        }
        
        private void OnSplitRecordOperationCompleted(object arg) {
            if ((this.SplitRecordCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SplitRecordCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    // CODEGEN: The optional WSDL extension element 'method' from namespace 'http://www.w3.org/2000/wsdl/suds' was not handled.
    // CODEGEN: The optional WSDL extension element 'class' from namespace 'http://www.w3.org/2000/wsdl/suds' was not handled.
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="WebServiceBaseBinding", Namespace="http://schemas.microsoft.com/clr/nsassem/Citect.Ampla.Downtime.Server/Citect.Ampl" +
        "a.Downtime.Server%2C%20Version%3D4.2.4629.0%2C%20Culture%3Dneutral%2C%20PublicKe" +
        "yToken%3D13aaee2494f61799")]
    public partial class WebServiceBaseBinding : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback AboutOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public WebServiceBaseBinding() {
            this.Url =AmplaSettings.legacyDowntimeWS_DowntimeWebServiceService_LOCALHOST;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event AboutCompletedEventHandler AboutCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapRpcMethodAttribute("http://schemas.microsoft.com/clr/nsassem/Citect.Ampla.General.Server.WebServiceBa" +
            "se/Citect.Ampla.General.Server#About", RequestNamespace="http://schemas.microsoft.com/clr/nsassem/Citect.Ampla.General.Server.WebServiceBa" +
            "se/Citect.Ampla.General.Server", ResponseNamespace="http://schemas.microsoft.com/clr/nsassem/Citect.Ampla.General.Server.WebServiceBa" +
            "se/Citect.Ampla.General.Server")]
        [return: System.Xml.Serialization.SoapElementAttribute("return")]
        public string About() {
            object[] results = this.Invoke("About", new object[0]);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void AboutAsync() {
            this.AboutAsync(null);
        }
        
        /// <remarks/>
        public void AboutAsync(object userState) {
            if ((this.AboutOperationCompleted == null)) {
                this.AboutOperationCompleted = new System.Threading.SendOrPostCallback(this.OnAboutOperationCompleted);
            }
            this.InvokeAsync("About", new object[0], this.AboutOperationCompleted, userState);
        }
        
        private void OnAboutOperationCompleted(object arg) {
            if ((this.AboutCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.AboutCompleted(this, new AboutCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    public delegate void AboutCompletedEventHandler(object sender, AboutCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class AboutCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal AboutCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    public delegate void GetChartCompletedEventHandler(object sender, GetChartCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetChartCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetChartCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public byte[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((byte[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.4927")]
    public delegate void SplitRecordCompletedEventHandler(object sender, System.ComponentModel.AsyncCompletedEventArgs e);
}

#pragma warning restore 1591