using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MapReducer {
    public class SplitterImpl<IMK, IMV, OMK, OMV, MF> : ISplitter<IMK, IMV, OMK, OMV, MF> where MF : IMapFunction<IMK, IMV, OMK, OMV> {
        public static int idThread;
        public static int contador;
        public DataFeeder<OMK, OMV> Splitting(int n, MF mf, IEnumerable<Pair<IMK, IMV>> enumerable) {
            var threads = new List<Thread>();
            var mappers = new Dictionary<int, IMapper<IMK, IMV, OMK, OMV, MF>>();
            IEnumerator<Pair<IMK, IMV>> dados = enumerable.GetEnumerator();

            int i = 0;
            while (dados.MoveNext()) {
                IMapper<IMK, IMV, OMK, OMV, MF> mapper;

                int partition = i++ % n;
                if (!mappers.TryGetValue(partition, out mapper)) {
                    mapper = new MapperImpl<IMK, IMV, OMK, OMV, MF>(mf);
                    mappers.Add(partition, mapper);
                }
                mapper.ReceiveInputPair(new Pair<IMK, IMV>(dados.Current.Key, dados.Current.Value));
            }

            foreach (var kv in mappers) {
                Thread t = new Thread(kv.Value.Compute);
                t.Name = kv.Key.ToString();
                threads.Add(t);
                contador++;
            }
            idThread = 0;
            foreach (Thread t in threads)
                t.Start(t.Name);
            foreach (Thread t in threads)
                t.Join();

            return new DataFeeder<OMK, OMV>();
        }
    }
}
