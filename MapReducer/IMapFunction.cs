using System;
using System.Collections.Generic;
using System.Text;

namespace MapReducer {
    public interface IMapFunction<IMK, IMV, OMK, OMV> {

        public List<Pair<OMK, OMV>> Run(IMK k, IMV v);

    }
    public class Pair<K, V> {
        public Pair(K k, V v) {
            Key = k;
            Value = v;
        }
        public K Key { get; set; }
        public V Value { get; set; }
    }
}
