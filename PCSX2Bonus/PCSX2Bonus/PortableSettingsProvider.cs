using System.Linq;

namespace PCSX2Bonus {
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Configuration;
	using System.IO;
	using System.Windows.Forms;
	using System.Xml;

	public sealed class PortableSettingsProvider : SettingsProvider {
		private XmlDocument _settingsXML;
		private const string SETTINGSROOT = "Settings";

		public string GetAppSettingsFilename() {
			return (ApplicationName + ".settings");
		}

		public string GetAppSettingsPath() {
			var info = new FileInfo(Application.ExecutablePath);
			return Path.Combine(info.DirectoryName, "PCSX2Bonus");
		}

		public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props) {
			var values = new SettingsPropertyValueCollection();
			foreach (SettingsProperty property in props) {
				var value2 = new SettingsPropertyValue(property) {
					IsDirty = false,
					SerializedValue = GetValue(property)
				};
				values.Add(value2);
			}
			return values;
		}

		private string GetValue(SettingsProperty setting) {
			var selectSingleNode = SettingsXML.SelectSingleNode("Settings/" + setting.Name);
			if (selectSingleNode != null)
				return selectSingleNode.InnerText;
			return setting.DefaultValue != null ? setting.DefaultValue.ToString() : "";
		}

		public override void Initialize(string name, NameValueCollection col) {
			base.Initialize(ApplicationName, col);
		}

		private bool IsRoaming(SettingsProperty prop) {
			return (from DictionaryEntry entry in prop.Attributes select (Attribute)entry.Value).OfType<SettingsManageabilityAttribute>().Any();
		}

		public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection propvals) {
			foreach (SettingsPropertyValue value2 in propvals) {
				SetValue(value2);
			}
			try {
				SettingsXML.Save(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
			}
			catch (Exception) {
			}
		}

		private void SetValue(SettingsPropertyValue propVal) {
			XmlElement newChild = null;
			try {
				newChild = (XmlElement)SettingsXML.SelectSingleNode("Settings/" + propVal.Name);
			}
			catch (Exception) {
				newChild = null;
			}
			if (newChild != null) {
				newChild.InnerText = propVal.SerializedValue.ToString();
			}
			else {
				newChild = SettingsXML.CreateElement(propVal.Name);
				newChild.InnerText = propVal.SerializedValue.ToString();
				var selectSingleNode = SettingsXML.SelectSingleNode("Settings");
				if (selectSingleNode != null)
					selectSingleNode.AppendChild(newChild);
			}
		}

		public override string ApplicationName {
			get {
				if (Application.ProductName.Trim().Length > 0) {
					return Application.ProductName;
				}
				var info = new FileInfo(Application.ExecutablePath);
				return info.Name.Substring(0, info.Name.Length - info.Extension.Length);
			}
			set {
			}
		}

		public override string Name {
			get {
				return "PortableSettingsProvider";
			}
		}

		private XmlDocument SettingsXML {
			get {
				if (_settingsXML != null) return _settingsXML;
				_settingsXML = new XmlDocument();
				try {
					_settingsXML.Load(Path.Combine(GetAppSettingsPath(), GetAppSettingsFilename()));
				}
				catch (Exception) {
					var newChild = _settingsXML.CreateXmlDeclaration("1.0", "utf-8", string.Empty);
					_settingsXML.AppendChild(newChild);
					XmlNode node = null;
					node = _settingsXML.CreateNode(XmlNodeType.Element, "Settings", "");
					_settingsXML.AppendChild(node);
				}
				return _settingsXML;
			}
		}
	}
}

