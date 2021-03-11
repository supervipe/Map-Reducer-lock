using System;
using System.Collections.Generic;
using System.Text;
using MapReducer;

namespace WordCount {
    public class WordCounting : IMapFunctionCombiner<int, string, string, int> {

        public List<Pair<string, int>> Run(int k, string v) {
            var pares = new List<Pair<string, int>>();
            string[] words = v.Split(' ');
            foreach (string word in words)
                pares.Add(new Pair<string, int>(word, 1));

            return pares;
        }

        public int Combiner(List<int> omvs) {
            int soma = 0;
            foreach (int v in omvs)
                soma = soma + v;

            return soma;
        }

    }
}
