using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace minecrap.graphics
{
    internal class Texture
    {
        public int ID;

        public Texture(string name)
        {
            ID = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, ID);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);

            StbImage.stbi_set_flip_vertically_on_load(1);
            ImageResult img = ImageResult.FromStream(File.OpenRead($"textures/{name}.png"), ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, img.Width, img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, img.Data);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            Unbind();
        }

        public void Bind() => GL.BindTexture(TextureTarget.Texture2D, ID);
        public void Unbind() => GL.BindTexture(TextureTarget.Texture2D, 0);
        public void Delete() => GL.DeleteTexture(ID);
    }
}