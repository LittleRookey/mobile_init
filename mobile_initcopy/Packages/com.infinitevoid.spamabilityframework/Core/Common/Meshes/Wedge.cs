using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common.Meshes
{
    /// <summary>
    /// A mesh in the form of a wedge.
    /// </summary>
    public class Wedge
    {
        public Mesh Mesh { get; }
        private Vector3[] _verts;
        private int[] _triangles;
        private int _segments;
        private int _numTriangles;
        private int _numVerts;
        private Vector2[] _uvs;
        private bool _dynamic;

        public Wedge(float distance, float angle, float height, bool dynamic = false)
        {
            Mesh = new Mesh();

            _segments = 10;
            _numTriangles = (_segments * 4) + 2 + 2;
            _numVerts = _numTriangles * 3;
            _verts = new Vector3[_numVerts];
            _triangles = new int[_numVerts];
            _uvs = new Vector2[_verts.Length];
            this._dynamic = dynamic;
            GenerateWedge(distance, angle, height);
        }

        public Wedge Regenerate(float distance, float angle, float height)
        {
            GenerateWedge(distance, angle,height);
            return this;
        }

        private void GenerateWedge(float distance, float angle, float height)
        {
            Vector3 bottomCenter = Vector3.zero;
            Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
            Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

            Vector3 topCenter = bottomCenter + Vector3.up * height;
            Vector3 topLeft = bottomLeft + Vector3.up * height;
            Vector3 topRight = bottomRight + Vector3.up * height;

            int vert = 0;

            // left
            _verts[vert++] = bottomCenter;
            _verts[vert++] = bottomLeft;
            _verts[vert++] = topLeft;

            _verts[vert++] = topLeft;
            _verts[vert++] = topCenter;
            _verts[vert++] = bottomCenter;

            // right
            _verts[vert++] = bottomCenter;
            _verts[vert++] = topCenter;
            _verts[vert++] = topRight;

            _verts[vert++] = topRight;
            _verts[vert++] = bottomRight;
            _verts[vert++] = bottomCenter;

            float currentAngle = -angle;
            float deltaAngle = (angle * 2) / _segments;
            for (int i = 0; i < _segments; ++i)
            {
                bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
                bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

                topLeft = bottomLeft + Vector3.up * height;
                topRight = bottomRight + Vector3.up * height;

                // far
                _verts[vert++] = bottomLeft;
                _verts[vert++] = bottomRight;
                _verts[vert++] = topRight;

                _verts[vert++] = topRight;
                _verts[vert++] = topLeft;
                _verts[vert++] = bottomLeft;

                // top
                _verts[vert++] = topCenter;
                _verts[vert++] = topLeft;
                _verts[vert++] = topRight;

                // bottom
                _verts[vert++] = bottomCenter;
                _verts[vert++] = bottomRight;
                _verts[vert++] = bottomLeft;

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
            Mesh.name = "Wedge";
        }
    }
}