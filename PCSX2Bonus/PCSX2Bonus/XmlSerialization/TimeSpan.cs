using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PCSX2Bonus.PCSX2Bonus.XmlSerialization {
	[StructLayout(LayoutKind.Sequential)]
	public struct TimeSpan : IXmlSerializable {
		private System.TimeSpan _value;
		public static implicit operator TimeSpan(System.TimeSpan value) {
			return new TimeSpan { _value = value };
		}

		public static implicit operator System.TimeSpan(TimeSpan value) {
			return value._value;
		}

		public XmlSchema GetSchema() {
			return null;
		}

		public void ReadXml(XmlReader reader) {
			_value = System.TimeSpan.Parse(reader.ReadElementContentAsString());
		}

		public void WriteXml(XmlWriter writer) {
			writer.WriteValue(_value.ToString());
		}

		public override string ToString() {
			return _value.ToString();
		}

		public TimeSpan Add(System.TimeSpan subtract){
			return new TimeSpan();
		}
	}
}

