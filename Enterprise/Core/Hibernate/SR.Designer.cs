﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.42
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.Enterprise.Data.Hibernate {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SR {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SR() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ClearCanvas.Enterprise.Data.Hibernate.SR", typeof(SR).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error closing context.
        /// </summary>
        internal static string ExceptionCloseContext {
            get {
                return ResourceManager.GetString("ExceptionCloseContext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Commit failed - transaction aborted.
        /// </summary>
        internal static string ExceptionCommitFailure {
            get {
                return ResourceManager.GetString("ExceptionCommitFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The entity is not in the persistence context.
        /// </summary>
        internal static string ExceptionEntityNotInContext {
            get {
                return ResourceManager.GetString("ExceptionEntityNotInContext", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No current transaction.
        /// </summary>
        internal static string ExceptionNoCurrentTransaction {
            get {
                return ResourceManager.GetString("ExceptionNoCurrentTransaction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error resuming context.
        /// </summary>
        internal static string ExceptionResumeContext {
            get {
                return ResourceManager.GetString("ExceptionResumeContext", resourceCulture);
            }
        }
    }
}
