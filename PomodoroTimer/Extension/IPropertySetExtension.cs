using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace PomodoroTimer.Extension
{
    static class IPropertySetExtension
    {
        public static T GetValue<T>(this IPropertySet set, string key, T defaul)
        {
            if (!set.ContainsKey(key))
                return defaul;

            return (T)set[key];
        }
    }
}
