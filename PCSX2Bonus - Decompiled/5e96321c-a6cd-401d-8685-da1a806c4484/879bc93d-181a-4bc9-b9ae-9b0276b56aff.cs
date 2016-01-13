namespace 
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    internal sealed class 
    {
        private static Assembly  = null;
        private static string[]  = new string[0];

        private static bool ()
        {
            try
            {
                StackFrame[] frames = new StackTrace().GetFrames();
                for (int i = 2; i < frames.Length; i++)
                {
                    StackFrame frame = frames[i];
                    if (frame.GetMethod().Module.Assembly == Assembly.GetExecutingAssembly())
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return true;
            }
        }

        internal static void ()
        {
            try
            {
                AppDomain.CurrentDomain.ResourceResolve += new ResolveEventHandler(..);
            }
            catch (Exception)
            {
            }
        }

        private static Assembly (object, ResolveEventArgs args1)
        {
            if ( == null)
            {
                lock ()
                {
                     = Assembly.Load("{2b24044f-6448-4732-a0d8-9a4b37841f47}, PublicKeyToken=3e56350693f7355e");
                    if ( != null)
                    {
                         = .GetManifestResourceNames();
                    }
                }
            }
            string name = args1.Name;
            for (int i = 0; i < .Length; i++)
            {
                if ([i] == name)
                {
                    if (!())
                    {
                        return null;
                    }
                    return ;
                }
            }
            return null;
        }
    }
}

