﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AlfredPlayer.Properties {
    
    
    [CompilerGenerated()]
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("PlayerJack")]
        public string ServerName {
            get {
                return ((string)(this["ServerName"]));
            }
            set {
                this["ServerName"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("nam.kicks-ass.org")]
        public string ServerHost {
            get {
                return ((string)(this["ServerHost"]));
            }
            set {
                this["ServerHost"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("13100")]
        public int ServerPort {
            get {
                return ((int)(this["ServerPort"]));
            }
            set {
                this["ServerPort"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("C:\\Program Files (x86)\\VideoLAN\\VLC\\vlc.exe")]
        public string VlcPath {
            get {
                return ((string)(this["VlcPath"]));
            }
            set {
                this["VlcPath"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("Guigui")]
        public string ServerLogin {
            get {
                return ((string)(this["ServerLogin"]));
            }
            set {
                this["ServerLogin"] = value;
            }
        }
        
        [UserScopedSetting()]
        [DebuggerNonUserCode()]
        [DefaultSettingValue("Ghiltoniel1")]
        public string ServerPassword {
            get {
                return ((string)(this["ServerPassword"]));
            }
            set {
                this["ServerPassword"] = value;
            }
        }
    }
}