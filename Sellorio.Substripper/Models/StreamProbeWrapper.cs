using System.Collections.Generic;

namespace Sellorio.Substripper.Models
{
    public class StreamProbeWrapper<T>
    {
        public IList<T> Streams { get; set; }
    }
}
