using OpenTK.Graphics.OpenGL4;

namespace minecrap.graphics
{
    internal class VAO
    {
        public int ID;

        public VAO()
        {
            ID = GL.GenVertexArray();
            Bind();
        }

        public void LinkToVAO(int location, int size, VBO VBO, VertexAttribPointerType type = VertexAttribPointerType.Float, bool normalized = false)
        {
            Bind();
            VBO.Bind();
            GL.VertexAttribPointer(location, size, type, normalized, 0, 0);
            GL.EnableVertexAttribArray(location);
            Unbind();
        }

        public void Bind() => GL.BindVertexArray(ID);
        public void Unbind() => GL.BindVertexArray(0);
        public void Delete() => GL.DeleteVertexArray(ID);
    }
}