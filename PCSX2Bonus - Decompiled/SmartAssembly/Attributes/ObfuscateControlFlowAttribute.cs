﻿namespace SmartAssembly.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct | AttributeTargets.Class)]
    internal sealed class ObfuscateControlFlowAttribute : Attribute
    {
    }
}

