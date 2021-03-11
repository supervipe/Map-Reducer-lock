using System;
using System.Collections.Generic;
using System.Text;

namespace MapReducer {
    public interface ISplitter<IMK, IMV, OMK, OMV, MF> where MF : IMapFunction<IMK, IMV, OMK, OMV> {

        public DataFeeder<OMK, OMV> Splitting(int n, MF mf, IEnumerable<Pair<IMK, IMV>> enumerable);

    }
}
