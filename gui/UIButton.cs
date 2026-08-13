using minecrap.graphics;
using OpenTK.Mathematics;

namespace minecrap.gui
{
    internal class UIButton : UIImage
    {
        private UIText txt;
        private float relTextSize;
        private float offTextSize;
        private Color textColor;

        public UIButton(Vector2 relSize, Vector2 offSize, Vector2 relPos, Vector2 offPos, Texture? texture = null, Color? color = null, float aspectRatio = 0, DomAxis dominantAxis = DomAxis.Height, Vector2? pivotPoint = null, Color? textColor = null, string text = "", float relTextSize = 0f, float offTextSize = 0f)
                : base(relSize, offSize, relPos, offPos, texture, aspectRatio, dominantAxis, pivotPoint, color, true)
        {
            this.relTextSize = relTextSize;
            this.offTextSize = offTextSize;
            this.textColor = textColor ?? new Color(255, 255, 255);
            if (text != "")
            {
                txt = new(relTextSize, offTextSize, new Vector2(0.5f, 0.5f), Vector2.Zero, text, TextAlignmentH.Center, TextAlignmentV.Middle, textColor);
                AddChild(txt);
            }
        }

        public void SetText(string str)
        {
            if (str == "") txt.Delete();
            else if (txt == null) txt = new(relTextSize, offTextSize, new Vector2(0.5f, 0.5f), Vector2.Zero, str, TextAlignmentH.Center, TextAlignmentV.Middle, textColor);
            else txt.SetText(str);
        }
    }
}