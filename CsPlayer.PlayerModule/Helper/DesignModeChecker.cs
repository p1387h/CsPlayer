using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CsPlayer.PlayerModule.Helper
{
    public class DesignModeChecker
    {
        public static bool IsInDesignMode()
        {
            System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
            bool res = process.ProcessName == "devenv";
            process.Dispose();
            return res;
        }
    }
}
