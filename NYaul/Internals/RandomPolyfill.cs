using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NYaul.Internals;

internal class RandomPolyfill
{
#if NET6_0_OR_GREATER
    public static readonly System.Random Shared = Random.Shared;
#else
    public static readonly System.Random Shared = new Random();
#endif
}