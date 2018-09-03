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
using System.Globalization;
using System.IO;

namespace MoonPad.Utility.Meshomatic
{
    internal class MeshObjLoader
    {
        public MeshData LoadStream(Stream stream)
        {
            var reader = new StreamReader(stream);

            var points = new List<MeshVector3>();
            var normals = new List<MeshVector3>();
            var texCoords = new List<MeshVector2>();

            var tris = new List<MeshTri>();

            string line;
            char[] splitChars = { ' ' };
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim(splitChars);
                line = line.Replace("  ", " ");

                var parameters = line.Split(splitChars);

                switch (parameters[0])
                {
                    case "p":
                        // Point
                        break;

                    case "v":
                        // Vertex
                        var x = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        var y = float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat);
                        var z = float.Parse(parameters[3], CultureInfo.InvariantCulture.NumberFormat);
                        points.Add(new MeshVector3(x, y, z));
                        break;

                    case "vt":
                        // TexCoord
                        var u = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        var v = float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat);
                        texCoords.Add(new MeshVector2(u, v));
                        break;

                    case "vn":
                        // Normal
                        var nx = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                        var ny = float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat);
                        var nz = float.Parse(parameters[3], CultureInfo.InvariantCulture.NumberFormat);
                        normals.Add(new MeshVector3(nx, ny, nz));
                        break;

                    case "f":
                        // Face
                        tris.AddRange(ParseFace(parameters));
                        break;
                }
            }

            var p = points.ToArray();
            var tc = texCoords.ToArray();
            var n = normals.ToArray();
            var f = tris.ToArray();

            // If there are no specified texcoords or normals, we add a dummy one.
            // That way the Points will have something to refer to.
            if (tc.Length == 0)
            {
                tc = new MeshVector2[1];
                tc[0] = new MeshVector2(0, 0);
            }
            if (n.Length == 0)
            {
                n = new MeshVector3[1];
                n[0] = new MeshVector3(1, 0, 0);
            }

            return new MeshData(p, n, tc, f);
        }

        public MeshData LoadFile(string file)
        {
            using (var s = File.Open(file, FileMode.Open))
                return LoadStream(s);
        }

        private static MeshTri[] ParseFace(string[] indices)
        {
            var p = new MeshPoint[indices.Length - 1];

            for (var i = 0; i < p.Length; i++)
                p[i] = ParsePoint(indices[i + 1]);

            return Triangulate(p);
        }

        /// <summary>
        /// Takes an array of points and returns an array of triangles.
        /// The points form an arbitrary polygon.
        /// </summary>
        /// <param name="ps"></param>
        /// <returns></returns>
        private static MeshTri[] Triangulate(MeshPoint[] ps)
        {
            var ts = new List<MeshTri>();
            if (ps.Length < 3)
                throw new Exception("Invalid shape!  Must have >2 points");

            var lastButOne = ps[1];
            var lastButTwo = ps[0];
            for (var i = 2; i < ps.Length; i++)
            {
                var t = new MeshTri(lastButTwo, lastButOne, ps[i]);
                lastButOne = ps[i];
                lastButTwo = ps[i - 1];
                ts.Add(t);
            }

            return ts.ToArray();
        }

        private static MeshPoint ParsePoint(string s)
        {
            char[] splitChars = { '/' };
            var parameters = s.Split(splitChars);

            var vert = int.Parse(parameters[0]) - 1;
            var tex = 0;
            var norm = 0;

            // Texcoords and normals are optional in .obj files.
            if (parameters[1] != "") tex = int.Parse(parameters[1]) - 1;
            if (parameters[2] != "") norm = int.Parse(parameters[2]) - 1;

            return new MeshPoint(vert, norm, tex);
        }
    }
}
