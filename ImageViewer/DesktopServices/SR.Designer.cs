//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.ImageViewer.DesktopServices {
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
    public class SR {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SR() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ClearCanvas.ImageViewer.DesktopServices.SR", typeof(SR).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 1 study has failed to load..
        /// </summary>
        public static string MessageFormatStudyLoadFailure {
            get {
                return ResourceManager.GetString("MessageFormatStudyLoadFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} studies failed to load..
        /// </summary>
        public static string MessageFormatStudyLoadFailures {
            get {
                return ResourceManager.GetString("MessageFormatStudyLoadFailures", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 1 study had one or more images that failed to load..
        /// </summary>
        public static string MessagePartialStudyLoadFailure {
            get {
                return ResourceManager.GetString("MessagePartialStudyLoadFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} studies had one or more images that failed to load..
        /// </summary>
        public static string MessagePartialStudyLoadFailures {
            get {
                return ResourceManager.GetString("MessagePartialStudyLoadFailures", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Please see the logs for more details..
        /// </summary>
        public static string MessagePleaseSeeLogs {
            get {
                return ResourceManager.GetString("MessagePleaseSeeLogs", resourceCulture);
            }
        }
    }
}
