using minecrap.graphics;
using minecrap.world;
using OpenTK.Mathematics;

namespace minecrap.gui
{
    internal class UIBlockGrid : UIElement
    {
        private int width;
        private int height;

        public UIBlockGrid(Vector2 relSize, Vector2 offSize, Vector2 relPos, Vector2 offPos, int width, int height, DomAxis dominantAxis = DomAxis.None, Vector2? pivotPoint = null, Color? color = null)
        {
            this.relSize = relSize;
            this.offSize = offSize;
            this.relPos = relPos;
            this.offPos = offPos;
            this.dominantAxis = dominantAxis;
            this.pivotPoint = pivotPoint ?? new Vector2(0.5f, 0.5f);
            this.color = color ?? new Color(255, 255, 255);
            this.width = width;
            this.height = height;
            aspectRatio = (float)width / height;
        }

        public void AddBlocks(BlockType[] blockTypes)
        {
            if (children != null)
            {
                foreach (UIElement block in children) block.Delete();
                children.Clear();
            }
            Vector2 blockSize = new Vector2(1 / (width + 0.5f), 1 / (height + 0.5f));

            for (int i = 0; i < Math.Min(blockTypes.Length, width * height); i++)
            {
                int x = i % width;
                int y = i / width;
                BlockType type = blockTypes[i];
                UIBlock block = new
                (
                    blockType: type,
                    relSize: blockSize, offSize: Vector2.Zero,
                    relPos: new Vector2((x - width / 2f + 0.5f) / width, (height / 2f - y - 0.5f) / height), offPos: Vector2.Zero,
                    clickable: true,
                    aspectRatio: 1f,
                    dominantAxis: DomAxis.Height
                );
                block.OnClick = () => Player.instance.GrabItem(type);
                AddChild(block);
            }
        }
    }
}