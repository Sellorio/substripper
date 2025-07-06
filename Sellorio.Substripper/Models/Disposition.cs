using System;

namespace Sellorio.Substripper.Models
{
    [Flags]
    public enum Disposition
    {
        None = 0,
        Default = 1,
        Forced = 2,

        Other = 128
    }
}
