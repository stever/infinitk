using InfiniTK.Utility;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace InfiniTK.Engine
{
    public class Block : IRender
    {
        private readonly MeshObject meshObject;
        private readonly Texture texture;
        private Vector3d position;

        public Block(MeshObject obj, Texture texture)
        {
            meshObject = obj;
            this.texture = texture;
            BoundingBox = BoundingBox.CreateFromSphere(Vector3d.Zero, 0.5f);
        }

        public BoundingBox BoundingBox { get; private set; }

        public Vector3d Position
        {
            get { return position; }
            set
            {
                position = value;
                BoundingBox = BoundingBox.CreateFromSphere(value, 0.5f);
            }
        }

        public void Load()
        {
            texture.Load();
        }

        public void Update(double timeSinceLastUpdate)
        { }

        public void Render()
        {
            GL.PushMatrix();
            GL.Translate(Position);
            GL.BindTexture(TextureTarget.Texture2D, texture.Id);
            meshObject.DrawVectorBufferObject();
            GL.PopMatrix();
        }
    }
}
