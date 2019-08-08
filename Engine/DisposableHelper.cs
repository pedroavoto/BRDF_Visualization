using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class DisposableHelper
    {
        public static void SafeDispose(this IDisposable obj)
        {
            if (obj != null)
                obj.Dispose();
        }
    }
}
