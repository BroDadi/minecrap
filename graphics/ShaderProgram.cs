using OpenTK.Graphics.OpenGL4;

namespace minecrap.graphics
{
    internal class ShaderProgram
    {
        public int ID;

        public ShaderProgram(string vertexPath, string fragmentPath)
        {
            ID = GL.CreateProgram();

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, LoadShaderSource(vertexPath));
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, LoadShaderSource(fragmentPath));
            GL.CompileShader(fragmentShader);

            GL.AttachShader(ID, vertexShader);
            GL.AttachShader(ID, fragmentShader);
            GL.LinkProgram(ID);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public static string LoadShaderSource(string filePath)
        {
            string shaderSource = "";
            try
            {
                using (StreamReader reader = new StreamReader("shaders/" + filePath))
                {
                    shaderSource = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return shaderSource;
        }

        public void Bind() => GL.UseProgram(ID);
        public void Unbind() => GL.UseProgram(0);
        public void Delete() => GL.DeleteShader(ID);
    }
}