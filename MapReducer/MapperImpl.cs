using System;
using System.Collections.Generic;
using System.Text;

namespace MapReducer {
    class MapperImpl<IMK, IMV, OMK, OMV, MF> : IMapper<IMK, IMV, OMK, OMV, MF> where MF : IMapFunction<IMK, IMV, OMK, OMV> {

        private readonly IDictionary<IMK, List<IMV>> _input = new Dictionary<IMK, List<IMV>>();
        private IDictionary<IMK, List<IMV>> Input => _input;
        private MF mapfunction = default;

        public MF Function { get { return mapfunction; } set { mapfunction = value; } }

        public MapperImpl(MF mf) {
            this.Function = mf;
        }

        public void ReceiveInputPair(Pair<IMK, IMV> pair) {// 1|"verde branco azul", 5|"rosa branco preto"
            if (!this.Input.TryGetValue(pair.Key, out List<IMV> ivs)) {
                ivs = new List<IMV>();
                Input.Add(pair.Key, ivs);
            }
            ivs.Add(pair.Value);
        }

        public void Compute() {
            var Output = DataFeeder<OMK, OMV>.DataFeed;
            bool[] flag;
            int[] turn;
            int cont = 0;
            foreach(var kv in Input){
                IMK chave = kv.Key;
                List<IMV> valores = kv.Value;
                int l = (Input.Count);
                flag = new bool[l];
                turn = new int[l];
                Array.Clear(turn, 0, l);
                foreach (IMV valor in valores) {
                    flag[cont] = true;
                    int cont2 = cont + 1;
                    if(cont == l - 1)
                    {
                        cont2 = 0;
                    }
                    while(flag[cont2] == true)
                    {
                        if(turn[cont] != 0)
                        {
                            flag[cont] = false;
                            while (turn[cont] != 0) ;
                            flag[cont] = true;
                        }
                    }
                    List<Pair<OMK, OMV>> pares = Function.Run(chave, valor);
                    foreach (Pair<OMK, OMV> par in pares) {
                        if (!Output.TryGetValue(par.Key, out List<OMV> omvs)) {
                            omvs = new List<OMV>();
                            if (par.Key != null && omvs != null)
                            {
                                Output.Add(par.Key, omvs);
                            }
                        }
                        omvs.Add(par.Value);
                        if (Function is IMapFunctionCombiner<IMK, IMV, OMK, OMV>) {
                            IMapFunctionCombiner<IMK, IMV, OMK, OMV> mf = (IMapFunctionCombiner<IMK, IMV, OMK, OMV>)Function;
                            OMV omv = mf.Combiner(omvs);
                            omvs.Clear();
                            omvs.Add(omv);
                        }
                    }
                    turn[cont] = 1;
                    flag[cont] = false;
                    if (cont == l - 1)
                    {
                        cont = 0;
                    } else
                    {
                        cont++;
                    }
                }
            }
        }
    }
}
