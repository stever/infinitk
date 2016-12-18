using InfiniTK.Meshomatic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace InfiniTK
{
    /// <summary>
    /// Stars in the sky using a starfield texture from 
    /// http://paulbourke.net/miscellaneous/starfield/
    /// </summary>
    public class Starfield : IRender
    {
        private readonly MeshObject meshObject = new MeshObject();
        private Vector3d relativePosition;
        private readonly Texture texture = new Texture("Starfield.png");

        #region IPosition implementation

        public Vector3d Position { get; set; }
        
        #endregion

        #region IRender implementation

        /// <summary>
        /// Loads the sky and applies the starfield texture.
        /// </summary>
        public void Load()
        {
            meshObject.LoadMeshData("Sphere.obj");
            meshObject.Scale = 7.0f;
            relativePosition = -(meshObject.Center * meshObject.Scale) / 2;
            texture.Load();
        }

        public void Update(double timeSinceLastUpdate)
        {
            // Nothing to do.
        }

        public void Render()
        {
            GL.PushMatrix();
            GL.Translate(Vector3d.Add(Position, relativePosition));
            var scale = meshObject.Scale;
            GL.Scale(scale, scale, scale);
            GL.BindTexture(TextureTarget.Texture2D, texture.Id);
            meshObject.DrawVectorBufferObject();
            GL.PopMatrix();
        }

        #endregion
    }
}
