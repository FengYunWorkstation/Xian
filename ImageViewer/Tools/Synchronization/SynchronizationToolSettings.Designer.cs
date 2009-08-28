﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.ImageViewer.Tools.Synchronization {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "9.0.0.0")]
    internal sealed partial class SynchronizationToolSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static SynchronizationToolSettings defaultInstance = ((SynchronizationToolSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new SynchronizationToolSettings())));
        
        public static SynchronizationToolSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        /// <summary>
        /// Toggles whether or not reference lines should be shown for first and last slices in a stack.
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Toggles whether or not reference lines should be shown for first and last slices " +
            "in a stack.")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool ShowFirstAndLastReferenceLines {
            get {
                return ((bool)(this["ShowFirstAndLastReferenceLines"]));
            }
            set {
                this["ShowFirstAndLastReferenceLines"] = value;
            }
        }
        
        /// <summary>
        /// Maximum angle difference, in degrees, between two planes for synchronization tools to treat the planes as parallel.
        /// </summary>
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Maximum angle difference, in degrees, between two planes for synchronization tool" +
            "s to treat the planes as parallel.")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public float ParallelPlanesToleranceAngle {
            get {
                return ((float)(this["ParallelPlanesToleranceAngle"]));
            }
            set {
                this["ParallelPlanesToleranceAngle"] = value;
            }
        }
    }
}
