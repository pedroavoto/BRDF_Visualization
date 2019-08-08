using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX;
using System.Runtime.InteropServices;

namespace Engine
{
	public enum ConstantBufferType
	{
		PerFrame,
		PerObject
	}

	[Flags]
	public enum PipelineStages
	{
		VertexShader = 0,
		PixelShader = 1 << 0,
		All         = 1 << 1
	}

    public interface ISizeable
    {
        int GetSize();
    }

	[StructLayout(LayoutKind.Sequential)]
	public struct PerFrameCB : ISizeable
	{
		public PerFrameCB(Matrix viewProj, Vector3 lightDirection, Vector3 eyePosition, float roughnessOrenNayar, int myDiffuse, int mySpecular, Vector4 myColorObject)
		{
			this.viewProj = viewProj;
            this.lightDirection = new Vector4(lightDirection,1);
            this.eyePosition = new Vector4(eyePosition,1);
            this.roughnessOrenNayar = roughnessOrenNayar;
            this.myDiffuse = myDiffuse;
            this.mySpecular = mySpecular;
            this.ColorObject = myColorObject;
        }

		Matrix viewProj;
        Vector4 lightDirection;
        Vector4 eyePosition;
        Vector4 ColorObject;
        float roughnessOrenNayar;
        int myDiffuse;
        int mySpecular;

        public int GetSize()
        {
            return Marshal.SizeOf(this.viewProj) + Marshal.SizeOf(this.lightDirection) + Marshal.SizeOf(this.eyePosition) + Marshal.SizeOf(this.roughnessOrenNayar) + Marshal.SizeOf(this.myDiffuse) + Marshal.SizeOf(this.mySpecular) + Marshal.SizeOf(this.ColorObject);
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 64)]
    public struct PerObjectCB : ISizeable
    {
        public PerObjectCB(Matrix world)
        {
            this.world = world;
        }
        [FieldOffset(0)]
        Matrix world;
    
        public int GetSize()
        {
            return Marshal.SizeOf(world);
        }
    }

	public abstract class ConstantBuffer : IDisposable
	{
		public ConstantBufferType ConstantBufferType { get; protected set; }

		public Buffer Buffer { get; protected set; }

		public PipelineStages StagesToBind { get; protected set; }

		public virtual void Dispose()
		{
			throw new NotImplementedException();
		}

		public virtual void Update()
		{
			throw new NotImplementedException();
		}
	}

	public class ConstantBuffer<T> : ConstantBuffer where T : struct, ISizeable
	{
		private Func<T> dataProvider;
		private int slot;
        private DataStream dataStream;
        private int sizeInBytes;

		public ConstantBuffer(Func<T> dataProvider, int slot, PipelineStages stagesToBind = PipelineStages.All, bool CPUAccess = true, ConstantBufferType type = ConstantBufferType.PerObject)
		{
			BufferDescription desc;
			desc.BindFlags = BindFlags.ConstantBuffer;
			if (CPUAccess)
			{
				desc.CpuAccessFlags = CpuAccessFlags.Write;
				desc.Usage = ResourceUsage.Dynamic;
			}
			else
			{
				desc.CpuAccessFlags = CpuAccessFlags.None;
				desc.Usage = ResourceUsage.Immutable;
			}

            this.dataProvider = dataProvider;
            T data = this.dataProvider();
			desc.OptionFlags = ResourceOptionFlags.None;

            int rest = (16 - data.GetSize() % 16) == 16 ? 0 : (16 - data.GetSize() % 16);

            desc.SizeInBytes = data.GetSize() + rest;
			desc.StructureByteStride = 0;
            this.sizeInBytes = desc.SizeInBytes;

			this.StagesToBind = stagesToBind;
			this.slot = slot;
            this.dataStream = new DataStream(desc.SizeInBytes, true, true);

            this.Buffer = Buffer.Create<T>(PipelineManager.Get().Device, BindFlags.ConstantBuffer, ref data, desc.SizeInBytes, ResourceUsage.Dynamic, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
		}

		public override void Update()
		{
			T dataToUpdate = this.dataProvider();

			var context = PipelineManager.Get().ImmediateContext;

            Marshal.StructureToPtr(dataToUpdate, this.dataStream.DataPointer, true);

            DataStream writeStream;

            context.MapSubresource(this.Buffer, MapMode.WriteDiscard, MapFlags.None, out writeStream);

            writeStream.Write(this.dataStream.DataPointer, 0, this.sizeInBytes);

			context.UnmapSubresource(this.Buffer, 0);


			if ((this.StagesToBind & PipelineStages.VertexShader) == PipelineStages.VertexShader || (this.StagesToBind & PipelineStages.All) == PipelineStages.All)
			{
				context.VertexShader.SetConstantBuffer(this.slot, this.Buffer);
			}

			if ((this.StagesToBind & PipelineStages.VertexShader) == PipelineStages.VertexShader || this.StagesToBind == PipelineStages.All)
			{
				context.PixelShader.SetConstantBuffer(this.slot, this.Buffer);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Buffer.SafeDispose();
                dataStream.SafeDispose();
			}
		}
		public override void Dispose()
		{
			Dispose(true);
		}
	}
}
