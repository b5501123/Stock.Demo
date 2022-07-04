using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stock.Common.Extension
{
    public static class DecimalExtension
    {
        public static decimal ToFixTwo(this decimal data)
        {
            return Math.Round(data, 2, MidpointRounding.AwayFromZero);
        }
    }
}
