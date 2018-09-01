/*
Copyright 2010 Simon Heath. All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are
permitted provided that the following conditions are met:

   1. Redistributions of source code must retain the above copyright notice, this list of
      conditions and the following disclaimer.

   2. Redistributions in binary form must reproduce the above copyright notice, this list
      of conditions and the following disclaimer in the documentation and/or other materials
      provided with the distribution.

THIS SOFTWARE IS PROVIDED BY SIMON HEATH ``AS IS'' AND ANY EXPRESS OR IMPLIED
WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

The views and conclusions contained in the software and documentation are those of the
authors and should not be interpreted as representing official policies, either expressed
or implied, of Simon Heath.
*/

using System;
using System.Collections.Generic;
using System.Text;
using OpenTK;

namespace MoonPad.Utility.Meshomatic
{
    /// <summary>
    /// A class containing all the necessary data for a mesh: Points, normal
    /// vectors, UV coordinates, and indices into each.
    ///
    /// Regardless of how the mesh file represents geometry, this is what we
    /// load it into, because this is most similar to how OpenGL represents
    /// geometry.
    ///
    /// We store data as arrays of vertices, UV coordinates and normals, and
    /// then a list of Triangle structures. A Triangle is a struct which
    /// contains integer offsets into the vertex/normal/texcoord arrays to
    /// define a face.
    /// </summary>
    public class MeshData
    {
        public MeshVector3[] Vertices { get; }
        public MeshVector2[] TexCoords { get; }
        public MeshVector3[] Normals { get; }
        public MeshTri[] Tris { get; }

        /// <summary>
        /// Creates a new MeshData object.
        /// </summary>
        public MeshData(MeshVector3[] vert, MeshVector3[] norm, MeshVector2[] tex, MeshTri[] tri)
        {
            Vertices = vert;
            TexCoords = tex;
            Normals = norm;
            Tris = tri;

            Verify();
        }

        /// <summary>
        /// Returns an array containing the coordinates of all the <value>Vertices</value>.
        /// So {[1,1,1], [2,2,2]} will turn into {1,1,1,2,2,2}
        /// </summary>
        public double[] VertexArray()
        {
            var verts = new double[Vertices.Length * 3];
            for (var i = 0; i < Vertices.Length; i++)
            {
                verts[i * 3] = Vertices[i].X;
                verts[i * 3 + 1] = Vertices[i].Y;
                verts[i * 3 + 2] = Vertices[i].Z;
            }
            return verts;
        }

        /// <summary>
        /// Returns an array containing the coordinates of the <value>Normals</value>,
        /// similar to VertexArray.
        /// </summary>
        public double[] NormalArray()
        {
            var norms = new double[Normals.Length * 3];
            for (var i = 0; i < Normals.Length; i++)
            {
                norms[i * 3] = Normals[i].X;
                norms[i * 3 + 1] = Normals[i].Y;
                norms[i * 3 + 2] = Normals[i].Z;
            }
            return norms;
        }

        /// <summary>
        /// Returns an array containing the coordinates of the <value>TexCoords</value>,
        /// similar to VertexArray.
        /// </summary>
        public double[] TexcoordArray()
        {
            var tcs = new double[TexCoords.Length * 2];
            for (var i = 0; i < TexCoords.Length; i++)
            {
                tcs[i * 3] = TexCoords[i].X;
                tcs[i * 3 + 1] = TexCoords[i].Y;
            }
            return tcs;
        }

        /*
        public void IndexArrays(out int[] verts, out int[] norms, out int[] texcoords)
        {
            List<int> v = new List<int>();
            List<int> n = new List<int>();
            List<int> t = new List<int>();
            foreach(Face f in Faces)
            {
                foreach(Point p in f.Points)
                {
                    v.Add(p.Vertex);
                    n.Add(p.Normal);
                    t.Add(p.TexCoord);
                }
            }
            verts = v.ToArray();
            norms = n.ToArray();
            texcoords = t.ToArray();
        }
        */

