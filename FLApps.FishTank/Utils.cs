using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FLApps.FishTank
{
    static class Utils
    {
        internal static double Distance(double x1, double y1, double x2, double y2)
        {
            return System.Math.Sqrt(System.Math.Pow(x1 - x2, 2) + System.Math.Pow(y1 - y2, 2));
        }
    }
}
