using System;
using System.Collections.Generic;
using System.Text;

namespace MapReducer {
    public interface IMapFunctionCombiner<IMK, IMV, OMK, OMV> : IMapFunction<IMK, IMV, OMK, OMV> {
        public OMV Combiner(List<OMV> omvs);
    }
}
