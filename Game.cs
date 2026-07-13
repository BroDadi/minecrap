using minecrap.graphics;
using minecrap.gui;
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
        public int width, height;
        public static Game instance;
        private ShaderProgram shaderProgram;
        private Camera cam;
        private Player player;
        private World world;
        private GUI gui;
        public UIBlock block; // temp stuff
        private Vector3 skyColor;
        public static HashSet<BlockType> transparentBlocks = new()
        {
            BlockType.Water,
        };
        public static HashSet<BlockType> cutoutBlocks = new()
        {
            BlockType.Glass,
            BlockType.Sapling,
        };
        public static HashSet<BlockType> doubleSidedBlocks = new()
        {
            BlockType.Water,
            BlockType.Sapling,
        };
        public static HashSet<BlockType> noColliderBlocks = new()
        {
            BlockType.Air,
            BlockType.Water,
            BlockType.Sapling,
        };
        public static HashSet<BlockType> renderAsSprite = new()
        {
            BlockType.Sapling,
        };
        public static Faces[] allFaces = Enum.GetValues<Faces>();

        public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.width = width;
            this.height = height;
            instance = this;
            CenterWindow(new Vector2i(width, height));
            skyColor = new(0.5f, 0.6f, 1f);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            width = e.Width;
            height = e.Height;
            gui?.RebuildGUI();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            Title = "MINECRAP!!!";

            shaderProgram = new ShaderProgram("default.vert", "default.frag");

            world = new World(new Random().Next(int.MinValue, int.MaxValue), shaderProgram);
            Vector2i worldSize = new(16, 16);
            world.GenerateWorld(worldSize);

            GL.Enable(EnableCap.DepthTest);
            GL.FrontFace(FrontFaceDirection.Cw);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(TriangleFace.Back);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            gui = new GUI();
            UIImage image = new(new Vector2(0.025f, 0.025f), Vector2.Zero, new Vector2(0.5f, 0.5f), Vector2.Zero, new Texture("crosshair"), 1f, DomAxis.Height);
            gui.AddToGUI(image);
            block = new(BlockType.Dirt, new Vector2(0.2f, 0.2f), Vector2.Zero, new Vector2(1f, 1f), new Vector2(-10f, -10f), 1f, DomAxis.Height, new Vector2(1f, 1f));
            gui.AddToGUI(block);

            Vector2i spawnPos = new(worldSize.X * 8, worldSize.Y * 8);
            Vector3 playerPos = world.GetHighestBlock(spawnPos).pos + new Vector3(0, 1.5f, 0);
            cam = new Camera(playerPos + new Vector3(0f, 0.5f, 0f));
            player = new Player(playerPos);

            CursorState = CursorState.Grabbed;
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.ClearColor(skyColor.X, skyColor.Y, skyColor.Z, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 model = Matrix4.Identity;
            Matrix4 view = cam.GetViewMatrix();
            Matrix4 projection = cam.GetProjectionMatrix();

            int modelLocation = GL.GetUniformLocation(shaderProgram.ID, "model");
            int viewLocation = GL.GetUniformLocation(shaderProgram.ID, "view");
            int projectionLocation = GL.GetUniformLocation(shaderProgram.ID, "projection");
            int skyColorLocation = GL.GetUniformLocation(shaderProgram.ID, "skyColor");
            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);
            GL.Uniform3(skyColorLocation, skyColor);

            world.RenderWorld();

            GL.Disable(EnableCap.DepthTest);
            view = Matrix4.Identity;
            projection = Matrix4.CreateOrthographicOffCenter(0, 1, 0, 1, -1, 1);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);
            gui.Render(shaderProgram);
            GL.Enable(EnableCap.DepthTest);

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
            world.Update(args);
        }
    }
}