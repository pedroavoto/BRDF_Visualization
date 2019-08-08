using Engine;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProjectIglooSharp
{
    class DemoApp : App, IDisposable
    {
        Mesh triMesh;

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {
            this.triMesh = SimpleMeshes.ScreenSpaceTriangle();

            base.LoadContent();
        }

        public override void Update(Timer timer)
        {
            base.Update(timer);
        }

        public override void Draw(Timer timer)
        {
            this.pipelineManager.ImmediateContext.ClearRenderTargetView(this.pipelineManager.BackBufferRTV, Color.CornflowerBlue);

            this.triMesh.Draw();

            base.Draw(timer);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.triMesh.SafeDispose();
				this.form.SafeDispose();
				this.pipelineManager.SafeDispose();
            }
        }
    }
}
