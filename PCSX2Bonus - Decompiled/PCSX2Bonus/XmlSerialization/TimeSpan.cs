namespace PCSX2Bonus.XmlSerialization
{
    using System;
    using System.Runtime.InteropServices;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [StructLayout(LayoutKind.Sequential)]
    public struct TimeSpan : IXmlSerializable
    {
        private System.TimeSpan _value;
        public static implicit operator PCSX2Bonus.XmlSerialization.TimeSpan(System.TimeSpan value)
        {
            return new PCSX2Bonus.XmlSerialization.TimeSpan { _value = value };
        }

        public static implicit operator System.TimeSpan(PCSX2Bonus.XmlSerialization.TimeSpan value)
        {
            return value._value;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            this._value = System.TimeSpan.Parse(reader.ReadElementContentAsString());
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteValue(this._value.ToString());
        }

        public override string ToString()
        {
            return this._value.ToString();
        }
    }
}

