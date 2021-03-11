using System;
using System.Collections.Generic;
using System.Text;

namespace MapReducer {
    public interface IMapper<IMK, IMV, OMK, OMV, MF> where MF : IMapFunction<IMK, IMV, OMK, OMV> {
        public MF Function { get; set; }
        public void ReceiveInputPair(Pair<IMK, IMV> pair);
        public void Compute();
    }
}
