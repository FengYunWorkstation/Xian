﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.ImageViewer.Annotations.Dicom {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class DicomFilteredAnnotationLayoutStoreSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static DicomFilteredAnnotationLayoutStoreSettings defaultInstance = ((DicomFilteredAnnotationLayoutStoreSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new DicomFilteredAnnotationLayoutStoreSettings())));
        
        public static DicomFilteredAnnotationLayoutStoreSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DicomFilteredAnnotationLayoutStoreDefaults.xml")]
        public string FilteredLayoutSettingsXml {
            get {
                return ((string)(this["FilteredLayoutSettingsXml"]));
            }
            set {
                this["FilteredLayoutSettingsXml"] = value;
            }
        }
    }
}
