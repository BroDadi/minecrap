using OpenTK.Mathematics;

namespace minecrap.world
{
    internal static class RayCast
    {
        public static Block? RayCastedBlock(Vector3 origin, Vector3 direction, float maxDist = 1f, float minDist = 0f, float step = 0.01f)
        {
            Vector3 temp = origin;
            Vector3 normalizedDir = Vector3.Normalize(direction);
            temp += normalizedDir * minDist;
            for (float i = minDist; i <= maxDist; i += step)
            {
                temp += normalizedDir * step;
                Block? block = World.instance.GetBlock(new Vector3i((int)Math.Round(temp.X), (int)Math.Round(temp.Y), (int)Math.Round(temp.Z)));
                if (block != null && block.blockType != BlockType.Air) return block;
            }
            return null;
        }

        public static Block? PlaceOnBlock(Vector3 origin, Vector3 direction, float maxDist = 1f, float minDist = 0f, float step = 0.01f)
        {
            Vector3 temp = origin;
            Vector3 normalizedDir = Vector3.Normalize(direction);
            temp += normalizedDir * minDist;

            Block? lastBlock = null;
            for (float i = minDist; i <= maxDist; i += step)
            {
                temp += normalizedDir * step;
                Block? block = World.instance.GetBlock(new Vector3i((int)Math.Round(temp.X), (int)Math.Round(temp.Y), (int)Math.Round(temp.Z)));
                if (block != null)
                {
                    if (block.blockType != BlockType.Air) return lastBlock;
                    else lastBlock = block;
                }
            }
            return null;
        }
    }
}