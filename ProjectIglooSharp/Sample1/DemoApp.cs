using Engine;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mesh = Engine.Mesh;
using AntTweakBar;
using SharpDX.Direct3D11;

namespace ProjectIglooSharp
{

    enum Diffuse
    {
        OrenNayar,
        Lambert   
    };

    enum Specular
    {
        BlinnPhong,
        CookTorrance
    };

    class DemoApp : App, IDisposable
    {
        private Mesh modelMesh;
        private FPSCam fpsCam;
        ConstantBuffer<PerFrameCB> cameraCB;
        ConstantBuffer<PerObjectCB> objectCB;
        Vector3 lightDirection;
        Vector4 ColorObject;
        float orenNayarRoughness;
        Diffuse myDiffuse = Diffuse.OrenNayar;
        Specular mySpecular = Specular.BlinnPhong;

        public override void BuildUI(Context UIContext)
        {
            var exampleBar = new Bar(form.Context);
            exampleBar.Label = "BRDF Models";
            exampleBar.Contained = true;

            // here we bind the variables to the renderer variables with their Changed event
            // (their initial value is whatever the renderer currently has set them to)

            var color1Var = new VectorVariable(exampleBar, this.lightDirection.X,
                                                           this.lightDirection.Y,
                                                           this.lightDirection.Z, "axisx=x axisy=y axisz=-z");
            color1Var.Label = "LightDirection";

            color1Var.Changed += delegate {
                this.lightDirection.X = color1Var.X;
                this.lightDirection.Y = color1Var.Y;
                this.lightDirection.Z = color1Var.Z;
            };

            var roughnessOrenNayar = new FloatVariable(exampleBar, 0.5f, "label='Roughness' min=0.0 max=1.0 step=0.01");

            var DiffuseModels = new AntTweakBar.EnumVariable<Diffuse>(exampleBar, Diffuse.OrenNayar, "label= 'Diffuse'");

            var SpecularModels = new AntTweakBar.EnumVariable<Specular>(exampleBar, Specular.BlinnPhong, "label= 'Specular'");

            var ColorObj = new AntTweakBar.Color4Variable(exampleBar, this.ColorObject.X, this.ColorObject.Y, this.ColorObject.Z, this.ColorObject.W);

            ColorObj.Label = "Color Object";

            ColorObj.Changed += delegate {
                this.ColorObject.X = ColorObj.R;
                this.ColorObject.Y = ColorObj.G;
                this.ColorObject.Z = ColorObj.B;
                this.ColorObject.W = ColorObj.A;
            };

            /*botaoStatua.Changed += delegate
            {
                this.modelMesh = //carrega estatua
                this.orenNayarRoughness = //rou

            }*/

            roughnessOrenNayar.Changed += delegate
            {
                this.orenNayarRoughness = roughnessOrenNayar.Value;
            };

            DiffuseModels.Changed += delegate
            {
                this.myDiffuse = DiffuseModels.Value;
            };

            SpecularModels.Changed += delegate
            {
                this.mySpecular = SpecularModels.Value;
            };


        }

        public override void Initialize()
        {
            this.lightDirection = new Vector3(0, 1, 0);
            this.fpsCam = new FPSCam(this.form.ClientSize.Width, this.form.ClientSize.Height);
            this.orenNayarRoughness = 0.5f;
            this.ColorObject = new Vector4(1.0f, 0, 0, 1.0f);
            base.Initialize();
        }

        public override void LoadContent()
        {
            this.cameraCB = new ConstantBuffer<PerFrameCB>(() => { return new PerFrameCB(this.fpsCam.ViewMatrix * this.fpsCam.ProjectionMatrix, this.lightDirection, this.fpsCam.Position, this.orenNayarRoughness, (int)this.myDiffuse, (int)this.mySpecular, this.ColorObject); }, 0, PipelineStages.VertexShader);

            Shader shader = new Shader();
            shader.SetVertexShader("Lightning.hlsl", InputLayoutCreator.PosTexCoordNormal);
            shader.SetPixelShader("Lightning.hlsl");
            Material material = new Material(shader);

            this.modelMesh = MeshLoader.LoadMesh(@"..\..\..\..\ProjectIglooSharpTestModels\Bunny\Bunny.obj", material);
            this.objectCB = new ConstantBuffer<PerObjectCB>(() => { return new PerObjectCB(Matrix.Scaling(1.0f)); }, 1, PipelineStages.VertexShader);
            this.modelMesh.Material.ConstantBuffers.Add(this.objectCB);

            base.LoadContent();
        }

        public override void Update(Timer timer)
        {
            Console.WriteLine(this.fpsCam.Position);
            this.cameraCB.Update();
            this.fpsCam.Update();
            base.Update(timer);
        }

        public override void Draw(Timer timer)
        {
            this.pipelineManager.ImmediateContext.ClearRenderTargetView(this.pipelineManager.BackBufferRTV, Color.CornflowerBlue);
            this.pipelineManager.ImmediateContext.ClearDepthStencilView(this.pipelineManager.DepthStencilView, SharpDX.Direct3D11.DepthStencilClearFlags.Depth | SharpDX.Direct3D11.DepthStencilClearFlags.Stencil, 1.0f, 0);
            this.modelMesh.Draw(timer);

            PipelineManager.Get().ImmediateContext.Rasterizer.State = RasterizerStates.CullCounterClockWise;

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
                this.modelMesh.SafeDispose();
                this.form.SafeDispose();
                this.pipelineManager.SafeDispose();
            }
        }
    }
}
