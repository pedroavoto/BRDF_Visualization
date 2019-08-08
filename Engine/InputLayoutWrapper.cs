using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;

namespace Engine
{
    public class InputLayoutWrapper : IDisposable
    {
        public InputLayoutWrapper(InputLayout inputLayout, int stride)
        {
            this.InputLayout = inputLayout;
            this.stride = stride;
        }

        public InputLayout InputLayout { get; private set; }

        public int stride { get; set; }

        public void Dispose()
        {
            this.Dispose(true);

        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.InputLayout.SafeDispose();
            }
        }
    }
}
