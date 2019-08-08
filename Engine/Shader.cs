using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using SharpDX.D3DCompiler;

namespace Engine
{
    public class Shader : IDisposable
    {

        public VertexShader VertexShader {get; private set;}
        public PixelShader PixelShader {get; private set;}

        public InputLayoutWrapper InputLayoutWrapper { get; private set;}

        public Shader()
        {

        }

        public void SetVertexShader(string shaderFile, Func<byte[], InputLayoutWrapper> inputLayoutFunc, bool isDebug = true)
        {
            shaderFile = GetShaderFilePath(shaderFile);
            byte[] vertexShaderByteCode = ShaderBytecode.CompileFromFile(shaderFile, "VS", "vs_5_0", ShaderFlags.SkipOptimization | ShaderFlags.Debug, EffectFlags.None);
            this.VertexShader = new VertexShader(PipelineManager.Get().Device, vertexShaderByteCode);


            this.InputLayoutWrapper = inputLayoutFunc(vertexShaderByteCode);
        }

        public void SetPixelShader(string shaderFile, bool isDebug = true)
        {
			shaderFile = GetShaderFilePath(shaderFile);

			byte[] pixelShaderByteCode = ShaderBytecode.CompileFromFile(shaderFile, "PS", "ps_5_0", ShaderFlags.SkipOptimization | ShaderFlags.Debug, EffectFlags.None);
            this.PixelShader = new PixelShader(PipelineManager.Get().Device, pixelShaderByteCode);
        }

        private string GetShaderFilePath(string shaderFileName)
        {
            string relativePath = "Shaders\\" + shaderFileName;

            return relativePath;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.VertexShader.SafeDispose();
                this.PixelShader.SafeDispose();
                this.InputLayoutWrapper.SafeDispose();
            }
        }
    }
}
