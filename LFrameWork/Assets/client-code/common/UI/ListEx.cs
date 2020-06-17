//------------------------------------------------------------------------------
// All Right Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//------------------------------------------------------------------------------

    using System.Collections.Generic;
    using Random = UnityEngine.Random;

    /// <summary>
    /// Extensions for <see cref="List"/>.
    /// </summary>
    public static class ListEx
    {
        /// <summary>
        /// Reserve the capacity for a list. The same mean as the 'std::vector'
        /// in c++.
        /// </summary>
        public static void Reserve<T>(this List<T> array, int capacity)
        {
            if (array.Capacity < capacity)
            {
                array.Capacity = capacity;
            }
        }

        /// <summary>
        /// Random shuffle the content of the list.
        /// </summary>
        public static void Shuffle<T>(this IList<T> array)
        {
            for (int i = 0; i < array.Count; ++i)
            {
                var randIdx = Random.Range(0, array.Count);
                var temp = array[randIdx];
                array[randIdx] = array[i];
                array[i] = temp;
            }
        }

        /// <summary>
        /// Remove duplicate elements.
        /// </summary>
        public static void RemoveDuplicate<T>(this List<T> list)
        {
            var lookup = new HashSet<T>();
            foreach (var i in list)
            {
                if (!lookup.Contains(i))
                {
                    lookup.Add(i);
                }
            }

            list.Clear();
            foreach (var i in lookup)
            {
                list.Add(i);
            }
        }
    }

