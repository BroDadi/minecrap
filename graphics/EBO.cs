using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace minecrap.graphics
{
    internal class EBO
    {
        public int ID;
        
        public EBO(List<uint> data)
        {
            Span<uint> dataSpan = CollectionsMarshal.AsSpan(data);
            ID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, data.Count * sizeof(uint), ref dataSpan[0], BufferUsageHint.StaticDraw);
        }

        public void Bind() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);
        public void Unbind() => GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        public void Delete() => GL.DeleteBuffer(ID);
    }
}