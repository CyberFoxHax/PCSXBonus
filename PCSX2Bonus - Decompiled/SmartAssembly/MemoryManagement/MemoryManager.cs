namespace SmartAssembly.MemoryManagement{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public sealed class MemoryManager {

        private static MemoryManager _mem;
        private long _time = DateTime.Now.Ticks;

        private static void F1(){
            using (var process = Process.GetCurrentProcess()){
                f3(process.Handle, -1, -1);
            }
        }

	    private void F2(object sender, EventArgs args){
            var ticks = DateTime.Now.Ticks;
	        if (ticks - _time <= 10000000) return;
	        _time = ticks;
	        this.F1();
        }

        [DllImport("kernel32", EntryPoint="SetProcessWorkingSetSize")]
		private static extern int f3(IntPtr ptr, int a, int b);
        private MemoryManager(){
            Application.Idle += F2;
            this.F1();
        }

        public static void AttachApp(){
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                _mem = new MemoryManager();
        }
    }
}

