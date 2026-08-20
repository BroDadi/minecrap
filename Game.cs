using minecrap.graphics;
using minecrap.gui;
using minecrap.world;
using NativeFileDialogSharp;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace minecrap
{
    internal class Game : GameWindow
    {
        public Vector2i screenSize;
        public static Game instance;
        private ShaderProgram shaderProgram;
        private Camera cam;
        private Player player;
        private World world;
        private GUI gui;
        private GUI pauseGUI;
        private UIBlock[] invBlocks;
        private UIImage select;
        private UIImage fullInv;
        private Vector3 skyColor;
        public bool inInventory;
        public bool paused;
        public static HashSet<BlockType> transparentBlocks =
        [
            BlockType.Water,
        ];
        public static HashSet<BlockType> cutoutBlocks =
        [
            BlockType.Glass,
            BlockType.Sapling,
            BlockType.Leaves,
        ];
        public static HashSet<BlockType> doubleSidedBlocks =
        [
            BlockType.Water,
        ];
        public static HashSet<BlockType> nonSolidBlocks =
        [
            BlockType.Air,
            BlockType.Water,
            BlockType.Sapling,
        ];
        public static HashSet<BlockType> renderAsSprite =
        [
            BlockType.Sapling,
        ];
        public static Faces[] allOuterFaces =
        [
            Faces.Front,
            Faces.Back,
            Faces.Left,
            Faces.Right,
            Faces.Top,
            Faces.Bottom
        ];
        public static Texture blocks;
        public static Texture font;
        public static Texture blank;
        public static Texture btnTexture;
        public static Dictionary<Faces, float> shadeSides = new()
        {
            [Faces.Front] = 0.85f,
            [Faces.Back] = 0.85f,
            [Faces.Left] = 0.75f,
            [Faces.Right] = 0.75f,
            [Faces.Top] = 1f,
            [Faces.Bottom] = 0.66f,
            [Faces.Inside] = 1f
        };
        public static BlockType[] fullInventory =
        [
            BlockType.Dirt,
            BlockType.Grass,
            BlockType.Stone,
            BlockType.Cobblestone,
            BlockType.Glass,
            BlockType.Sand,
            BlockType.Sapling,
            BlockType.Log,
            BlockType.Leaves,
            BlockType.Planks,
            BlockType.Bricks,
        ];

        public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            screenSize = new Vector2i(width, height);
            instance = this;
            CenterWindow(new Vector2i(width, height));
            skyColor = new(0.5f, 0.6f, 1f);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            screenSize = new Vector2i(e.Width, e.Height);
            gui?.RebuildGUI();
            pauseGUI?.RebuildGUI();
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            Title = "MINECRAP!!!";

            shaderProgram = new ShaderProgram("default.vert", "default.frag");
            blocks = new Texture("textures");
            font = new Texture("font");
            blank = new Texture("blank");
            btnTexture = new Texture("button");

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
            
            UIImage image = new
            (
                relSize: new Vector2(0.025f, 0.025f), offSize: Vector2.Zero,
                relPos: new Vector2(0.5f, 0.5f), offPos: Vector2.Zero,
                texture: new Texture("crosshair"),
                aspectRatio: 1f, dominantAxis: DomAxis.Height
            );
            
            UIImage inv = new
            (
                relSize: new Vector2(0.2f, 0.066f), offSize: Vector2.Zero,
                relPos: new Vector2(0.5f, 0.01f), offPos: Vector2.Zero,
                texture: new Texture("inventory"),
                aspectRatio: 9f, dominantAxis: DomAxis.Height,
                pivotPoint: new Vector2(0.5f, 0f)
            );

            invBlocks = new UIBlock[9]; 
            for (int i = 0; i < 9; i++)
            {
                UIBlock invBlock = new
                (
                    blockType: BlockType.Dirt,
                    relSize: new Vector2(7/83f, 7f/9f), offSize: Vector2.Zero,
                    relPos: new Vector2((i - 4) * 16 / 146f, 0f), offPos: Vector2.Zero,
                    aspectRatio: 1, dominantAxis: DomAxis.Height,
                    clickable: true
                );
                inv.AddChild(invBlock);
                invBlocks[i] = invBlock;
            }
            select = new
            (
                relSize: new Vector2(10/73f, 10/9f), offSize: Vector2.Zero,
                relPos: new Vector2(-4/9f, 0f),  offPos: Vector2.Zero,
                texture: new Texture("select"),
                aspectRatio: 1,
                dominantAxis: DomAxis.Height
            );
            inv.AddChild(select);

            fullInv = new UIImage
            (
                relSize: new Vector2(0.5f, 0.5f), offSize: Vector2.Zero,
                relPos: new Vector2(0.5f, 0.5f), offPos: Vector2.Zero,
                color: new Color(127, 127, 127, 127),
                aspectRatio: 9f/4f,
                dominantAxis: DomAxis.Height
            );

            UIBlockGrid invGrid = new
            (
                relSize: new Vector2(1, 1), offSize: Vector2.Zero,
                relPos: Vector2.Zero, offPos: Vector2.Zero,
                width: 9, height: 4,
                dominantAxis: DomAxis.Height
            );
            fullInv.AddChild(invGrid);
            invGrid.AddBlocks(fullInventory);
            fullInv.Disable();

            UIText verText = new
            (
                relCharSize: 0.03f, offCharSize: 0,
                relPos: new Vector2(0f, 1f), offPos: new Vector2(3, -3),
                text: "minecrap v0.0.7", txtAlignH: TextAlignmentH.Left, txtAlignV: TextAlignmentV.Top
            );
            
            gui.AddToGUI(image);
            gui.AddToGUI(inv);
            gui.AddToGUI(fullInv);
            gui.AddToGUI(verText);

            pauseGUI = new GUI();
            UIImage bg = new
            (
                relSize: new Vector2(1f, 1f), offSize: Vector2.Zero,
                relPos: new Vector2(0.5f, 0.5f), offPos: Vector2.Zero,
                color: new Color(0, 0, 0, 127)
            );
            UIText pauseText = new
            (
                relCharSize: 0.05f, offCharSize: 0f,
                relPos: new Vector2(0.5f, 0.75f), offPos: Vector2.Zero,
                text: "PAUSED", txtAlignH: TextAlignmentH.Center, txtAlignV: TextAlignmentV.Middle
            );
            UIButton resumeBtn = new
            (
                relSize: new Vector2(0.35f, 0.09f), offSize: Vector2.Zero,
                relPos: new Vector2(0.5f, 0.6f), offPos: Vector2.Zero,
                aspectRatio: 5f, dominantAxis: DomAxis.Height,
                texture: btnTexture, text: "resume",
                relTextSize: 0.4f
            );
            resumeBtn.OnClick = () => TogglePause();

            UIButton saveBtn = new
            (
                relSize: new Vector2(0.35f, 0.09f), offSize: Vector2.Zero,
                relPos: new Vector2(0.5f, 0.5f), offPos: Vector2.Zero,
                aspectRatio: 5f, dominantAxis: DomAxis.Height,
                texture: btnTexture, text: "save world",
                relTextSize: 0.4f
            );
            saveBtn.OnClick = () =>
            {
                DialogResult result = Dialog.FolderPicker(Path.Combine(Environment.CurrentDirectory, "worlds"));
                if (result.IsOk) World.instance.SaveWorld(result.Path);
                TogglePause();
            };

            UIButton loadBtn = new
            (
                relSize: new Vector2(0.35f, 0.09f), offSize: Vector2.Zero,
                relPos: new Vector2(0.5f, 0.4f), offPos: Vector2.Zero,
                aspectRatio: 5f, dominantAxis: DomAxis.Height,
                texture: btnTexture, text: "load world",
                relTextSize: 0.4f
            );
            loadBtn.OnClick = () =>
            {
                DialogResult result = Dialog.FolderPicker(Path.Combine(Environment.CurrentDirectory, "worlds"));
                if (result.IsOk)
                {
                    world.LoadWorld(result.Path);
                    Vector2i spawnPos = new(world.worldSize.X * 8, world.worldSize.Y * 8);
                    Vector3 playerPos = world.GetHighestBlock(spawnPos).pos + new Vector3(0, 1.5f, 0);
                    cam = new Camera(playerPos + new Vector3(0f, 0.5f, 0f));
                    player = new Player(playerPos);
                }
            };

            pauseGUI.AddToGUI(bg);
            pauseGUI.AddToGUI(pauseText);
            pauseGUI.AddToGUI(resumeBtn);
            pauseGUI.AddToGUI(saveBtn);
            pauseGUI.AddToGUI(loadBtn);

            pauseGUI.Disable();

            Vector2i spawnPos = new(worldSize.X * 8, worldSize.Y * 8);
            Vector3 playerPos = world.GetHighestBlock(spawnPos).pos + new Vector3(0, 1.5f, 0);
            cam = new Camera(playerPos + new Vector3(0f, 0.5f, 0f));
            player = new Player(playerPos);

            LockCursor();
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

            world.RenderChunks(world.GetChunksAroundPlayer(8));

            GL.Disable(EnableCap.DepthTest);
            view = Matrix4.Identity;
            projection = Matrix4.CreateOrthographicOffCenter(0, 1, 0, 1, -1, 1);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            gui.Render(shaderProgram);
            pauseGUI.Render(shaderProgram);

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

            if (inInventory && mouse.IsButtonPressed(MouseButton.Left)) gui.Click(new Vector2(mouse.Position.X, screenSize.Y - mouse.Position.Y));
            if (paused && mouse.IsButtonPressed(MouseButton.Left)) pauseGUI.Click(new Vector2(mouse.Position.X, screenSize.Y - mouse.Position.Y));
        }

        public void UpdateInvBlockType(int index, BlockType blockType) => invBlocks[index].SetBlockType(blockType);
        
        public void UpdateSelectPlacement(int num)
        {
            select.relPos = new Vector2((num - 4) * 16 / 146f, 0f);
            select.GenElement();
        }

        public void LockCursor() => CursorState = CursorState.Grabbed;
        public void UnlockCursor() => CursorState = CursorState.Normal;
        public bool IsCursorLocked() => CursorState == CursorState.Grabbed;

        public void ToggleFullInv()
        {
            if (inInventory)
            {
                LockCursor();
                fullInv.Disable();
            }
            else
            {
                UnlockCursor();
                fullInv.Enable();
            }
            inInventory = !inInventory;
        }
        
        public void TogglePause()
        {
            if (paused)
            {
                LockCursor();
                pauseGUI.Disable();
            }
            else
            {
                UnlockCursor();
                pauseGUI.Enable();
            }
            paused = !paused;
        }
    }
}