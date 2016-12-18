using System.Drawing;
using System.Windows.Forms;
using InfiniTK.Utility;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace InfiniTK.Engine
{
    public class SceneViewer
    {
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

        protected readonly SceneViewerControls Controls = new SceneViewerControls();

        private Point restoreMousePosition;

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
            var aspectRatio = (float)width / height;
            const float near = 0.1f;
            const float far = 64000f;
            var perspective = Matrix4.CreatePerspectiveFieldOfView(fov, aspectRatio, near, far);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
            GL.Viewport(0, 0, width, height);
        }

        public void EnableMouseControl()
        {
            if (Controls.MouseControlEnabled)
                DisableMouseControl();

            restoreMousePosition = Cursor.Position;
            Cursor.Hide();

            Mouse.SnapshotCurrentMouseState();

            Controls.MouseControlEnabled = true;
        }

        public void DisableMouseControl()
        {
            if (!Controls.MouseControlEnabled) return;

            Cursor.Position = restoreMousePosition;
            Cursor.Show();

            Controls.MouseControlEnabled = false;
        }
    }
}
