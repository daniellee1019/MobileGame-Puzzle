using System.Collections.Generic;
using UnityEngine;

namespace Cube.Battle
{
    public class SkinnedMeshNormalAverage : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer skinnedMesh;

        private void Awake()
        {
            Mesh tempMesh = skinnedMesh.sharedMesh;
            MeshNormalAverage(tempMesh);
            skinnedMesh.sharedMesh = tempMesh;
        }

        private void MeshNormalAverage(Mesh mesh)
        {
            Dictionary<Vector3, List<int>> dicVertices = new Dictionary<Vector3, List<int>>();

            for (int i = 0; i < mesh.vertexCount; ++i)
            {
                if (!dicVertices.ContainsKey(mesh.vertices[i]))
                {
                    dicVertices.Add(mesh.vertices[i], new List<int>());
                }

                dicVertices[mesh.vertices[i]].Add(i);
            }

            Vector3[] normals = mesh.normals;
            Vector3 normal;

            foreach (var p in dicVertices)
            {
                normal = Vector3.zero;

                foreach (int n in p.Value)
                {
                    normal += mesh.normals[n];
                }

                normal /= p.Value.Count;

                foreach (int n in p.Value)
                {
                    normals[n] = normal;
                }
            }

            mesh.normals = normals;
        }
    }
}
