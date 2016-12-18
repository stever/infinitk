using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using InfiniTK.Artifacts;
using InfiniTK.Utility;
using log4net;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace InfiniTK.GameEngine
{
    public class GameEngine
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // NOTE: 60 - 100 used in Minecraft. 45 was used in the original example.
        private const float PerspectiveFovDegrees = 60;

        /*
        private const string V_SHADER_SOURCE = @"
void main()
{
	gl_Position = ftransform();
	gl_TexCoord[0] = gl_MultiTexCoord0;
}
";

        private const string F_SHADER_SOURCE = @"
uniform sampler2D tex;
void main()
{
	gl_FragColor = texture2D(tex, gl_TexCoord[0].st);
}
";
        */

        #region Gameplay variables

        private readonly Player player = new Player();
        private readonly InputStates controls = new InputStates();
        private readonly FrameTimer frameTimer = new FrameTimer();
        private readonly Terrain terrain = new Terrain("Terrain.png");
        private readonly MeshObject blockTemplate = new MeshObject();
        private Point restoreMousePosition;

        // Collections. NOTE: Probably require some efficient way to remove items from these.
        // NOTE: I've read that it would be better to use arrays here. Worth testing that theory.
        private readonly List<Block> blocks = new List<Block>();
        private readonly List<ICollide> colliders = new List<ICollide>();

        #endregion

        #region Initialisation methods

        /// <summary>
        /// This method is called during form Load to initialise GL.
        /// </summary>
        public void SetupGL()
        {
            GL.ClearColor(Color.SkyBlue);

            GL.Enable(EnableCap.DepthTest); // TODO: Work with transparent textures.
            GL.Enable(EnableCap.Texture2D);

            // Alpha blending.
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            // TODO: Determine purpose of the following.
            GL.Disable(EnableCap.CullFace);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            // Required for VBO drawing.
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            /*
            // Compile shaders. TODO: Make more of shaders.
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(vertexShader, V_SHADER_SOURCE);
            GL.ShaderSource(fragmentShader, F_SHADER_SOURCE);
            GL.CompileShader(vertexShader);
            GL.CompileShader(fragmentShader);
            Console.Write(GL.GetShaderInfoLog(vertexShader));
            Console.Write(GL.GetShaderInfoLog(fragmentShader));

            // Create and link the shader program.
            int shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);
            Console.Write(GL.GetProgramInfoLog(shaderProgram));
            GL.UseProgram(shaderProgram);
            */
        }

        /// <summary>
        /// This method is used to set-up the GL control viewport, using the dimensions of the GL control.
        /// This method is called on initial form load, and also when the GL control is resized.
        /// </summary>
        public void SetupViewport(int width, int height)
        {
            var fov = MathHelper.DegreesToRadians(PerspectiveFovDegrees);
            var aspectRatio = (float) width / height;
            const float near = 0.1f;
            const float far = 64000f;
            var perspective
                = Matrix4.CreatePerspectiveFieldOfView(fov, aspectRatio, near, far);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
            GL.Viewport(0, 0, width, height);
        }

        /// <summary>
        /// This method is used to create the game world and entities within it.
        /// </summary>
        public void SetupGame()
        {
            // Player set-up.
            player.Controls = controls;
            colliders.Add(player);

            // Surface blocks.
            const int n = 20;
            for (var z = -n / 2; z < n / 2; z++)
            {
                for (var x = -n / 2; x < n / 2; x++)
                {
                    var block = new Block(blockTemplate, terrain.TextureMaps[3, 0]);
                    block.Position = new Vector3d(x, 0, z);
                    blocks.Add(block);
                }
            }

            // Tree trunk.
            for (var i = 1; i <= 2; i++)
            {
                var block = new Block(blockTemplate, terrain.TextureMaps[4, 1]);
                block.Position = new Vector3d(2, i, 0);
                blocks.Add(block);
            }

            // Tree trunk.
            for (var i = 1; i <= 3; i++)
            {
                if (i == 2) continue; // Skip middle block.
                var block = new Block(blockTemplate, terrain.TextureMaps[4, 1]);
                block.Position = new Vector3d(2, i, -4);
                blocks.Add(block);
            }

            {
                // Diamond block.
                var block = new Block(blockTemplate, terrain.TextureMaps[2, 3]);
                block.Position = new Vector3d(2, 1, 2);
                blocks.Add(block);
            }

            {
                // Grass block.
                var block = new Block(blockTemplate, terrain.TextureMaps[3, 0]);
                block.Position = new Vector3d(2, 2, -2);
                blocks.Add(block);
            }
        }

        #endregion

        public bool LimitFrameRate
        {
            get { return frameTimer.LimitFrameRate; }
            set { frameTimer.LimitFrameRate = value; }
        }

        public void Load()
        {
            blockTemplate.LoadMeshData("Cube.obj");
            foreach (var entity in blocks) entity.Load();
            foreach (var entity in colliders) entity.Load();
            frameTimer?.Start();
        }

        public void Update()
        {
            // Compute time since last Idle start.
            var timeSinceLastIdle = frameTimer.ComputeTimeSinceLastFrameStart();

            // Update blocks and colliders before checking for collisions.
            foreach (var entity in blocks) entity.Update(timeSinceLastIdle);
            foreach (var entity in colliders) entity.Update(timeSinceLastIdle);

            // Check for collisions.
            var blockCollisions = new List<BlockCollision>();
            var colliderCollisions = new List<ColliderCollision>();
            foreach (var collider in colliders)
            {
                // Colliders could collide with each other.
                foreach (var other in colliders)
                {
                    if (other == collider) continue;
                    if (!collider.Collides(other)) continue;
                    if (Log.IsDebugEnabled) LogCollision(collider, other);
                    colliderCollisions.Add(new ColliderCollision(collider, other));
                }

                // Compare colliders (which move) to blocks (which don't move).
                foreach (var block in blocks)
                {
                    if (!collider.Collides(block)) continue;
                    if (Log.IsDebugEnabled) LogCollision(collider, block);
                    blockCollisions.Add(new BlockCollision(collider, block));
                }
            }

            // Colliders handle collisions.
            foreach (var collision in blockCollisions)
                collision.Collider.HandleCollision(collision.Block);
            foreach (var collision in colliderCollisions)
                collision.Collider.HandleCollision(collision.Other);

            // Log the amount of time this Idle method takes.
            frameTimer.ComputeTimeSinceIdleStart();
        }

        private static void LogCollision(IPosition collider, IPosition entity)
        {
            Log.DebugFormat("Collision between {0} ({1:F2}, {2:F2}, {3:F2}) " +
                            "and {4} ({5:F2}, {6:F2}, {7:F2})",
                collider.GetType().Name, 
                collider.Position.X, 
                collider.Position.Y, 
                collider.Position.Z,
                entity.GetType().Name, 
                entity.Position.X, 
                entity.Position.Y, 
                entity.Position.Z);
        }

        public void Paint()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            player.ApplyCamera();
            foreach (var entity in blocks) entity.Render();
            foreach (var entity in colliders) entity.Render();
        }

        public void Reset()
        {
            controls.Reset();
            player.MoveToStartPosition();
        }

        public void KeyDown(Key key)
        {
            controls.KeyDown(key);
        }

        public void KeyUp(Key key)
        {
            controls.KeyUp(key);
        }

        public void EnableMouseControl()
        {
            if (controls.MouseControlEnabled) 
                DisableMouseControl();

            restoreMousePosition = Cursor.Position;
            Cursor.Hide();

            Utility.Mouse.SnapshotCurrentMouseState();

            controls.MouseControlEnabled = true;
        }

        public void DisableMouseControl()
        {
            if (!controls.MouseControlEnabled) return;

            Cursor.Position = restoreMousePosition;
            Cursor.Show();

            controls.MouseControlEnabled = false;
        }

        public void ResetKeyStates()
        {
            controls.ResetKeyStates();
        }
    }
}
