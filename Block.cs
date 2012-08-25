using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace InfiniTK
{
    public class Block : IRender
    {
        private readonly MeshObject _meshObject;
        private readonly Texture _texture;

        public BoundingBox BoundingBox { get; private set; }

        public Block(MeshObject obj, Texture texture)
        {
            _meshObject = obj;
            _texture = texture;
            BoundingBox = BoundingBox.CreateFromSphere(Vector3d.Zero, 0.5f);
        }

        #region IPosition implementation

        private Vector3d _position;

        public Vector3d Position
        {
            get { return _position; }
            set
            {
                _position = value;
                BoundingBox = BoundingBox.CreateFromSphere(value, 0.5f);
            }
        }

        #endregion

        #region IRender implementation

        public void Load()
        {
            _texture.Load();
        }

        public void Update(double timeSinceLastUpdate)
        {
            // Nothing to do.
        }

        public void Render()
        {
            GL.PushMatrix();
            GL.Translate(Position);
            GL.BindTexture(TextureTarget.Texture2D, _texture.ID);
            _meshObject.DrawVectorBufferObject();
            GL.PopMatrix();
        }

        #endregion
    }
}
