﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.Ris.Client {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class PrintReportComponentSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static PrintReportComponentSettings defaultInstance = ((PrintReportComponentSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new PrintReportComponentSettings())));
        
        public static PrintReportComponentSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CloseOnPrint {
            get {
                return ((bool)(this["CloseOnPrint"]));
            }
            set {
                this["CloseOnPrint"] = value;
            }
        }
    }
}
