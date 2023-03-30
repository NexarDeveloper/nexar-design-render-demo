using Assimp;
using Nexar.Renderer.Shapes;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexar.Renderer.GltfRendering
{
    public class GltfDemo
    {
        public void Demo()
        {
            try
            {
                var importer = new AssimpContext();
                var formats = importer.GetSupportedImportFormats();
                var scene = importer.ImportFile("./DemoFiles/IC.gltf");
                //var scene = importer.ImportFile("./DemoFiles/IC.glb", PostProcessSteps.);

                // Loop through all meshes in the scene
                for (int i = 0; i < scene.MeshCount; i++)
                {
                    Mesh mesh = scene.Meshes[i];
                    int vertexCount = mesh.VertexCount;
                    int vertexColorChannelCount = mesh.VertexColorChannelCount;

                    bool hasColours = mesh.HasVertexColors(i);

                    List<Triangle> triangles = new List<Triangle>();

                    // Loop through all faces in the mesh
                    for (int j = 0; j < mesh.FaceCount; j++)
                    {
                        triangles.Clear();
                        Face face = mesh.Faces[j];

                        int indice = face.Indices[0];
                        var vertexColors = mesh.VertexColorChannels[0];
                        var vertexColor = (vertexColors.Count > 0 ? vertexColors[face.Indices[0]] : new Color4D(1.0f, 1.0f, 1.0f, 1.0f));

                        // Create a new triangle from the face indices
                        Vector3D v1 = mesh.Vertices[face.Indices[0]];
                        Vector3D v2 = mesh.Vertices[face.Indices[1]];
                        Vector3D v3 = mesh.Vertices[face.Indices[2]];
                        Triangle triangle = new Triangle(v1.X, v1.Y, v1.Z, v2.X, v2.Y, v2.Z, v3.X, v3.Y, v3.Z);
                        triangles.Add(triangle);

                        var colour = new Color4(vertexColor.R, vertexColor.G, vertexColor.B, vertexColor.A);
                        //threeDModelShader.AddVertices(triangles, 0.0f, colour);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
