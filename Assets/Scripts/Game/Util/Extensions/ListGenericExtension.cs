using System;
using System.Collections.Generic;

namespace Game.Util.Extensions
{
    public static class ListGenericExtension
    {
        public static T GetIndex<T>(this List<T> list, int index)
        {
            index = index < 0 ? index + list.Count : index;
            index %= list.Count;

            if (index < 0 || index >= list.Count)
                throw new IndexOutOfRangeException($"{list.ToString()} can`t exist {index}");

            return list[index];
        }
    }
}
