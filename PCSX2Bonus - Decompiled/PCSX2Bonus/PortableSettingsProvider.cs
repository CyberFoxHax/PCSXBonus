namespace PCSX2Bonus
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.IO;
    using System.Windows.Forms;
    using System.Xml;

    public sealed class PortableSettingsProvider : SettingsProvider
    {
        private XmlDocument _settingsXML;
        private const string SETTINGSROOT = "Settings";

        public virtual string GetAppSettingsFilename()
        {
            return (this.ApplicationName + ".settings");
        }

        public virtual string GetAppSettingsPath()
        {
            FileInfo info = new FileInfo(Application.ExecutablePath);
            return Path.Combine(info.DirectoryName, "PCSX2Bonus");
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
        {
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();
            foreach (SettingsProperty property in props)
            {
                SettingsPropertyValue value2 = new SettingsPropertyValue(property) {
                    IsDirty = false,
                    SerializedValue = this.GetValue(property)
                };
                values.Add(value2);
            }
            return values;
        }

        private string GetValue(SettingsProperty setting)
        {
            try
            {
                return this.SettingsXML.SelectSingleNode("Settings/" + setting.Name).InnerText;
            }
            catch (Exception)
            {
                if (setting.DefaultValue != null)
                {
                    return setting.DefaultValue.ToString();
                }
                return "";
            }
        }

        public override void Initialize(string name, NameValueCollection col)
        {
            base.Initialize(this.ApplicationName, col);
        }

        private bool IsRoaming(SettingsProperty prop)
        {
            foreach (DictionaryEntry entry in prop.Attributes)
            {
                Attribute attribute = (Attribute) entry.Value;
                if (attribute is SettingsManageabilityAttribute)
                {
                    return true;
                }
            }
            return false;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection propvals)
        {
            foreach (SettingsPropertyValue value2 in propvals)
            {
                this.SetValue(value2);
            }
            try
            {
                this.SettingsXML.Save(Path.Combine(this.GetAppSettingsPath(), this.GetAppSettingsFilename()));
            }
            catch (Exception)
            {
            }
        }

        private void SetValue(SettingsPropertyValue propVal)
        {
            XmlElement newChild = null;
            try
            {
                newChild = (XmlElement) this.SettingsXML.SelectSingleNode("Settings/" + propVal.Name);
            }
            catch (Exception)
            {
                newChild = null;
            }
            if (newChild != null)
            {
                newChild.InnerText = propVal.SerializedValue.ToString();
            }
            else
            {
                newChild = this.SettingsXML.CreateElement(propVal.Name);
                newChild.InnerText = propVal.SerializedValue.ToString();
                this.SettingsXML.SelectSingleNode("Settings").AppendChild(newChild);
            }
        }

        public override string ApplicationName
        {
            get
            {
                if (Application.ProductName.Trim().Length > 0)
                {
                    return Application.ProductName;
                }
                FileInfo info = new FileInfo(Application.ExecutablePath);
                return info.Name.Substring(0, info.Name.Length - info.Extension.Length);
            }
            set
            {
            }
        }

        public override string Name
        {
            get
            {
                return "PortableSettingsProvider";
            }
        }

        private XmlDocument SettingsXML
        {
            get
            {
                if (this._settingsXML == null)
                {
                    this._settingsXML = new XmlDocument();
                    try
                    {
                        this._settingsXML.Load(Path.Combine(this.GetAppSettingsPath(), this.GetAppSettingsFilename()));
                    }
                    catch (Exception)
                    {
                        XmlDeclaration newChild = this._settingsXML.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
                        this._settingsXML.AppendChild(newChild);
                        XmlNode node = null;
                        node = this._settingsXML.CreateNode(XmlNodeType.Element, "Settings", "");
                        this._settingsXML.AppendChild(node);
                    }
                }
                return this._settingsXML;
            }
        }
    }
}

