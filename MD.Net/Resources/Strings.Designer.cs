﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MD.Net.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MD.Net.Resources.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to Creating track: {0}.
        /// </summary>
        internal static string AddTrackAction_Description {
            get {
                return ResourceManager.GetString("AddTrackAction.Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The source disc state was unexpected, cannot update..
        /// </summary>
        internal static string Error_UnexpectedDisc {
            get {
                return ResourceManager.GetString("Error.UnexpectedDisc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Encoding track: {0}.
        /// </summary>
        internal static string FormatManager_Description {
            get {
                return ResourceManager.GetString("FormatManager.Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deleting track: {0}.
        /// </summary>
        internal static string RemoveTrackAction_Description {
            get {
                return ResourceManager.GetString("RemoveTrackAction.Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to OK..
        /// </summary>
        internal static string Result_Success {
            get {
                return ResourceManager.GetString("Result.Success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown error..
        /// </summary>
        internal static string Result_UnknownError {
            get {
                return ResourceManager.GetString("Result.UnknownError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tool process &quot;{0} {1}&quot; exited with code {2}: {3}.
        /// </summary>
        internal static string ToolException_Message {
            get {
                return ResourceManager.GetString("ToolException.Message", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unknown error..
        /// </summary>
        internal static string ToolException_UnknownError {
            get {
                return ResourceManager.GetString("ToolException.UnknownError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Renaming disc: {0}.
        /// </summary>
        internal static string UpdateDiscTitleAction_Description {
            get {
                return ResourceManager.GetString("UpdateDiscTitleAction.Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Renaming track: {0}.
        /// </summary>
        internal static string UpdateTrackNameAction_Description {
            get {
                return ResourceManager.GetString("UpdateTrackNameAction.Description", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File &quot;{0}&quot; is unsupported. Expected WAVE/ATRAC 44.1kHz/16 bit/Stereo..
        /// </summary>
        internal static string WaveFormatException_Message {
            get {
                return ResourceManager.GetString("WaveFormatException.Message", resourceCulture);
            }
        }
    }
}
