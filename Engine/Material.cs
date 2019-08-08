using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Engine
{
    public class Material : IDisposable
	{
		public Shader Shader { get; set; }

		public IList<ConstantBuffer> ConstantBuffers { get; set; }

        public ShaderResourceView DiffuseTexture { get; set; }

		public Material(Shader shader = null)
        {
            this.Shader = shader;
			this.ConstantBuffers = new List<ConstantBuffer>();
        }

		public void BindMaterial()
		{
			var context = PipelineManager.Get().ImmediateContext;

			this.ConstantBuffers.ForEach((p) => p.Update());

			context.InputAssembler.InputLayout = this.Shader.InputLayoutWrapper.InputLayout;
			context.VertexShader.Set(this.Shader.VertexShader);
			context.PixelShader.Set(this.Shader.PixelShader);
            if (DiffuseTexture != null)
            {
                context.PixelShader.SetShaderResource(0, DiffuseTexture);
            }
            context.PixelShader.SetSampler(0, SamplerStates.MinMagMipLinearWrap);
		}

		public void Dispose()
        {
            this.Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Shader.SafeDispose();

				this.ConstantBuffers.ForEach((p) => p.SafeDispose());
            }
        }
    }
}