        /// <summary>
        ///  Turns the Triangles into an array of Points.
        /// </summary>
        protected MeshPoint[] Points()
        {
            var points = new List<MeshPoint>();
            foreach (var t in Tris)
            {
                points.Add(t.P1);
                points.Add(t.P2);
                points.Add(t.P3);
            }
            return points.ToArray();
        }

        /// <summary>
        /// OpenGL's vertex buffers use the same index to refer to vertices, normals
        /// and floats, and just duplicate data as necessary. So, we do the same.
        /// </summary>
        /// <remarks>
        /// This may or may not be correct, and is certainly not efficient.
        /// But when in doubt, use brute force.
        /// </remarks>
        public void OpenGLArrays(out float[] verts, out float[] norms, out float[] texcoords, out uint[] indices)
        {
            var points = Points();

            verts = new float[points.Length * 3];
            norms = new float[points.Length * 3];
            texcoords = new float[points.Length * 2];

            indices = new uint[points.Length];

            for (uint i = 0; i < points.Length; i++)
            {
                var p = points[i];

                verts[i * 3] = (float) Vertices[p.Vertex].X;
                verts[i * 3 + 1] = (float) Vertices[p.Vertex].Y;
                verts[i * 3 + 2] = (float) Vertices[p.Vertex].Z;

                norms[i * 3] = (float) Normals[p.Normal].X;
                norms[i * 3 + 1] = (float) Normals[p.Normal].Y;
                norms[i * 3 + 2] = (float) Normals[p.Normal].Z;

                texcoords[i * 2] = (float) TexCoords[p.TexCoord].X;
                texcoords[i * 2 + 1] = (float) TexCoords[p.TexCoord].Y;

                indices[i] = i;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Vertices:");
            foreach (var v in Vertices)
                sb.AppendLine(v.ToString());

            sb.AppendLine("Normals:");
            foreach (var n in Normals)
                sb.AppendLine(n.ToString());

            sb.AppendLine("TexCoords:");
            foreach (var t in TexCoords)
                sb.AppendLine(t.ToString());

            sb.AppendLine("Tris:");
            foreach (var t in Tris)
                sb.AppendLine(t.ToString());

            return sb.ToString();
        }

        /// <remarks>
        /// Might technically be incorrect, since a (malformed) file could have
        /// vertices that aren't actually in any face. Don't take the names of
        /// the out parameters too literally...
        /// </remarks>
        public void Dimensions(out double width, out double length, out double height, out Vector3d center)
        {
            double maxX = 0;
            double minX = 0;

            double maxY = 0;
            double minY = 0;

            double maxZ = 0;
            double minZ = 0;

            foreach (var vert in Vertices)
            {
                if (vert.X > maxX) maxX = vert.X;
                if (vert.X < minX) minX = vert.X;

                if (vert.Y > maxY) maxY = vert.Y;
                if (vert.Y < minY) minY = vert.Y;

                if (vert.Z > maxZ) maxZ = vert.Z;
                if (vert.Z < minZ) minZ = vert.Z;
            }

            width = maxX - minX;
            length = maxY - minY;
            height = maxZ - minZ;

            center = new Vector3d(maxX + minX, maxY + minY, maxZ + minZ);
        }

        /// <summary>
        /// Does some simple sanity checking to make sure that the offsets of
        /// the Triangles actually refer to real points. Throws an <exception
        /// cref="IndexOutOfRangeException">IndexOutOfRangeException</exception> if not.
        /// </summary>
        public void Verify()
        {
            foreach (var t in Tris)
            {
                foreach (var p in t.Points())
                {
                    if (p.Vertex >= Vertices.Length)
                    {
                        throw new IndexOutOfRangeException(
                            $"Vertex {p.Vertex} >= length of vertices {Vertices.Length}");
                    }
                    if (p.Normal >= Normals.Length)
                    {
                        throw new IndexOutOfRangeException(
                            $"Normal {p.Normal} >= number of normals {Normals.Length}");
                    }
                    if (p.TexCoord > TexCoords.Length)
                    {
                        throw new IndexOutOfRangeException(
                            $"TexCoord {p.TexCoord} > number of texcoords {TexCoords.Length}");
                    }
                }
            }
        }
    }
}
