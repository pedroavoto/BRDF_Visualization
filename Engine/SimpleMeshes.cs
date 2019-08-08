using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Engine
{
    public class SimpleMeshes
    {

        public static Mesh ScreenSpaceTriangle()
        {
            Buffer vertices = Buffer.Create(PipelineManager.Get().Device, BindFlags.VertexBuffer, new[]
                                  {
                                      new Vector4(0.0f, 0.5f, 0.5f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4(-0.5f, -0.5f, 0.5f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f)
                                  });
            Shader shader = new Shader();

            shader.SetVertexShader("MiniTri.fx", InputLayoutCreator.PosColor);
            shader.SetPixelShader("MiniTri.fx");

            Material material = new Material(shader);

            Mesh screenSpaceTriMesh = new Mesh(vertices, null, material, 3, 0);

            return screenSpaceTriMesh;
        }
    }
}
