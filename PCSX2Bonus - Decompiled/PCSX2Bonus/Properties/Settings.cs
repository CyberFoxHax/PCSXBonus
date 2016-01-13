namespace PCSX2Bonus.Properties
{
    using PCSX2Bonus;
    using System;
    using System.CodeDom.Compiler;
    using System.Configuration;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Windows;

    [CompilerGenerated, GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed class Settings : ApplicationSettingsBase
    {
        private static Settings defaultInstance = ((Settings) SettingsBase.Synchronized(new Settings()));

        [DebuggerNonUserCode, DefaultSettingValue("1"), UserScopedSetting, SettingsProvider(typeof(PortableSettingsProvider))]
        public int buttonCancel
        {
            get
            {
                return (int) this["buttonCancel"];
            }
            set
            {
                this["buttonCancel"] = value;
            }
        }

        [DefaultSettingValue("0"), DebuggerNonUserCode, UserScopedSetting, SettingsProvider(typeof(PortableSettingsProvider))]
        public int buttonOk
        {
            get
            {
                return (int) this["buttonOk"];
            }
            set
            {
                this["buttonOk"] = value;
            }
        }

        public static Settings Default
        {
            get
            {
                return defaultInstance;
            }
        }

        [SettingsProvider(typeof(PortableSettingsProvider)), DefaultSettingValue("Default"), DebuggerNonUserCode, UserScopedSetting]
        public string defaultSort
        {
            get
            {
                return (string) this["defaultSort"];
            }
            set
            {
                this["defaultSort"] = value;
            }
        }

        [UserScopedSetting, SettingsProvider(typeof(PortableSettingsProvider)), DebuggerNonUserCode, DefaultSettingValue("default.xml")]
        public string defaultTheme
        {
            get
            {
                return (string) this["defaultTheme"];
            }
            set
            {
                this["defaultTheme"] = value;
            }
        }

        [SettingsProvider(typeof(PortableSettingsProvider)), DefaultSettingValue("Stacked"), UserScopedSetting, DebuggerNonUserCode]
        public string defaultView
        {
            get
            {
                return (string) this["defaultView"];
            }
            set
            {
                this["defaultView"] = value;
            }
        }

        [UserScopedSetting, DebuggerNonUserCode, DefaultSettingValue("False"), SettingsProvider(typeof(PortableSettingsProvider))]
        public bool enableGamepad
        {
            get
            {
                return (bool) this["enableGamepad"];
            }
            set
            {
                this["enableGamepad"] = value;
            }
        }

        [DefaultSettingValue("True"), UserScopedSetting, SettingsProvider(typeof(PortableSettingsProvider)), DebuggerNonUserCode]
        public bool enableGameToast
        {
            get
            {
                return (bool) this["enableGameToast"];
            }
            set
            {
                this["enableGameToast"] = value;
            }
        }

        [UserScopedSetting, DefaultSettingValue(""), SettingsProvider(typeof(PortableSettingsProvider)), DebuggerNonUserCode]
        public string pcsx2DataDir
        {
            get
            {
                return (string) this["pcsx2DataDir"];
            }
            set
            {
                this["pcsx2DataDir"] = value;
            }
        }

        [DefaultSettingValue(""), UserScopedSetting, SettingsProvider(typeof(PortableSettingsProvider)), DebuggerNonUserCode]
        public string pcsx2Dir
        {
            get
            {
                return (string) this["pcsx2Dir"];
            }
            set
            {
                this["pcsx2Dir"] = value;
            }
        }

        [DebuggerNonUserCode, DefaultSettingValue(""), UserScopedSetting, SettingsProvider(typeof(PortableSettingsProvider))]
        public string pcsx2Exe
        {
            get
            {
                return (string) this["pcsx2Exe"];
            }
            set
            {
                this["pcsx2Exe"] = value;
            }
        }

        [UserScopedSetting, SettingsProvider(typeof(PortableSettingsProvider)), DebuggerNonUserCode, DefaultSettingValue("True")]
        public bool saveInfo
        {
            get
            {
                return (bool) this["saveInfo"];
            }
            set
            {
                this["saveInfo"] = value;
            }
        }

        [SettingsProvider(typeof(PortableSettingsProvider)), DebuggerNonUserCode, DefaultSettingValue("False"), UserScopedSetting]
        public bool saveScreenshots
        {
            get
            {
                return (bool) this["saveScreenshots"];
            }
            set
            {
                this["saveScreenshots"] = value;
            }
        }

        [DefaultSettingValue("False"), UserScopedSetting, SettingsProvider(typeof(PortableSettingsProvider)), DebuggerNonUserCode]
        public bool useUpdatedCompat
        {
            get
            {
                return (bool) this["useUpdatedCompat"];
            }
            set
            {
                this["useUpdatedCompat"] = value;
            }
        }

        [SettingsProvider(typeof(PortableSettingsProvider)), UserScopedSetting, DefaultSettingValue("0,0"), DebuggerNonUserCode]
        public Point windowLocation
        {
            get
            {
                return (Point) this["windowLocation"];
            }
            set
            {
                this["windowLocation"] = value;
            }
        }

        [DebuggerNonUserCode, UserScopedSetting, SettingsProvider(typeof(PortableSettingsProvider)), DefaultSettingValue("925,522")]
        public Size windowSize
        {
            get
            {
                return (Size) this["windowSize"];
            }
            set
            {
                this["windowSize"] = value;
            }
        }

        [UserScopedSetting, DefaultSettingValue("Normal"), SettingsProvider(typeof(PortableSettingsProvider)), DebuggerNonUserCode]
        public string windowState
        {
            get
            {
                return (string) this["windowState"];
            }
            set
            {
                this["windowState"] = value;
            }
        }
    }
}

