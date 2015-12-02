using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using SlimDX;
using OutsideEngine;
using System.Diagnostics;
using System.Drawing;

namespace OutsideSimulator
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Configuration.EnableObjectTracking = true;
            var ti = new MainRenderWindow(Process.GetCurrentProcess().Handle);
            if(!ti.Init())
            {
                return;
            }
            ti.Run();
        }
    }
}
