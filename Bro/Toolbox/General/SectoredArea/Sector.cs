

using Bro.Sketch;
using UnityEngine;

namespace Bro.Toolbox.SectoredArea
{
    public class Sector
    {
        public int SectorId;

        public short IndexX;
        public short IndexY;

        public float MinX;
        public float MaxX;
        public float MinY;
        public float MaxY;
        
        public void Init(int id, short indexX, short indexY, float minX, float maxX, float minY, float maxY)
        {
            SectorId = id;

            IndexX = indexX;
            IndexY = indexY;

            MinX = minX;
            MaxX = maxX;
            MinY = minY;
            MaxY = maxY;
        }

        public Vector2 Center => new Vector2((MaxX + MinX) * 0.5f, (MaxY + MinY) * 0.5f);

        public bool IsObjectInside(Vector2 pos)
        {
            return pos.x > MinX && pos.x <= MaxX && pos.y > MinY && pos.y <= MaxY;
        }
    }
}