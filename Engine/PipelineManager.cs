using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace Engine
{
    public class PipelineManager : IDisposable
    {
        private static PipelineManager instance = null;

        private PipelineManager()
        { }

        public static PipelineManager Get()
        {
            if(PipelineManager.instance == null)
            {
                PipelineManager.instance = new PipelineManager();
            }
            return PipelineManager.instance;
        }

        public DeviceContext ImmediateContext { get; private set; }

        public RenderTargetView BackBufferRTV { get; private set; }

        public DepthStencilView DepthStencilView { get; private set; }

        public Texture2D BackBufferTexture { get; private set; }

        public Texture2D DepthStencilTexture { get; private set; }

        public SwapChain SwapChain { get; private set; }

		public Device Device { get; private set; }

		public RenderForm RenderForm { get; private set; }

		private Factory factory;

        public void Initialize(RenderForm form)
        {
			this.RenderForm = form;

            // SwapChain description
            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription =
                    new ModeDescription(form.ClientSize.Width, form.ClientSize.Height,
                                        new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = form.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            // Create Device and SwapChain
            Device device;
            SwapChain swapChain;
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, desc, out device, out swapChain); 

            this.Device = device;
            this.ImmediateContext = device.ImmediateContext;
            this.SwapChain = swapChain;
           
            // Ignore all windows events
            this.factory = this.SwapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(form.Handle, WindowAssociationFlags.IgnoreAll);

            // New RenderTargetView from the backbuffer
            this.BackBufferTexture = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            this.BackBufferRTV = new RenderTargetView(device, this.BackBufferTexture);

            
            Texture2DDescription depthDesc = new Texture2DDescription();
            depthDesc.ArraySize = 1;
            depthDesc.BindFlags = BindFlags.DepthStencil;
            depthDesc.CpuAccessFlags = CpuAccessFlags.None;
            depthDesc.Format = Format.D24_UNorm_S8_UInt;
            depthDesc.Height = form.ClientSize.Height;
            depthDesc.Width = form.ClientSize.Width;
            depthDesc.MipLevels = 1;
            depthDesc.SampleDescription.Count = 1;
            depthDesc.SampleDescription.Quality = 0;
            depthDesc.Usage = ResourceUsage.Default;

            this.DepthStencilTexture = new Texture2D(this.Device, depthDesc);

            ShaderResourceViewDescription depthSRVdesc;
            depthSRVdesc.Dimension = ShaderResourceViewDimension.Texture2D;
            depthSRVdesc.Format = Format.D24_UNorm_S8_UInt;
            depthSRVdesc.Texture2D.MipLevels = 1;
            depthSRVdesc.Texture2D.MostDetailedMip = 1;

            this.DepthStencilView = new DepthStencilView(this.Device, this.DepthStencilTexture);

            this.ImmediateContext.Rasterizer.SetViewport(new Viewport(0, 0, form.ClientSize.Width, form.ClientSize.Height, 0.0f, 1.0f));
            
            this.ImmediateContext.OutputMerger.SetTargets(this.BackBufferRTV);
            this.ImmediateContext.OutputMerger.SetRenderTargets(this.DepthStencilView, this.BackBufferRTV);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if(disposing)
            {
                this.DepthStencilTexture.SafeDispose();
                this.DepthStencilView.SafeDispose();
                this.BackBufferRTV.SafeDispose();
                this.BackBufferTexture.SafeDispose();
                this.ImmediateContext.ClearState();
                this.ImmediateContext.Flush();
                this.Device.SafeDispose();
                this.ImmediateContext.SafeDispose();
                this.SwapChain.SafeDispose();
                this.factory.SafeDispose();    
                
            }
        }
    }
}
