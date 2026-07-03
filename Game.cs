using minecrap.graphics;
using minecrap.world;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace minecrap
{
    internal class Game : GameWindow
    {
        private int width;
        private int height;
        private ShaderProgram shaderProgram;
        private Camera cam;
        private Player player;
        private World world;

        public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.width = width;
            this.height = height;
            CenterWindow(new Vector2i(width, height));
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            width = e.Width;
            height = e.Height;
            cam?.SetSize(width, height);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            shaderProgram = new ShaderProgram("default.vert", "default.frag");
            world = new World(new Random().Next(int.MinValue, int.MaxValue), shaderProgram);

            Vector2i worldSize = new(16, 16);
            world.GenerateWorld(worldSize);

            GL.Enable(EnableCap.DepthTest);
            GL.FrontFace(FrontFaceDirection.Cw);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(TriangleFace.Back);

            Vector3 spawnPos = new Vector3(worldSize.X * 8, 64, worldSize.Y * 8);
            cam = new Camera(width, height, spawnPos + new Vector3(0, 0.5f, 0));
            player = new Player(spawnPos);

            CursorState = CursorState.Grabbed;
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(0.5f, 0.5f, 1f, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 model = Matrix4.Identity;
            Matrix4 view = cam.GetViewMatrix();
            Matrix4 projection = cam.GetProjectionMatrix();

            int modelLocation = GL.GetUniformLocation(shaderProgram.ID, "model");
            int viewLocation = GL.GetUniformLocation(shaderProgram.ID, "view");
            int projectionLocation = GL.GetUniformLocation(shaderProgram.ID, "projection");
            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);
            world.RenderWorld();

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            MouseState mouse = MouseState;
            KeyboardState input = KeyboardState;
            base.OnUpdateFrame(args);
            cam.Update(mouse, args);
            player.Update(input, mouse, args);
        }
    }
}