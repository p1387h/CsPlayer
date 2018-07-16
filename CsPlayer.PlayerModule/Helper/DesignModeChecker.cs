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
            var maybeExpressionUseLayoutRounding = Application.Current.Resources["ExpressionUseLayoutRounding"] as bool?;
            return maybeExpressionUseLayoutRounding ?? false;
        }
    }
}
