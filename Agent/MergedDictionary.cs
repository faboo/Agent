using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Data;

namespace Agent {
    public class MergedDictionary<Key, Value>: IDictionary<Key, Value> {
        private Dictionary<Key, Value> from = null;
        private Dictionary<Key, Value> derived = new Dictionary<Key, Value>();

        public MergedDictionary(Dictionary<Key, Value> from) {
            this.from = from;
        }

        public ICollection<Key> Keys {
            get {
                return from.Keys.Union(derived.Keys).ToList();
            }
        }

        public ICollection<Value> Values {
            get {
                return new CompositeCollection{
                    from.Values,
                    derived.Values
                }.Cast<Value>().ToList();
            }
        }

        public Value this[Key key] {
            get {
                if(from.ContainsKey(key))
                    return from[key];
                else
                    return derived[key];
            }
            set {
                derived[key] = value;
            }
        }

        public int Count {
            get { return from.Count + derived.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public void Add(Key key, Value value) {
            derived.Add(key, value);
        }

        public bool ContainsKey(Key key) {
            return derived.ContainsKey(key) || from.ContainsKey(key);
        }

        public bool Remove(Key key) {
            return derived.Remove(key);
        }

        public bool TryGetValue(Key key, out Value value) {
            if(from.ContainsKey(key)) {
                value = from[key];
                return true;
            }
            else {
                return derived.TryGetValue(key, out value);
            }
        }

        public void Add(KeyValuePair<Key, Value> item) {
            derived[item.Key] = item.Value;
        }

        public void Clear() {
            derived.Clear();
        }

        public bool Contains(KeyValuePair<Key, Value> item) {
            return from.Contains(item) || derived.Contains(item);
        }

        public void CopyTo(KeyValuePair<Key, Value>[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<Key, Value> item) {
            return derived.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator() {
            return new CompositeCollection {
                from.GetEnumerator(),
                derived.GetEnumerator()
            }.Cast<KeyValuePair<Key, Value>>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new CompositeCollection {
                from.GetEnumerator(),
                derived.GetEnumerator()
            }.Cast<KeyValuePair<Key, Value>>().GetEnumerator();
        }
    }
}
