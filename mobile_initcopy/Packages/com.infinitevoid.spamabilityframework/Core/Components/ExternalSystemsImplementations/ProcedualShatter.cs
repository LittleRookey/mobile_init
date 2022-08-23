using System;
using System.Collections.Generic;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Components.ExternalSystemsImplementations
{
    /// <summary>
    /// Enables the given mesh to be shattered procedually on demand.
    /// </summary>
    public class ProcedualShatter : MonoBehaviour, IShatterable
    {
        [HelpBox("Set these if the mesh-filter and -renderer component is not on this game-object")]
        [SerializeField] private MeshFilter _meshFilter;
       [SerializeField] private MeshRenderer _meshRenderer;
        public event Action WasShattered;
        public string MeshName => _meshFilter.mesh.name;
        public MeshRenderer Renderer => _meshRenderer;
        
        private bool edgeSet = false;
        private Vector3 edgeVertex = Vector3.zero;
        private Vector2 edgeUV = Vector2.zero;
        private Plane edgePlane = new Plane();

        private int _cutCascades = 3;
        private float _explodeForce = 2500;

        private int _partMinDespawnTime;
        private int _partMaxDespawnTime;

        private void Awake()
        {
            if (!_meshFilter)
                _meshFilter = GetComponent<MeshFilter>();
            if (!_meshRenderer)
                _meshRenderer = GetComponent<MeshRenderer>();
        }

        public void DestroyMesh(int cutCascades, float explodeForce, int partDespawnSecondsMin, int partDespawnSecondsMax)
        {
            _cutCascades = cutCascades;
            _explodeForce = explodeForce;
            _partMinDespawnTime = partDespawnSecondsMin;
            _partMaxDespawnTime = partDespawnSecondsMax;
            var originalMesh = _meshFilter.mesh;
            originalMesh.RecalculateBounds();
            var parts = new List<PartMesh>();
            var subParts = new List<PartMesh>();

            var mainPart = new PartMesh(_partMinDespawnTime, _partMaxDespawnTime)
            {
                UV = originalMesh.uv,
                Vertices = originalMesh.vertices,
                Normals = originalMesh.normals,
                Triangles = new int[originalMesh.subMeshCount][],
                Bounds = originalMesh.bounds
            };

            for (int i = 0; i < originalMesh.subMeshCount; i++)
                mainPart.Triangles[i] = originalMesh.GetTriangles(i);

            parts.Add(mainPart);

            for (var c = 0; c < _cutCascades; c++)
            {
                for (var i = 0; i < parts.Count; i++)
                {
                    var bounds = parts[i].Bounds;
                    bounds.Expand(0.5f);

                    var plane = new Plane(UnityEngine.Random.onUnitSphere, new Vector3(0, 0, 0));

                    subParts.Add(GenerateMesh(parts[i], plane, true));
                    subParts.Add(GenerateMesh(parts[i], plane, false));
                }

                parts = new List<PartMesh>(subParts);
                subParts.Clear();
            }

            for (var i = 0; i < parts.Count; i++)
            {
                parts[i].MakeGameobject(this);
                parts[i].GameObject.GetComponent<Rigidbody>()
                    .AddForceAtPosition(parts[i].Bounds.center * _explodeForce, transform.position);
            }
            WasShattered?.Invoke();
            Destroy(gameObject);
            
        }

        private PartMesh GenerateMesh(PartMesh original, Plane plane, bool left)
        {
            var partMesh = new PartMesh(_partMinDespawnTime, _partMaxDespawnTime) { };
            var ray1 = new Ray();
            var ray2 = new Ray();


            for (var i = 0; i < original.Triangles.Length; i++)
            {
                var triangles = original.Triangles[i];
                edgeSet = false;

                for (var j = 0; j < triangles.Length; j = j + 3)
                {
                    var sideA = plane.GetSide(original.Vertices[triangles[j]]) == left;
                    var sideB = plane.GetSide(original.Vertices[triangles[j + 1]]) == left;
                    var sideC = plane.GetSide(original.Vertices[triangles[j + 2]]) == left;

                    var sideCount = (sideA ? 1 : 0) +
                                    (sideB ? 1 : 0) +
                                    (sideC ? 1 : 0);
                    if (sideCount == 0)
                    {
                        continue;
                    }

                    if (sideCount == 3)
                    {
                        partMesh.AddTriangle(i,
                            original.Vertices[triangles[j]], original.Vertices[triangles[j + 1]],
                            original.Vertices[triangles[j + 2]],
                            original.Normals[triangles[j]], original.Normals[triangles[j + 1]],
                            original.Normals[triangles[j + 2]],
                            original.UV[triangles[j]], original.UV[triangles[j + 1]], original.UV[triangles[j + 2]]);
                        continue;
                    }

                    //cut points
                    var singleIndex = sideB == sideC ? 0 : sideA == sideC ? 1 : 2;

                    ray1.origin = original.Vertices[triangles[j + singleIndex]];
                    var dir1 = original.Vertices[triangles[j + ((singleIndex + 1) % 3)]] -
                               original.Vertices[triangles[j + singleIndex]];
                    ray1.direction = dir1;
                    plane.Raycast(ray1, out var enter1);
                    var lerp1 = enter1 / dir1.magnitude;

                    ray2.origin = original.Vertices[triangles[j + singleIndex]];
                    var dir2 = original.Vertices[triangles[j + ((singleIndex + 2) % 3)]] -
                               original.Vertices[triangles[j + singleIndex]];
                    ray2.direction = dir2;
                    plane.Raycast(ray2, out var enter2);
                    var lerp2 = enter2 / dir2.magnitude;

                    //first vertex = ancor
                    AddEdge(i,
                        partMesh,
                        left ? plane.normal * -1f : plane.normal,
                        ray1.origin + ray1.direction.normalized * enter1,
                        ray2.origin + ray2.direction.normalized * enter2,
                        Vector2.Lerp(original.UV[triangles[j + singleIndex]],
                            original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                        Vector2.Lerp(original.UV[triangles[j + singleIndex]],
                            original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2));

                    if (sideCount == 1)
                    {
                        partMesh.AddTriangle(i,
                            original.Vertices[triangles[j + singleIndex]],
                            //Vector3.Lerp(originalMesh.vertices[triangles[j + singleIndex]], originalMesh.vertices[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            //Vector3.Lerp(originalMesh.vertices[triangles[j + singleIndex]], originalMesh.vertices[triangles[j + ((singleIndex + 2) % 3)]], lerp2),
                            ray1.origin + ray1.direction.normalized * enter1,
                            ray2.origin + ray2.direction.normalized * enter2,
                            original.Normals[triangles[j + singleIndex]],
                            Vector3.Lerp(original.Normals[triangles[j + singleIndex]],
                                original.Normals[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            Vector3.Lerp(original.Normals[triangles[j + singleIndex]],
                                original.Normals[triangles[j + ((singleIndex + 2) % 3)]], lerp2),
                            original.UV[triangles[j + singleIndex]],
                            Vector2.Lerp(original.UV[triangles[j + singleIndex]],
                                original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            Vector2.Lerp(original.UV[triangles[j + singleIndex]],
                                original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2));

                        continue;
                    }

                    if (sideCount == 2)
                    {
                        partMesh.AddTriangle(i,
                            ray1.origin + ray1.direction.normalized * enter1,
                            original.Vertices[triangles[j + ((singleIndex + 1) % 3)]],
                            original.Vertices[triangles[j + ((singleIndex + 2) % 3)]],
                            Vector3.Lerp(original.Normals[triangles[j + singleIndex]],
                                original.Normals[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            original.Normals[triangles[j + ((singleIndex + 1) % 3)]],
                            original.Normals[triangles[j + ((singleIndex + 2) % 3)]],
                            Vector2.Lerp(original.UV[triangles[j + singleIndex]],
                                original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            original.UV[triangles[j + ((singleIndex + 1) % 3)]],
                            original.UV[triangles[j + ((singleIndex + 2) % 3)]]);
                        partMesh.AddTriangle(i,
                            ray1.origin + ray1.direction.normalized * enter1,
                            original.Vertices[triangles[j + ((singleIndex + 2) % 3)]],
                            ray2.origin + ray2.direction.normalized * enter2,
                            Vector3.Lerp(original.Normals[triangles[j + singleIndex]],
                                original.Normals[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            original.Normals[triangles[j + ((singleIndex + 2) % 3)]],
                            Vector3.Lerp(original.Normals[triangles[j + singleIndex]],
                                original.Normals[triangles[j + ((singleIndex + 2) % 3)]], lerp2),
                            Vector2.Lerp(original.UV[triangles[j + singleIndex]],
                                original.UV[triangles[j + ((singleIndex + 1) % 3)]], lerp1),
                            original.UV[triangles[j + ((singleIndex + 2) % 3)]],
                            Vector2.Lerp(original.UV[triangles[j + singleIndex]],
                                original.UV[triangles[j + ((singleIndex + 2) % 3)]], lerp2));
                        continue;
                    }
                }
            }

            partMesh.FillArrays();

            return partMesh;
        }

        private void AddEdge(int subMesh, PartMesh partMesh, Vector3 normal, Vector3 vertex1, Vector3 vertex2, Vector2 uv1,
            Vector2 uv2)
        {
            if (!edgeSet)
            {
                edgeSet = true;
                edgeVertex = vertex1;
                edgeUV = uv1;
            }
            else
            {
                edgePlane.Set3Points(edgeVertex, vertex1, vertex2);

                partMesh.AddTriangle(subMesh,
                    edgeVertex,
                    edgePlane.GetSide(edgeVertex + normal) ? vertex1 : vertex2,
                    edgePlane.GetSide(edgeVertex + normal) ? vertex2 : vertex1,
                    normal, normal, normal, edgeUV, uv1, uv2);
            }
        }

        public class PartMesh
        {
            private readonly float _minDespawnTime;
            private readonly float _maxDespawnTime;
            private List<Vector3> _Verticies = new List<Vector3>();
            private List<Vector3> _Normals = new List<Vector3>();
            private List<List<int>> _Triangles = new List<List<int>>();
            private List<Vector2> _UVs = new List<Vector2>();
            public Vector3[] Vertices;
            public Vector3[] Normals;
            public int[][] Triangles;
            public Vector2[] UV;
            public GameObject GameObject;
            public Bounds Bounds = new Bounds();

            public PartMesh(int minDespawnTime, int maxDespawnTime)
            {
                _minDespawnTime = minDespawnTime;
                _maxDespawnTime = maxDespawnTime;
            }

            public void AddTriangle(int submesh, Vector3 vert1, Vector3 vert2, Vector3 vert3, Vector3 normal1,
                Vector3 normal2, Vector3 normal3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
            {
                if (_Triangles.Count - 1 < submesh)
                    _Triangles.Add(new List<int>());

                _Triangles[submesh].Add(_Verticies.Count);
                _Verticies.Add(vert1);
                _Triangles[submesh].Add(_Verticies.Count);
                _Verticies.Add(vert2);
                _Triangles[submesh].Add(_Verticies.Count);
                _Verticies.Add(vert3);
                _Normals.Add(normal1);
                _Normals.Add(normal2);
                _Normals.Add(normal3);
                _UVs.Add(uv1);
                _UVs.Add(uv2);
                _UVs.Add(uv3);

                Bounds.min = Vector3.Min(Bounds.min, vert1);
                Bounds.min = Vector3.Min(Bounds.min, vert2);
                Bounds.min = Vector3.Min(Bounds.min, vert3);
                Bounds.max = Vector3.Min(Bounds.max, vert1);
                Bounds.max = Vector3.Min(Bounds.max, vert2);
                Bounds.max = Vector3.Min(Bounds.max, vert3);
            }

            public void FillArrays()
            {
                Vertices = _Verticies.ToArray();
                Normals = _Normals.ToArray();
                UV = _UVs.ToArray();
                Triangles = new int[_Triangles.Count][];
                for (var i = 0; i < _Triangles.Count; i++)
                    Triangles[i] = _Triangles[i].ToArray();
            }

            public void MakeGameobject(ProcedualShatter original)
            {
                var originalTransform = original.transform;
                GameObject = new GameObject(original.name);
                GameObject.transform.position = originalTransform.position;
                GameObject.transform.rotation = originalTransform.rotation;
                GameObject.transform.localScale = originalTransform.localScale;

                var mesh = new Mesh();
                mesh.name = original.MeshName;

                mesh.vertices = Vertices;
                mesh.normals = Normals;
                mesh.uv = UV;

                for (var i = 0; i < Triangles.Length; i++)
                    mesh.SetTriangles(Triangles[i], i, true);
                Bounds = mesh.bounds;

                var renderer = GameObject.AddComponent<MeshRenderer>();
                renderer.materials = original.Renderer.materials;

                var filter = GameObject.AddComponent<MeshFilter>();
                filter.mesh = mesh;

                var collider = GameObject.AddComponent<MeshCollider>();
                collider.convex = true;

                GameObject.AddComponent<Rigidbody>();

                Destroy(GameObject, UnityEngine.Random.Range(_minDespawnTime, _maxDespawnTime));
            }
        }

    }
}