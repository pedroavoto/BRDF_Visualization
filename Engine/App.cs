using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Windows;
using System.Drawing;
using System.Windows.Forms;
using Color = SharpDX.Color;
using SharpDX.DXGI;
using AntTweakBar;

namespace Engine
{
    public class ATBRenderForm : RenderForm
    {
        public Context Context { get; set; }

        public ATBRenderForm() : base() { }
        public ATBRenderForm(String text) : base(text) { }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            /* We could have handled all the mouse and keyboard events
             * separately, but AntTweakBar has a handy function that
             * can automatically hook into the Windows message pump.
            */

            if ((Context == null) || !Context.EventHandlerWin(m.HWnd, m.Msg, m.WParam, m.LParam))
            {
                base.WndProc(ref m);
            }
        }
    };

    public abstract class App
    {
        protected ATBRenderForm form;

        protected PipelineManager pipelineManager;

        public void Run(string windowName, Vector2 windowPos, int width, int height)
        {
            this.form = new ATBRenderForm(windowName);
            this.form.ClientSize = new Size(width, height);
            this.form.Location = new System.Drawing.Point((int)windowPos.X, (int)windowPos.Y);
            this.form.StartPosition = FormStartPosition.Manual;
            this.pipelineManager = PipelineManager.Get();
            pipelineManager.Initialize(this.form);

            Context UiContext = new Context(Tw.GraphicsAPI.D3D11, this.pipelineManager.Device.NativePointer);
            this.form.Context = UiContext;
            Timer timer = new Timer();
            timer.Reset(); // pq vc reseta duas vezes o timer aqui?

            this.Initialize();
            this.LoadContent();
            this.BuildUI(this.form.Context);
			

			RenderLoop.Run(this.form, () =>
			{
				timer.Update();
				this.Update(timer);
				this.Draw(timer);
                this.form.Context.Draw(); //DrawUI
				this.pipelineManager.SwapChain.Present(0, PresentFlags.None);
			});

            UiContext.Dispose();
            this.form.Dispose();
        }

        public virtual void BuildUI(Context UIContext)
        {

        }

        public virtual void Update(Timer timer)
        {
			InputHandler.Get().Update();
        }

        public virtual void Draw(Timer timer)
        {

        }

        public virtual void Initialize()
        {

        }

        public virtual void LoadContent()
        {

        }
    }
}
