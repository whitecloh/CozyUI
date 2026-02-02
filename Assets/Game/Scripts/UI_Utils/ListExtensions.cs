using System.Collections.Generic;

namespace Game.Scripts.UI_Utils
{
    public static class ListExtensions
    {
        public static bool InBounds<T>(this IReadOnlyList<T> list, int index)
        {
            return list != null && index >= 0 && index < list.Count;
        }
    }
}