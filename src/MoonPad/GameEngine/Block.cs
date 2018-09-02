using MoonPad.Engine;
using MoonPad.Utility;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MoonPad.GameEngine
{
    public class Block : SceneObject
    {
        private readonly MeshObject meshObject;
        private readonly Texture texture;

        public Block(MeshObject obj, Texture texture)
        {
            meshObject = obj;
            this.texture = texture;
            BoundingBox = BoundingBox.CreateFromSphere(Vector3d.Zero, 0.5f);
        }

        public BoundingBox BoundingBox { get; private set; }

        public new Vector3d Position
        {
            get { return base.Position; }
            set
            {
                base.Position = value;
                BoundingBox = BoundingBox.CreateFromSphere(value, 0.5f);
            }
        }

        public override void Load()
        {
            texture.Load();
        }

        public override void Update(double timeSinceLastUpdate)
        { }

        public override void Render()
        {
            GL.PushMatrix();
            GL.Translate(Position);
            GL.BindTexture(TextureTarget.Texture2D, texture.Id);
            meshObject.DrawVectorBufferObject();
            GL.PopMatrix();
        }
    }
}
