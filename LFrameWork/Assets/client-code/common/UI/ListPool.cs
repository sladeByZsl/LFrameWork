//------------------------------------------------------------------------------
// All Right Reserved.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//------------------------------------------------------------------------------


    using System.Collections.Generic;

    /// <summary>
    /// The list pool.
    /// </summary>
    internal static class ListPool<T>
    {
        private static List<T> list;

        /// <summary>
        /// Get a new list.
        /// </summary>
        internal static List<T> Get()
        {
            if (list != null)
            {
                var l = list;
                list = null;
                return l;
            }

            return new List<T>();
        }

        /// <summary>
        /// Put back a list.
        /// </summary>
        internal static void Release(List<T> l)
        {
            if (list == null)
            {
                list = l;
            }
        }
    }


