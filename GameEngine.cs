using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace InfiniTK
{
    public class GameEngine
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Constants

        private const float PERSPECTIVE_FOV_DEGREES = 45; // NOTE: 60 - 100 used in Minecraft. 45 was used in the original example.

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

        #endregion

        #region Gameplay variables
        private readonly Player _player = new Player();
        private readonly InputStates _controls = new InputStates();
        private readonly FrameTimer _frameTimer = new FrameTimer();
        private readonly Terrain _terrain = new Terrain("Terrain.png");
        private readonly MeshObject _blockTemplate = new MeshObject();
        private Point _restoreMousePosition;

        // Collections. NOTE: Probably require some efficient way to remove items from these.
        // NOTE: I've read that it would be better to use arrays here. Worth testing that theory.
        private readonly List<Block> _blocks = new List<Block>();
        private readonly List<ICollide> _colliders = new List<ICollide>();
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
            //GL.Disable(EnableCap.CullFace);
            //GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

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
            float fov = MathHelper.DegreesToRadians(PERSPECTIVE_FOV_DEGREES);
            float aspectRatio = (float) width / height;
            const float near = 0.1f;
            const float far = 64000f;
            Matrix4 perspective
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
            _player.Controls = _controls;
            _colliders.Add(_player);

            // Surface blocks.
            const int n = 20;
            for (int z = -n / 2; z < n / 2; z++)
            {
                for (int x = -n / 2; x < n / 2; x++)
                {
                    Block block = new Block(_blockTemplate, _terrain.TextureMaps[3, 0]);
                    block.Position = new Vector3d(x, 0, z);
                    _blocks.Add(block);
                }
            }

            // Tree trunk.
            for (int i = 1; i <= 2; i++)
            {
                Block block = new Block(_blockTemplate, _terrain.TextureMaps[4, 1]);
                block.Position = new Vector3d(2, i, 0);
                _blocks.Add(block);
            }

            // Tree trunk.
            for (int i = 1; i <= 3; i++)
            {
                if (i == 2) continue; // Skip middle block.
                Block block = new Block(_blockTemplate, _terrain.TextureMaps[4, 1]);
                block.Position = new Vector3d(2, i, -4);
                _blocks.Add(block);
            }

            {
                // Diamond block.
                Block block = new Block(_blockTemplate, _terrain.TextureMaps[2, 3]);
                block.Position = new Vector3d(2, 1, 2);
                _blocks.Add(block);
            }

            {
                // Grass block.
                Block block = new Block(_blockTemplate, _terrain.TextureMaps[3, 0]);
                block.Position = new Vector3d(2, 2, -2);
                _blocks.Add(block);
            }
        }

        #endregion

        public bool LimitFrameRate
        {
            get { return _frameTimer.LimitFrameRate; }
            set { _frameTimer.LimitFrameRate = value; }
        }

        public void Load()
        {
            _blockTemplate.LoadMeshData("Cube.obj");
            foreach (Block entity in _blocks) entity.Load();
            foreach (ICollide entity in _colliders) entity.Load();
            if (_frameTimer != null) _frameTimer.Start();
        }

        public void Update()
        {
            // Compute time since last Idle start.
            double timeSinceLastIdle = _frameTimer.ComputeTimeSinceLastFrameStart();

            // Update blocks and colliders before checking for collisions.
            foreach (Block entity in _blocks) entity.Update(timeSinceLastIdle);
            foreach (ICollide entity in _colliders) entity.Update(timeSinceLastIdle);

            // Check for collisions.
            List<BlockCollision> blockCollisions = new List<BlockCollision>();
            List<ColliderCollision> colliderCollisions = new List<ColliderCollision>();
            foreach (ICollide collider in _colliders)
            {
                // Colliders could collide with each other.
                foreach (ICollide other in _colliders)
                {
                    if (other == collider) continue;
                    if (!collider.Collides(other)) continue;
                    if (Log.IsDebugEnabled) LogCollision(collider, other);
                    colliderCollisions.Add(new ColliderCollision(collider, other));
                }

                // Compare colliders (which move) to blocks (which don't move).
                foreach (Block block in _blocks)
                {
                    if (!collider.Collides(block)) continue;
                    if (Log.IsDebugEnabled) LogCollision(collider, block);
                    blockCollisions.Add(new BlockCollision(collider, block));
                }
            }

            // Colliders handle collisions.
            foreach (BlockCollision collision in blockCollisions)
                collision.Collider.HandleCollision(collision.Block);
            foreach (ColliderCollision collision in colliderCollisions)
                collision.Collider.HandleCollision(collision.Other);

            // Log the amount of time this Idle method takes.
            _frameTimer.ComputeTimeSinceIdleStart();
        }

        private static void LogCollision(IPosition collider, IPosition entity)
        {
            Log.DebugFormat("Collision between {0} ({1}, {2}, {3}) and {4} ({5}, {6}, {7})",
                collider.GetType().Name,
                collider.Position.X.ToString("F2"),
                collider.Position.Y.ToString("F2"),
                collider.Position.Z.ToString("F2"),
                entity.GetType().Name,
                entity.Position.X.ToString("F2"),
                entity.Position.Y.ToString("F2"),
                entity.Position.Z.ToString("F2"));
        }

        public void Paint()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            _player.ApplyCamera();
            foreach (Block entity in _blocks) entity.Render();
            foreach (ICollide entity in _colliders) entity.Render();
        }

        public void Reset()
        {
            _controls.Reset();
            _player.MoveToStartPosition();
        }

        public void KeyDown(Key key)
        {
            _controls.KeyDown(key);
        }

        public void KeyUp(Key key)
        {
            _controls.KeyUp(key);
        }

        public void EnableMouseControl()
        {
            if (_controls.MouseControlEnabled) 
                DisableMouseControl();

            _restoreMousePosition = Cursor.Position;
            Cursor.Hide();

            _controls.SnapshotCurrentMouseState();
            _controls.MouseControlEnabled = true;
        }

        public void DisableMouseControl()
        {
            if (!_controls.MouseControlEnabled) return;

            Cursor.Position = _restoreMousePosition;
            Cursor.Show();

            _controls.MouseControlEnabled = false;
        }

        public void ResetKeyStates()
        {
            _controls.ResetKeyStates();
        }
    }
}
