using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MapReducer {
    class MapperImpl<IMK, IMV, OMK, OMV, MF> : IMapper<IMK, IMV, OMK, OMV, MF> where MF : IMapFunction<IMK, IMV, OMK, OMV> {

        private readonly IDictionary<IMK, List<IMV>> _input = new Dictionary<IMK, List<IMV>>();
        private IDictionary<IMK, List<IMV>> Input => _input;
        private MF mapfunction = default;
        //volatile int[][] flag;
        //volatile int[][] turn;
        static int levels = -1;
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
                for (int k = 1; k < l - 1; k++)
                {
                    if(k == i)
                    {
                        continue;
                    }
                    while (flag[k] >= j && turn[j] == i)
                    {
                        Console.WriteLine("Esperando...");
                    }
                }
            }
            flag[i] = 0;
        }

        /*public void lockerN(Object locker, int l, int cont) {

            node = cont;
            binaryID = Convert.ToString(l,2);
            for (int i=0; i<levels; i++) {

                int idThisLevel = 0;
                if (i<binaryID.Length){
                    idThisLevel = binaryID[binaryID.Length-1-i] - '0';
                }

                node = node/2;
                flag[i][2 * node + idThisLevel] = 1;
                turn[i][node] =  1 - idThisLevel;

                while (flag[i][2 * node + 1 - idThisLevel] == 1 && (turn[i][node] != idThisLevel)) {};
            }

        }

        public void unlocker()
        {
            for (int i = levels - 1; i >= 0; i--)
            {
                int idThisLevel = 0;
                if (i < binaryID.Length)
                {
                    idThisLevel = binaryID[binaryID.Length - 1 - i] - '0';
                }

                flag[i][2 * node + idThisLevel] = 0;
                node = (int)(ID / Math.Pow(2, i));
            }
        }*/

        public void Compute()
        {
            int cont = 0;
            var Output = DataFeeder<OMK, OMV>.DataFeed;
                
            foreach (var kv in Input)
            {
                IMK chave = kv.Key;
                List<IMV> valores = kv.Value;
                int l = (Input.Count);
                lockN(locker, l, cont);
                //lock(locker)
                {
                   
                    foreach (IMV valor in valores)
                    {
                       
                        List<Pair<OMK, OMV>> pares = Function.Run(chave, valor);
                        foreach (Pair<OMK, OMV> par in pares)
                        {
                            if (!Output.TryGetValue(par.Key, out List<OMV> omvs))
                            {
                                omvs = new List<OMV>();
                                if (par.Key != null || omvs != null)
                                {
                                    Output.Add(par.Key, omvs);
                                }
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
                    
                }
                //unlocker();
                //unlockN(cont);
                cont++;
            }
        }
    }
}
