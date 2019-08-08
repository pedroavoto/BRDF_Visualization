using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public static class SamplerStates
    {
        public static SamplerState MinMagMipLinearWrap { get; private set; }

        static SamplerStates()
        {
            MinMagMipLinearWrap = CreateMinMagMipLinearWrap();
        }

        private static SamplerState CreateMinMagMipLinearWrap()
        {
            SamplerStateDescription desc = new SamplerStateDescription();
            desc.AddressU = TextureAddressMode.Wrap;
            desc.AddressV = TextureAddressMode.Wrap;
            desc.AddressW = TextureAddressMode.Wrap;
            desc.Filter = Filter.ComparisonMinMagMipLinear;
            desc.ComparisonFunction = Comparison.Always;
            desc.MipLodBias = 0;
            desc.MaximumLod = 1000;
            desc.MinimumLod = 0;

            return new SamplerState(PipelineManager.Get().Device, desc);
        }
    }
}
