namespace SmartAssembly.AssemblyResolver
{
    using System;

    public sealed class AssemblyResolver
    {
        public static void AttachApp()
        {
            try
            {
                AssemblyResolverHelper.Attach();
            }
            catch (Exception)
            {
            }
        }
    }
}

