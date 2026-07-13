using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace minecrap
{
    internal class Camera
    {
        private float sens = 0.25f;
        public Vector3 pos;
        public Vector3 right = Vector3.UnitX;
        public Vector3 up = Vector3.UnitY;
        public Vector3 front = -Vector3.UnitZ;
        private float pitch;
        private float yaw = -90;
        private bool firstMove = true;
        public Vector2 lastPos;
        public static Camera instance;

        public Camera(Vector3 pos)
        {
            this.pos = pos;
            instance = this;
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(pos, pos + front, up);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60), (float)Game.instance.width / Game.instance.height, 0.1f, 128);
        }

        private void UpdateVectors()
        {
            if (pitch > 89.99f) pitch = 89.99f;
            else if (pitch < -89.99f) pitch = -89.99f;
            front.X = MathF.Cos(MathHelper.DegreesToRadians(pitch)) * MathF.Cos(MathHelper.DegreesToRadians(yaw));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(pitch));
            front.Z = MathF.Cos(MathHelper.DegreesToRadians(pitch)) * MathF.Sin(MathHelper.DegreesToRadians(yaw));
            front = Vector3.Normalize(front);
            right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
            up = Vector3.Normalize(Vector3.Cross(right, front));
        }

        private void InputController(MouseState mouse, FrameEventArgs e)
        {
            if (firstMove)
            {
                lastPos = new Vector2(mouse.X, mouse.Y);
                firstMove = false;
            }
            else
            {
                Vector2 delta = new(mouse.X - lastPos.X, mouse.Y - lastPos.Y);
                lastPos = new Vector2(mouse.X, mouse.Y);
                yaw += delta.X * sens;
                pitch -= delta.Y * sens;
            }
            UpdateVectors();
        }

        public void Update(MouseState mouse, FrameEventArgs e)
        {
            InputController(mouse, e);
        }
    }
}