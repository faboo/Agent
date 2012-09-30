using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Agent {
    public static class ObservableCollectionExtension {
        public static ICollection<T> RemoveRange<T>(this ObservableCollection<T> collection, int start, int end) {
            List<T> removed = new List<T>();

            while (start != end) {
                removed.Add(collection[start]);
                collection.RemoveAt(start);
                end -= 1;
            }

            return removed;
        }

        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> range) {
            foreach (var item in range)
                collection.Add(item);
        }
    }
}
