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
