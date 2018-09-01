using System;
using System.Reflection;
using InfiniTK.Utility.Meshomatic;
using log4net;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace InfiniTK.Utility
{
    public class MeshObject
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private MeshData meshData;

        #region Box dimensions

        /// <summary>
        /// The width (x) of the object box.
        /// </summary>
        public double BoxWidth { get; private set; }

        /// <summary>
        /// The height (y) of the object box.
        /// </summary>
        public double BoxHeight { get; private set; }

        /// <summary>
        /// The depth (z) of the object box.
        /// </summary>
        public double BoxDepth { get; private set; }

        #endregion

        /// <summary>
        /// The position at the center of the object box.
        /// </summary>
        public Vector3d Center { get; private set; }

        /// <summary>
        /// Scale is a useful property to store with the mesh object. This
        /// property is not used directly in this class but is used to scale
        /// the co-ordinates for the mesh when rendering a modelled object.
        /// </summary>
        public double Scale { get; set; }

        #region VBO variables

        private uint objectDataBuffer;
        private uint objectIndexBuffer;
        private int textureCoordOffset;
        private int objectVerticesOffset;
        private int objectNormalsOffset;

        #endregion

        /// <summary>
        /// Loads the mesh data from Wavefront OBJ file.
        /// </summary>
        /// <param name="filename">OBJ filename.</param>
        public void LoadMeshData(string filename)
        {
            meshData = new MeshObjLoader().LoadFile(filename);
            Log.Debug(meshData);
            LoadObjectBuffers(meshData);
            DetermineBoxDimensions();
        }

        private void DetermineBoxDimensions()
        {
            double w, l, h;
            Vector3d center;
            meshData.Dimensions(out w, out l, out h, out center);
            Log.DebugFormat("Box dimensions: {0} x {1} x {2}", w, l, h);
            Log.DebugFormat("Box center: {0}, {1}, {2}", center.X, center.Y, center.Z);
            BoxWidth = w;
            BoxHeight = h;
            BoxDepth = l;
            Center = center;
        }

        private void LoadObjectBuffers(MeshData data)
        {
            float[] verts, norms, texcoords;
            uint[] indices;
            data.OpenGLArrays(out verts, out norms, out texcoords, out indices);
            GL.GenBuffers(1, out objectDataBuffer);
            GL.GenBuffers(1, out objectIndexBuffer);

            // Set up data for VBO.
            // We're going to use one VBO for all geometry, and stick it in
            // in (VVVVNNNNCCCC) order.  Non interleaved.
            var buffersize = (verts.Length + norms.Length + texcoords.Length);
            var bufferdata = new float[buffersize];
            objectVerticesOffset = 0;
            objectNormalsOffset = verts.Length;
            textureCoordOffset = (verts.Length + norms.Length);

            verts.CopyTo(bufferdata, objectVerticesOffset);
            norms.CopyTo(bufferdata, objectNormalsOffset);
            texcoords.CopyTo(bufferdata, textureCoordOffset);

            var v = false;
            for (var i = textureCoordOffset; i < bufferdata.Length; i++)
            {
                if (v)
                {
                    bufferdata[i] = 1 - bufferdata[i];
                    v = false;
                }
                else
                {
                    v = true;
                }
            }

            // Load geometry data
            GL.BindBuffer(BufferTarget.ArrayBuffer, objectDataBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (buffersize * sizeof(float)), bufferdata, BufferUsageHint.StaticDraw);

            // Load index data
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, objectIndexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr) (indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);
        }

        public void DrawVectorBufferObject()
        {
            // Push current Array Buffer state so we can restore it later
            GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);

            // Texture
            // NOTE: Loaded the texture before calling this method.
            //GL.ClientActiveTexture(TextureUnit.Texture0); // NOTE: This call doesn't seem to be required?

            GL.BindBuffer(BufferTarget.ArrayBuffer, objectDataBuffer);

            // Normal buffer
            GL.NormalPointer(NormalPointerType.Float, 0, (IntPtr) (objectNormalsOffset * sizeof(float)));

            // TexCoord buffer
            GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, (IntPtr) (textureCoordOffset * sizeof(float)));

            // Vertex buffer
            GL.VertexPointer(3, VertexPointerType.Float, 0, (IntPtr) (objectVerticesOffset * sizeof(float)));

            // Index array
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, objectIndexBuffer);
            GL.DrawElements(BeginMode.Triangles, meshData.Tris.Length * 3, DrawElementsType.UnsignedInt, IntPtr.Zero);

            // Restore the state
            GL.PopClientAttrib();
        }

        /*
        /// <summary>
        /// Example of how you would draw things in (deprecated) immediate mode.
        /// </summary>
        public void DrawWithImmediateMode()
        {
            GL.Begin(BeginMode.Triangles);
            foreach (MeshTri t in meshData.Tris)
            {
                foreach (MeshPoint p in t.Points())
                {
                    MeshVector3 v = meshData.Vertices[p.Vertex];
                    MeshVector3 n = meshData.Normals[p.Normal];
                    MeshVector2 tc = meshData.TexCoords[p.TexCoord];
                    GL.Normal3(n.X, n.Y, n.Z);
                    GL.TexCoord2(tc.X, 1 - tc.Y);
                    GL.Vertex3(v.X, v.Y, v.Z);
                }
            }
            GL.End();
        }
        */
    }
}
