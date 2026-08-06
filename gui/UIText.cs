using minecrap.graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace minecrap.gui
{
    internal class UIText : UIElement
    {
        private string text;

        public UIText(float relCharSize, float offCharSize, Vector2 relPos, Vector2 offPos, string text, Color? color = null)
        {
            this.relPos = relPos;
            this.offPos = offPos;
            this.color = color ?? new Color(255, 255, 255, 255);
            this.text = text;
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
            Vector2 startPos = CalculatePos() / Game.instance.screenSize;
            Vector2 charSize = CalculateSize() / Game.instance.screenSize;
            startPos -= charSize / 2f;
            Vector2 currPos = startPos;

            uint vertexCount = 0;
            foreach (char c in text)
            {
                if (c == '\n')
                {
                    currPos.Y -= charSize.Y;
                    currPos.X = startPos.X;
                }
                else
                {
                    if (FontData.coordsByChar.ContainsKey(c))
                    {
                        vertexes.Add(new Vector3(currPos.X, currPos.Y, 0));
                        vertexes.Add(new Vector3(currPos.X + charSize.X, currPos.Y, 0));
                        vertexes.Add(new Vector3(currPos.X + charSize.X, currPos.Y - charSize.Y, 0));
                        vertexes.Add(new Vector3(currPos.X, currPos.Y - charSize.Y, 0));
                        texCoords.AddRange(FontData.coordsByChar[c]);
                        indexes.Add(0 + vertexCount);
                        indexes.Add(1 + vertexCount);
                        indexes.Add(2 + vertexCount);
                        indexes.Add(2 + vertexCount);
                        indexes.Add(3 + vertexCount);
                        indexes.Add(0 + vertexCount);
                        vertexCount += 4;
                        currPos.X += charSize.X;
                    }
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