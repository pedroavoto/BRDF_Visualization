using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using Assimp.Configs;
using Mesh = Engine.Mesh;
using Buffer = SharpDX.Direct3D11.Buffer;
using SharpDX;
using SharpDX.Direct3D11;
namespace Engine
{
    struct VertexPosTexCoordNormal
    {
        public VertexPosTexCoordNormal(Vector4 pos, Vector2 texCoord, Vector3 normal)
        {
            this.pos = pos;
            this.texCoord = texCoord;
            this.normal = normal;
        }
        Vector4 pos;
        Vector2 texCoord;
        Vector3 normal;
    }

    struct VertexPosTexCoordNormalWeightsBoneIdx
    {
        public VertexPosTexCoordNormalWeightsBoneIdx(Vector4 pos, Vector2 texCoord, Vector3 normal, Vector3 weights, uint[] boneIndices)
        {
            this.pos = pos;
            this.texCoord = texCoord;
            this.normal = normal;
            this.weights = weights;
            this.boneIndices0 = boneIndices[0];
            this.boneIndices1 = boneIndices[1];
            this.boneIndices2 = boneIndices[2];
            this.boneIndices3 = boneIndices[3];
        }

        Vector4 pos; 
	    Vector2 texCoord; 
	    Vector3 normal;
	    Vector3 weights;
	    uint boneIndices0;
        uint boneIndices1;
        uint boneIndices2;
        uint boneIndices3;
    }

    public static class MeshLoader
    {
        private static AssimpContext importer = new AssimpContext();
        private static LogStream logStream = new LogStream( (msg, userData) => Console.WriteLine(msg) );

        static MeshLoader()
        {
            NormalSmoothingAngleConfig config = new NormalSmoothingAngleConfig(80.0f);
            importer.SetConfig(config);
        }

        public static Mesh LoadMesh(string path, Material meshMaterial)
        {
            Scene scene = importer.ImportFile(path, PostProcessSteps.MakeLeftHanded | PostProcessSteps.GenerateSmoothNormals);

            Buffer vertexBuffer = MeshLoader.CreateVertexBufferFromMesh(scene.Meshes[0]);
            Buffer indexBuffer = MeshLoader.CreateIndexBufferFromMesh(scene.Meshes[0]);

            Mesh createdMesh = new Mesh(vertexBuffer, indexBuffer, meshMaterial, 0, scene.Meshes[0].GetIndices().Length);
            return createdMesh;
        }

        private static Buffer CreateVertexBufferFromMesh(Assimp.Mesh mesh)
        {
            List<VertexPosTexCoordNormal> vertexList = new List<VertexPosTexCoordNormal>();

            for (int i = 0; i < mesh.VertexCount; i ++ )
            {
               Vector4 pos = new Vector4(mesh.Vertices[i].X, mesh.Vertices[i].Y, mesh.Vertices[i].Z, 1.0f);
               Vector2 texCoord = new Vector2(0,0);
               Vector3 normal = new Vector3(mesh.Normals[i].X, mesh.Normals[i].Y, mesh.Normals[i].Z);

               VertexPosTexCoordNormal newElem = new VertexPosTexCoordNormal(pos, texCoord, normal);
               vertexList.Add(newElem);
            }

            Buffer ret = Buffer.Create(PipelineManager.Get().Device, SharpDX.Direct3D11.BindFlags.VertexBuffer, vertexList.ToArray(), 0, ResourceUsage.Immutable);

            return ret;
        }

        private static Buffer CreateIndexBufferFromMesh(Assimp.Mesh mesh)
        {
            List<int> indexList = new List<int>();

            foreach (Face f in mesh.Faces)
            {
                f.Indices.ForEach((p) => indexList.Add(p));           
            }

            Buffer ret = Buffer.Create(PipelineManager.Get().Device, BindFlags.IndexBuffer, indexList.ToArray(), 0, ResourceUsage.Immutable);

            return ret;
        }

    }
}
