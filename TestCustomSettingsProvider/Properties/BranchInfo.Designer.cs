﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestCustomSettingsProvider.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.4.0.0")]
    internal sealed partial class BranchInfo : global::System.Configuration.ApplicationSettingsBase {
        
        private static BranchInfo defaultInstance = ((BranchInfo)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new BranchInfo())));
        
        public static BranchInfo Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Configuration.SettingsProviderAttribute(typeof(CustomSettingsProvider))]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("\"\"")]
        [global::System.Configuration.SettingsManageabilityAttribute(global::System.Configuration.SettingsManageability.Roaming)]
        public global::System.Collections.Generic.List<Branch> DKBranches {
            get {
                return ((global::System.Collections.Generic.List<Branch>)(this["DKBranches"]));
            }
            set {
                this["DKBranches"] = value;
            }
        }
    }
}
