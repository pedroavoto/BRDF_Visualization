using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
	public static class RasterizerStates
	{
        public static RasterizerState CullModeNone { get; private set; }
        public static RasterizerState CullClockWise { get; private set; }
        public static RasterizerState CullCounterClockWise { get; private set; }

        public static RasterizerState WireFrame { get; private set; }

        static RasterizerStates()
        {
            CullModeNone = CreateCullModeNone();
            CullClockWise = CreateCullClockWise();
            CullCounterClockWise = CreateCullCounterClockWise();
            WireFrame = CreateWireFrame();
        }

        private static RasterizerState CreateWireFrame()
        {
            RasterizerStateDescription desc = new RasterizerStateDescription();
            desc.CullMode = CullMode.None;
            desc.FillMode = FillMode.Wireframe;

            return new RasterizerState(PipelineManager.Get().Device, desc);
        }

		private  static RasterizerState CreateCullModeNone()
		{
			RasterizerStateDescription desc = new RasterizerStateDescription();
			desc.CullMode = CullMode.None;
			desc.FillMode = FillMode.Solid;

			return new RasterizerState(PipelineManager.Get().Device, desc);			
		}

        private static RasterizerState CreateCullClockWise()
        {
            RasterizerStateDescription desc = new RasterizerStateDescription();
            desc.CullMode = CullMode.Back;
            desc.FillMode = FillMode.Solid;

            return new RasterizerState(PipelineManager.Get().Device, desc);
        }

        private static RasterizerState CreateCullCounterClockWise()
        {
            RasterizerStateDescription desc = new RasterizerStateDescription();
            desc.CullMode = CullMode.Front;
            desc.FillMode = FillMode.Solid;

            return new RasterizerState(PipelineManager.Get().Device, desc);
        }
	}
}
