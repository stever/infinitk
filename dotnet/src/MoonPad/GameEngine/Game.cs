using System;
using System.Collections.Generic;
using System.Reflection;
using MoonPad.Engine;
using MoonPad.Utility;
using log4net;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace MoonPad.GameEngine
{
    internal class Game : SceneViewer
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly GameControls controls;
        private readonly Player controllingPlayer = new Player();
        private readonly FrameTimer frameTimer = new FrameTimer();
        private readonly Terrain terrain = new Terrain("Terrain.png");
        private readonly MeshObject blockTemplate = new MeshObject();
        private readonly HashSet<Player> players = new HashSet<Player>();
        private readonly HashSet<Block> blocks = new HashSet<Block>();

        public Game(GameControls controls) : base(controls)
        {
            this.controls = controls;
        }

        public Game() : this(new GameControls())
        { }

        /// <summary>
        /// This method is used to create the game world and entities within it.
        /// </summary>
        public void SetupGame()
        {
            // Player set-up.
            controllingPlayer.Controls = controls;
            players.Add(controllingPlayer);

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

        public void Load()
        {
            blockTemplate.LoadMeshData("Cube.obj");
            foreach (var entity in blocks) entity.Load();
            foreach (var entity in players) entity.Load();
            frameTimer?.Start();
        }

        public void Update()
        {
            // Compute time since last Idle start.
            var timeSinceLastIdle = frameTimer.ComputeTimeSinceLastFrameStart();

            // Update blocks and players before checking for collisions.
            foreach (var entity in blocks) entity.Update(timeSinceLastIdle);
            foreach (var entity in players) entity.Update(timeSinceLastIdle);

            // Check for collisions.
            var collisions = new HashSet<Tuple<Player, Block>>();
            foreach (var collider in players)
            {
                // Compare players (which move) to blocks (which don't move).
                foreach (var block in blocks)
                {
                    if (!collider.Collides(block)) continue;

                    collisions.Add(Tuple.Create(collider, block));

                    Log.DebugFormat("Collision between {0} ({1:F2}, {2:F2}, {3:F2}) " +
                                    "and {4} ({5:F2}, {6:F2}, {7:F2})",
                        collider.GetType().Name,
                        collider.Position.X,
                        collider.Position.Y,
                        collider.Position.Z,
                        block.GetType().Name,
                        block.Position.X,
                        block.Position.Y,
                        block.Position.Z);
                }
            }

            // Players handle collisions.
            foreach (var collision in collisions)
                collision.Item1.HandleCollision(collision.Item2);

            // Log the amount of time this Idle method takes.
            frameTimer.ComputeTimeSinceIdleStart();
        }

        public void Paint()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            controllingPlayer.ApplyCamera();
            foreach (var entity in blocks) entity.Render();
            foreach (var entity in players) entity.Render();
        }

        public void Reset()
        {
            controls.ResetKeyStates();
            controllingPlayer.MoveToStartPosition();
        }

        public void KeyDown(Key key)
        {
            controls.KeyDown(key);
        }

        public void KeyUp(Key key)
        {
            controls.KeyUp(key);
        }

        public void ResetKeyStates()
        {
            controls.ResetKeyStates();
        }
    }
}
