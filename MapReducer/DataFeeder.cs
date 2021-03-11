using System;
using System.Collections.Generic;
using System.Text;

namespace MapReducer {
    public class DataFeeder<OMK, OMV> {
        private static readonly IDictionary<OMK, List<OMV>> DATA = new Dictionary<OMK, List<OMV>>();
        public static IDictionary<OMK, List<OMV>> DataFeed { get { return DATA; } }

        public void PrintDataFeed() {
            foreach (var kv in DataFeed) {
                Console.Write("KEY: " + kv.Key + "[ ");
                foreach (OMV val in kv.Value) {
                    Console.Write(val + " ");
                }
                Console.WriteLine("]");
            }
        }
    }
}

