using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HyrphusQ.Helpers
{
    public static class MeshHelper
    {
        #region Extension
        public static Mesh CreateInvertedMesh(this Mesh originMesh)
        {
            Vector3[] normals = new Vector3[originMesh.normals.Length];
            Vector4[] tangents = new Vector4[originMesh.tangents.Length];

            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = -originMesh.normals[i];
            }
            for (int i = 0; i < tangents.Length; i++)
            {
                tangents[i] = originMesh.tangents[i];
                tangents[i].w = -tangents[i].w;
            }

            return new Mesh()
            {
                vertices = originMesh.vertices,
                uv = originMesh.uv,
                normals = normals,
                tangents = tangents,
                triangles = originMesh.triangles.Reverse().ToArray()
            };
        }
        #endregion

        public static Mesh CombineMeshes(params Tuple<Mesh, Matrix4x4?>[] meshes)
        {
            if(meshes == null || meshes.Length <= 1)
            {
                Debug.LogError("Meshes length need to greater than 2!!!");
                return null;
            }
            CombineInstance[] combine = new CombineInstance[meshes.Length];
            for (int i = 0; i < meshes.Length; i++)
            {
                if (!meshes[i].Item1.isReadable)
                {
                    Debug.LogError($"Mesh {meshes[i].Item1.name} is not Readable");
                    return null;
                }
                combine[i].mesh = meshes[i].Item1;
                combine[i].transform = meshes[i].Item2 ?? Matrix4x4.identity;
            }
            var mesh = new Mesh();
            mesh.CombineMeshes(combine);
            return mesh;
        }
        public static Mesh CombineMeshes(params MeshFilter[] meshFilters)
        {
            if (meshFilters == null || meshFilters.Length <= 1)
            {
                Debug.LogError("Meshes length need to greater than 2!!!");
                return null;
            }
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            for (int i = 0; i < meshFilters.Length; i++)
            {
                if (!meshFilters[i].mesh.isReadable)
                {
                    Debug.LogError($"Mesh {meshFilters[i].mesh.name} is not Readable");
                    return null;
                }
                combine[i].mesh = meshFilters[i].mesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            }
            var mesh = new Mesh();
            mesh.CombineMeshes(combine);
            return mesh;
        }
    }
}