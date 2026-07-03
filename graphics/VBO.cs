using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace minecrap.graphics
{
    internal class VBO
    {
        public int ID;

        public VBO(List<Vector3> data)
        {
            Span<Vector3> dataSpan = CollectionsMarshal.AsSpan(data);
            ID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Count * Vector3.SizeInBytes, ref dataSpan[0], BufferUsageHint.StaticDraw);
        }

        public VBO(List<Vector2> data)
        {
            Span<Vector2> dataSpan = CollectionsMarshal.AsSpan(data);
            ID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Count * Vector2.SizeInBytes, ref dataSpan[0], BufferUsageHint.StaticDraw);
        }

        public void Bind() => GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
        public void Unbind() => GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        public void Delete() => GL.DeleteBuffer(ID);
    }
}