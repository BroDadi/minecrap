using minecrap.world;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace minecrap
{
    internal class Player
    {
        private float speed = 4f;
        private float speedY = 0f;
        private float gravity = 20f;
        private float jumpForce = 7f;
        private bool onGround;
        public Vector3 pos;
        public Collider collider;
        public static Player instance;

        public Player(Vector3 pos)
        {
            this.pos = pos;
            collider = new Collider(pos, new Vector3(0.6f, 1.8f, 0.6f));
            instance = this;
        }

        public void Update(KeyboardState input, MouseState mouse, FrameEventArgs e)
        {
            InputController(input, mouse, e);
        }

        private void InputController(KeyboardState input, MouseState mouse, FrameEventArgs e)
        {
            float deltaTime = (float)e.Time;
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
            speedY -= gravity * deltaTime;
            
            if (move != Vector3.Zero) move = Vector3.Normalize(move) * speed;
            move *= deltaTime;

            if (!CheckCollision(pos + new Vector3(move.X, 0, 0))) pos += new Vector3(move.X, 0, 0);
            if (!CheckCollision(pos + new Vector3(0, 0, move.Z))) pos += new Vector3(0, 0, move.Z);
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

            if ((input.IsKeyDown(Keys.Space) || input.IsKeyDown(Keys.KeyPad0)) && onGround)
            {
                speedY = jumpForce;
                onGround = false;
            }

            Camera.instance.pos = pos + new Vector3(0, 0.5f, 0);
            collider.SetPosition(pos);
        }

        private bool CheckCollision(Vector3 position)
        {
            Collider coll = new(position, collider.size);
            foreach (Block block in World.instance.GetBlocksAroundCollider(coll))
            {
                if (coll.Intersects(block.collider))
                {
                    return true;
                }
            }
            return false;
        }
    }
}