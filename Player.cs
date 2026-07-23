using minecrap.gui;
using minecrap.world;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace minecrap
{
    internal class Player
    {
        private const float speed = 4f;
        private const float gravity = 20f;
        private const float jumpForce = 7f;
        private const float reach = 5f;
        private const float waterSpeed = 2f;
        private const float waterGravity = 5f;
        private const float waterDrag = 0.2f;
        private const float swimUpSpeed = 10f;
        private float speedY = 0f;
        private bool onGround, inWater;
        private int selected;
        private BlockType[] hotbar;
        public Vector3 pos;
        public Collider collider;
        public static Player instance;

        public Player(Vector3 pos)
        {
            this.pos = pos;
            collider = new Collider(pos, new Vector3(0.6f, 1.8f, 0.6f));
            instance = this;
            hotbar = new BlockType[]
            {
                BlockType.Dirt,
                BlockType.Grass,
                BlockType.Stone,
                BlockType.Cobblestone,
                BlockType.Glass,
                BlockType.Sand,
                BlockType.Sapling,
                BlockType.Log,
                BlockType.Leaves
            };
            selected = 0;

            for (int i = 0; i < hotbar.Length; i++)
            {
                Game.instance.UpdateInvBlockType(i, hotbar[i]);
            }
        }

        public void Update(KeyboardState input, MouseState mouse, FrameEventArgs e)
        {
            InputController(input, mouse, e);
        }

        private void InputController(KeyboardState input, MouseState mouse, FrameEventArgs e)
        {
            float deltaTime = Math.Min((float)e.Time, 0.5f);

            if (mouse.IsButtonPressed(MouseButton.Left))
            {
                Block? block = RayCast.RayCastedBlock(Camera.instance.pos, Camera.instance.front, reach);
                if (block != null) World.instance.SetBlock(block.pos, BlockType.Air);
            }

            if (mouse.IsButtonPressed(MouseButton.Right))
            {
                Block? block = RayCast.PlaceOnBlock(Camera.instance.pos, Camera.instance.front, reach);
                if (block != null && !collider.Intersects(block.GetCollider())) World.instance.SetBlock(block.pos, hotbar[selected]);
            }

            if (mouse.ScrollDelta.Y == -1) SelectBlock((selected + 1) % hotbar.Length);
            if (mouse.ScrollDelta.Y == 1) SelectBlock((selected - 1 + hotbar.Length) % hotbar.Length);

            if (input.IsKeyPressed(Keys.R))
            {
                Random rand = new();
                Vector2i randomPos = new(rand.Next(0, World.instance.worldSize.X * World.chunkSize), rand.Next(0, World.instance.worldSize.Y * World.chunkSize));
                pos = World.instance.GetHighestBlock(randomPos).pos + new Vector3(0, 1.5f, 0);
                speedY = 0;
            }

            if (input.IsKeyPressed(Keys.Escape))
            {
                Game.instance.CursorState = Game.instance.CursorState == CursorState.Grabbed ? CursorState.Normal : CursorState.Grabbed;
            }
            inWater = CheckWater(pos);
            if (inWater) speedY *= MathF.Pow(waterDrag, deltaTime);

            if (input.IsKeyPressed(Keys.D1) || input.IsKeyPressed(Keys.KeyPad1)) SelectBlock(0);
            if (input.IsKeyPressed(Keys.D2) || input.IsKeyPressed(Keys.KeyPad2)) SelectBlock(1);
            if (input.IsKeyPressed(Keys.D3) || input.IsKeyPressed(Keys.KeyPad3)) SelectBlock(2);
            if (input.IsKeyPressed(Keys.D4) || input.IsKeyPressed(Keys.KeyPad4)) SelectBlock(3);
            if (input.IsKeyPressed(Keys.D5) || input.IsKeyPressed(Keys.KeyPad5)) SelectBlock(4);
            if (input.IsKeyPressed(Keys.D6) || input.IsKeyPressed(Keys.KeyPad6)) SelectBlock(5);
            if (input.IsKeyPressed(Keys.D7) || input.IsKeyPressed(Keys.KeyPad7)) SelectBlock(6);
            if (input.IsKeyPressed(Keys.D8) || input.IsKeyPressed(Keys.KeyPad8)) SelectBlock(7);
            if (input.IsKeyPressed(Keys.D9) || input.IsKeyPressed(Keys.KeyPad9)) SelectBlock(8);

            Vector3 move = Vector3.Zero;
            Vector3 front = Camera.instance.front;
            front.Y = 0;
            front.Normalize();

            Vector3 right = Camera.instance.right;
            right.Y = 0;
            right.Normalize();

            if (input.IsKeyDown(Keys.Up) || input.IsKeyDown(Keys.W)) move += front;
            if (input.IsKeyDown(Keys.Left) || input.IsKeyDown(Keys.A)) move -= right;
            if (input.IsKeyDown(Keys.Down) || input.IsKeyDown(Keys.S)) move -= front;
            if (input.IsKeyDown(Keys.Right) || input.IsKeyDown(Keys.D)) move += right;
            if (move != Vector3.Zero)
            {
                move = Vector3.Normalize(move) * deltaTime * (inWater ? waterSpeed : speed);
                if (!CheckCollision(pos + new Vector3(move.X, 0, 0))) pos += new Vector3(move.X, 0, 0);
                if (!CheckCollision(pos + new Vector3(0, 0, move.Z))) pos += new Vector3(0, 0, move.Z);
            }
            speedY -= (inWater ? waterGravity : gravity) * deltaTime;

            if (!CheckCollision(pos + new Vector3(0, speedY * deltaTime, 0)))
            {
                pos += new Vector3(0, speedY * deltaTime, 0);
                onGround = false;
            }
            else
            {
                onGround = speedY < 0;
                speedY = 0;
            }
            if ((input.IsKeyDown(Keys.Space) || input.IsKeyDown(Keys.KeyPad0)) && (onGround || inWater))
            {
                speedY = inWater ? Math.Min(speedY + swimUpSpeed * deltaTime, 3) : jumpForce;
                onGround = false;
            }

            Camera.instance.pos = pos + new Vector3(0, 0.5f, 0);
            collider.SetPosition(pos);
        }

        private bool CheckCollision(Vector3 position)
        {
            if (position.X < -0.5f || position.Z < -0.5f || position.X > World.instance.worldSize.X * World.chunkSize - 0.5f || position.Z > World.instance.worldSize.Y * World.chunkSize - 0.5f) return true;
            Collider coll = new(position, collider.size);
            foreach (Block block in World.instance.GetSolidBlocksAroundCollider(coll))
            {
                if (coll.Intersects(block.GetCollider()))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckWater(Vector3 position)
        {
            Collider coll = new(position, collider.size);
            foreach (Block block in World.instance.GetWaterAroundCollider(coll))
            {
                if (coll.Intersects(block.GetCollider()))
                {
                    return true;
                }
            }
            return false;
        }
        private void SelectBlock(int num)
        {
            selected = num;
            Game.instance.UpdateSelectPlacement(num);
        }
    }
}