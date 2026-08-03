using OpenTK.Mathematics;

namespace minecrap.world
{
    internal static class RayCast
    {
        public static Block? RayCastedBlock(Vector3 origin, Vector3 direction, float maxDist = 1f, float minDist = 0f, bool build = false)
        {
            Vector3 temp = origin;
            Vector3 normalizedDir = Vector3.Normalize(direction);
            Vector3i sign = new(Math.Sign(normalizedDir.X), Math.Sign(normalizedDir.Y), Math.Sign(normalizedDir.Z));
            temp += normalizedDir * minDist;
            Vector3i blockPos = new((int)Math.Round(temp.X), (int)Math.Round(temp.Y), (int)Math.Round(temp.Z));
            Block? buildBlock = null;
            
            while ((temp - origin).Length < maxDist)
            {
                Vector3 distToNext = new(sign.X == 1 ? MathF.Floor(temp.X + 0.5f) + 0.5f : MathF.Ceiling(temp.X - 0.5f) - 0.5f,
                                    sign.Y == 1 ? MathF.Floor(temp.Y + 0.5f) + 0.5f : MathF.Ceiling(temp.Y - 0.5f) - 0.5f,
                                    sign.Z == 1 ? MathF.Floor(temp.Z + 0.5f) + 0.5f : MathF.Ceiling(temp.Z - 0.5f) - 0.5f);
                distToNext -= temp;
                distToNext /= normalizedDir;
                Vector3i nextBlock = blockPos;
                Vector3 nextTemp;
                if (distToNext.X <= distToNext.Y && distToNext.X <= distToNext.Z)
                {
                    nextBlock.X += sign.X;
                    nextTemp = temp + normalizedDir * distToNext.X;
                }
                else if (distToNext.Y <= distToNext.X && distToNext.Y <= distToNext.Z)
                {
                    nextBlock.Y += sign.Y;
                    nextTemp = temp + normalizedDir * distToNext.Y;
                }
                else
                {
                    nextBlock.Z += sign.Z;
                    nextTemp = temp + normalizedDir * distToNext.Z;
                }

                Block? block = World.instance.GetBlock(blockPos);
                if (block != null)
                {
                    Vector2 intersections = block.GetIntCollider().RayIntersections(temp, normalizedDir);
                    if (intersections.Y >= 0 && intersections.X <= intersections.Y)
                    {
                        if (build) return buildBlock;
                        else return block;
                    }
                    else if (build) buildBlock = block;
                }
                temp = nextTemp;
                blockPos = nextBlock;
            }
            Vector3 lastPosOfRay = origin + normalizedDir * maxDist;
            Vector3i lastBlockPos = new((int)Math.Round(lastPosOfRay.X), (int)Math.Round(lastPosOfRay.Y), (int)Math.Round(lastPosOfRay.Z));
            Block? lastBlock = World.instance.GetBlock(lastBlockPos);

            if (lastBlock != null && lastBlock.GetIntCollider().IsPointInside(lastPosOfRay))
            {
                if (build) return buildBlock;
                else return lastBlock;
            }
            return null;
        }
    }
}