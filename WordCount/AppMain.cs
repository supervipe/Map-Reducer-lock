using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using MapReducer;

namespace WordCount {
    class AppMain {
        public static readonly int N = 4;
        public static readonly string file = @"C:\\Users\\USUARIO\\Downloads\\Comp Paralela\\MapReducer\\WordCount\\Words.txt";

        public static void Main(string[] args) {
            IEnumerable<string> enumerable = File.ReadAllLines(file);
            IEnumerator<string> iterator = enumerable.GetEnumerator();

            var lista = new List<Pair<int, string>>();
            int i = 1;

            while (iterator.MoveNext()) {
                if (!iterator.Current.Trim().Equals("")) {
                    lista.Add(new Pair<int, string>(i++, iterator.Current));
                }
            }

            WordCounting wc = new WordCounting();
            var splitter = new SplitterImpl<int, string, string, int, WordCounting>();
            
            var feeder = splitter.Splitting(N, wc, lista);

            feeder.PrintDataFeed();
        }
    }
}
