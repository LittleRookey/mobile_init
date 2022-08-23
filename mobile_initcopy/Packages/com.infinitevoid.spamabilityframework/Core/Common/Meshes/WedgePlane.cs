using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common.Meshes
{
    /// <summary>
    /// A mesh in the form of a wedge with no height.
    /// </summary>
    public class WedgePlane
    {
        public Mesh Mesh { get; }
        private readonly Vector3[] _verts;
        private readonly int[] _triangles;
        private readonly bool _dynamic;
        private readonly int _segments;
        private readonly int _numVerts;
        private Vector2[] _uvs;

        public WedgePlane(float distance, float angle, bool dynamic = false)
        {
            this._dynamic = dynamic;
            Mesh = new Mesh();
            _segments = 10;
            int numTriangles = (_segments * 4) + 2 + 2;
            _numVerts = numTriangles * 3;
            _verts = new Vector3[_numVerts];
            _triangles = new int[_numVerts];
            _uvs = new Vector2[_verts.Length];
            CreateWedgePlane(distance, angle);
        }

        public WedgePlane Regenerate(float distance, float angle)
        {
            CreateWedgePlane(distance, angle);
            return this;
        }

        private void CreateWedgePlane(float distance, float angle)
        {
            if (!Mesh) return;
            Vector3 center = Vector3.zero;
            int vert = 0;
            float currentAngle = -angle;
            float deltaAngle = (angle * 2) / _segments;
            for (int i = 0; i < _segments; ++i)
            {
                var left = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
                var right = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

                _verts[vert++] = center;
                _verts[vert++] = left;
                _verts[vert++] = right;

                currentAngle += deltaAngle;
            }


            for (int i = 0; i < _numVerts; ++i)
            {
                _triangles[i] = i;
            }

            for (int i = 0; i < _uvs.Length; i++)
            {
                _uvs[i] = new Vector2(_verts[i].x, _verts[i].z);
            }

            Mesh.vertices = _verts;
            Mesh.triangles = _triangles;
            Mesh.uv = _uvs;
            Mesh.RecalculateNormals();
            if(_dynamic)
                Mesh.MarkDynamic();
            Mesh.name = "Wedge plane";
        }
    }
}