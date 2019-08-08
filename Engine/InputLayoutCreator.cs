using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class InputLayoutCreator
    {

        public static InputLayoutWrapper PosColor(byte[] vertexShaderByteCode)
        {

            InputLayout layout = new InputLayout(PipelineManager.Get().Device, ShaderSignature.GetInputSignature(vertexShaderByteCode),
            new[]{
                 new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
            });

            var wrapper = new InputLayoutWrapper(layout, 32);

            return wrapper;
        }

        public static InputLayoutWrapper PosTexCoordNormal(byte[] vertexShaderByteCode)
        {

            InputLayout layout = new InputLayout(PipelineManager.Get().Device, ShaderSignature.GetInputSignature(vertexShaderByteCode),
            new[]{
                 new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32_Float, 16, 0),
                 new InputElement("NORMAL", 0, Format.R32G32B32_Float, 24, 0),
            });

            var wrapper = new InputLayoutWrapper(layout, 36);

            return wrapper;
        }

        public static InputLayoutWrapper PosTexCoordNormalWeightsBoneIdx(byte[] vertexShaderByteCode)
        {

            InputLayout layout = new InputLayout(PipelineManager.Get().Device, ShaderSignature.GetInputSignature(vertexShaderByteCode),
            new[]{
                 new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32_Float, 16, 0),
                 new InputElement("NORMAL", 0, Format.R32G32B32_Float, 24, 0),
                 new InputElement("WEIGHTS", 0, Format.R32G32B32_Float, 36, 0),
                 new InputElement("BONEINDICES", 0, Format.R32G32B32A32_UInt, 48, 0),
            });

            var wrapper = new InputLayoutWrapper(layout, 64);

            return wrapper;
        }
    }
}
