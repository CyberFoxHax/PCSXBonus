namespace SmartAssembly.Attributes
{
    using System;

    public sealed class PoweredByAttribute : Attribute
    {
	    public string P1 { get; set; }

	    public PoweredByAttribute(string p1){
	        P1 = p1;
        }
    }
}

