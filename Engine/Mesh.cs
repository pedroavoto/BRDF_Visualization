using SharpDX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;

namespace Engine
{
    public class Mesh : IDisposable
    {
        public const int MAX_BONES_IN_MESH = 96;

        public Mesh( Buffer vertexBuffer = null, Buffer indexBuffer = null, Material material = null, int numVertices = 0, int numIndices = 0)
        {
            this.VertexBuffer = vertexBuffer;
            this.IndexBuffer = indexBuffer; 
            this.Material = material;
            this.NumVertices = numVertices;
            this.NumIndices = numIndices;
		}

        public Material Material { get; set; }

        public Buffer VertexBuffer { get; set; }

        public Buffer IndexBuffer { get; set; }

        public Matrix WorldMatrix { get; set; }

        public Matrix[] RawBoneConstantBufferData { get; private set; }

        public PrimitiveTopology PrimitiveTopology 
        { 
            get 
            {
                return PrimitiveTopology.TriangleList;
            } 
        }

        public int NumVertices { get; set; }

        public int NumIndices { get; set; }

        public void Draw(Timer timer)
        {
			DeviceContext context = PipelineManager.Get().ImmediateContext;

			this.Material.BindMaterial();
			
			context.InputAssembler.PrimitiveTopology = this.PrimitiveTopology;
			context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.VertexBuffer, this.Material.Shader.InputLayoutWrapper.stride, 0));

			if (this.IndexBuffer == null)
            {
                context.Draw(this.NumVertices, 0);
            }
            else
            {
                context.InputAssembler.SetIndexBuffer(this.IndexBuffer, Format.R32_UInt, 0);

                context.DrawIndexed(this.NumIndices, 0, 0);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.IndexBuffer.SafeDispose();
                this.VertexBuffer.SafeDispose();
                this.Material.SafeDispose();
            }
        }

    }
}
