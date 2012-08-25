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
        private readonly MeshObject _meshObject = new MeshObject();
        private Vector3d _relativePosition;
        private readonly Texture _texture = new Texture("Starfield.png");

        #region IPosition implementation

        public Vector3d Position { get; set; }
        
        #endregion

        #region IRender implementation

        /// <summary>
        /// Loads the sky and applies the starfield texture.
        /// </summary>
        public void Load()
        {
            _meshObject.LoadMeshData("Sphere.obj");
            _meshObject.Scale = 7.0f;
            _relativePosition = -(_meshObject.Center * _meshObject.Scale) / 2;
            _texture.Load();
        }

        public void Update(double timeSinceLastUpdate)
        {
            // Nothing to do.
        }

        public void Render()
        {
            GL.PushMatrix();
            GL.Translate(Vector3d.Add(Position, _relativePosition));
            double scale = _meshObject.Scale;
            GL.Scale(scale, scale, scale);
            GL.BindTexture(TextureTarget.Texture2D, _texture.ID);
            _meshObject.DrawVectorBufferObject();
            GL.PopMatrix();
        }

        #endregion
    }
}
