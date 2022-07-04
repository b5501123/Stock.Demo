using System.Collections.Generic;
using System.Linq;
using Mapster;

namespace Stock.Common.Extension
{
    public static class ObjectExtension
    {
        public static T Reflect<T>(this object data)
        {
            if (data == null)
            {
                return default;
            }

            return data.Adapt<T>();
        }

        public static List<T> Reflect<T>(this IEnumerable<object> data)
        {
            return data.Select(i => i.Reflect<T>()).ToList();
        }
    }
}
