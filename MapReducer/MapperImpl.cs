using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MapReducer {
    class MapperImpl<IMK, IMV, OMK, OMV, MF> : IMapper<IMK, IMV, OMK, OMV, MF> where MF : IMapFunction<IMK, IMV, OMK, OMV> {

        private readonly IDictionary<IMK, List<IMV>> _input = new Dictionary<IMK, List<IMV>>();
        private IDictionary<IMK, List<IMV>> Input => _input;
        private MF mapfunction = default;
        volatile int[] flag;
        volatile int[] turn;
        private static readonly Object locker = new Object();
        private int ID;
        private int node;
        private String binaryID;

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

       
        private void lockN(Object locker, int l, int i)
        {
            flag = new int[l + 1];
            turn = new int[l];
            for(int j = 1; j < l - 1; j++)
            {
                flag[i] = j;
                turn[j] = i;
                int k = SplitterImpl<IMK, IMV, OMK, OMV, MF>.idThread;
                while(turn[j] != i || (k != i && flag[k] >= j)) ;
            }
            flag[i] = -1;
        }
        private void unlockN()
        {
            SplitterImpl<IMK, IMV, OMK, OMV, MF>.idThread = (SplitterImpl<IMK, IMV, OMK, OMV, MF>.idThread + 1) % SplitterImpl<IMK, IMV, OMK, OMV, MF>.contador;
        }


        public void Compute(object currentId)
        {
            int id = SplitterImpl<IMK, IMV, OMK, OMV, MF>.idThread;
            int l = SplitterImpl<IMK, IMV, OMK, OMV, MF>.contador;
            lockN(locker, l, id);

            var Output = DataFeeder<OMK, OMV>.DataFeed;
                
            foreach (var kv in Input)
            {
                IMK chave = kv.Key;
                List<IMV> valores = kv.Value;
                   
                    foreach (IMV valor in valores)
                    {
                       
                        List<Pair<OMK, OMV>> pares = Function.Run(chave, valor);
                        foreach (Pair<OMK, OMV> par in pares)
                        {
                            if (!Output.TryGetValue(par.Key, out List<OMV> omvs))
                            {
                                omvs = new List<OMV>();
                                Output.Add(par.Key, omvs);
                            }
                            omvs.Add(par.Value);
                            if (Function is IMapFunctionCombiner<IMK, IMV, OMK, OMV>)
                            {
                                IMapFunctionCombiner<IMK, IMV, OMK, OMV> mf = (IMapFunctionCombiner<IMK, IMV, OMK, OMV>)Function;
                                OMV omv = mf.Combiner(omvs);
                                omvs.Clear();
                                omvs.Add(omv);
                            }
                        }
                    }
                unlockN();
            }
        }
    }
}
