using System.Text;
using minecrap.graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace minecrap.gui
{
    internal class UIText : UIElement
    {
        private string text;
        private TextAlignmentH txtAlignH;
        private TextAlignmentV txtAlignV;

        public UIText(float relCharSize, float offCharSize, Vector2 relPos, Vector2 offPos, string text, TextAlignmentH txtAlignH, TextAlignmentV txtAlignV, Color? color = null)
        {
            this.relPos = relPos;
            this.offPos = offPos;
            this.color = color ?? new Color(255, 255, 255);
            this.text = text;
            this.txtAlignH = txtAlignH;
            this.txtAlignV = txtAlignV;

            relSize = new Vector2(relCharSize, relCharSize);
            offSize = new Vector2(offCharSize, offCharSize);
            aspectRatio = 1;
            dominantAxis = DomAxis.Height;
        }

        public override void GenElement()
        {
            List<Vector3> vertexes = new();
            List<Vector2> texCoords = new();
            List<uint> indexes = new();
            Vector2 centerPos = CalculatePos() / Game.instance.screenSize;
            Vector2 charSize = CalculateSize() / Game.instance.screenSize;

            List<string> lines = new();
            StringBuilder sb = new();

            foreach (char c in text)
            {
                if (c == '\n')
                {
                    lines.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    if (FontData.coordsByChar.ContainsKey(c)) sb.Append(c);
                    else sb.Append(' ');
                }
            }
            if (sb.Length != 0) lines.Add(sb.ToString());

            float startY = 0;
            switch (txtAlignV)
            {
                case TextAlignmentV.Top:
                    startY = centerPos.Y;
                    break;
                case TextAlignmentV.Middle:
                    startY = centerPos.Y + lines.Count / 2f * charSize.Y;
                    break;
                case TextAlignmentV.Bottom:
                    startY = centerPos.Y + lines.Count * charSize.Y;
                    break;
            }
            startY -= charSize.Y / 2f;

            uint vertexCount = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                float startX = 0;
                switch (txtAlignH)
                {
                    case TextAlignmentH.Left:
                        startX = centerPos.X;
                        break;
                    case TextAlignmentH.Center:
                        startX = centerPos.X - lines[i].Length / 2f * charSize.X;
                        break;
                    case TextAlignmentH.Right:
                        startX = centerPos.X - lines[i].Length * charSize.X;
                        break;
                }
                startX -= charSize.X / 2f;

                for (int j = 0; j < lines[i].Length; j++)
                {
                    Vector2 letterPos = new(startX + j * charSize.X, startY - i * charSize.Y);
                    vertexes.Add(new Vector3(letterPos.X, letterPos.Y, 0));
                    vertexes.Add(new Vector3(letterPos.X + charSize.X, letterPos.Y, 0));
                    vertexes.Add(new Vector3(letterPos.X + charSize.X, letterPos.Y - charSize.Y, 0));
                    vertexes.Add(new Vector3(letterPos.X, letterPos.Y - charSize.Y, 0));
                    texCoords.AddRange(FontData.coordsByChar[lines[i][j]]);
                    indexes.Add(0 + vertexCount);
                    indexes.Add(1 + vertexCount);
                    indexes.Add(2 + vertexCount);
                    indexes.Add(2 + vertexCount);
                    indexes.Add(3 + vertexCount);
                    indexes.Add(0 + vertexCount);
                    vertexCount += 4;
                }
            }

            Color[] colors = new Color[vertexCount];
            Array.Fill(colors, color);

            vao = new VAO();
            vao.Bind();

            vbo = new VBO(vertexes);
            vbo.Bind();
            vao.LinkToVAO(0, 3, vbo);

            textureVBO = new VBO(texCoords);
            textureVBO.Bind();
            vao.LinkToVAO(1, 2, textureVBO);

            colorVBO = new VBO(colors);
            colorVBO.Bind();
            vao.LinkToVAO(2, 4, colorVBO, VertexAttribPointerType.UnsignedByte, true);

            ebo = new EBO(indexes);
            ebo.Bind();

            base.GenElement();
        }

        protected override void OnRender(ShaderProgram shaderProgram)
        {
            shaderProgram.Bind();
            Game.font.Bind();
            vao.Bind();
            ebo.Bind();
            GL.DrawElements(PrimitiveType.Triangles, text.Length * 6, DrawElementsType.UnsignedInt, 0);
            base.OnRender(shaderProgram);
        }

        public void SetText(string text) => this.text = text;
    }
}