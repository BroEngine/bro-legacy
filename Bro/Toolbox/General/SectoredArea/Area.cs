using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bro.Toolbox.SectoredArea
{
    public class Area<T> where T : Sector, new()
    {
        private short _sectorsAmountX;
        private short _sectorsAmountY;
        private int _sectorsTotalAmount;
        private T[] _sectors;

        public Vector2 AreaSize { get; private set; }
        public Vector2 SectorSize { get; private set; }
        public Vector2 AreaMin { get; private set; }
        public Vector2 AreaMax { get; private set; }

        public short MinIndexX { get; private set; }
        public short MinIndexY { get; private set; }
        public short MaxIndexX { get; private set; }
        public short MaxIndexY { get; private set; }

        public T[] Sectors {get{return _sectors;}} 

        public Area()
        {
        }

        public void Init(Rect areaRect, Vector2 sectorSize)
        {
            AreaMin = areaRect.min;
            AreaMax = areaRect.max;
            AreaSize = areaRect.size;
            SectorSize = sectorSize;

            MaxIndexX = (short) (Math.Ceiling(AreaSize.x / SectorSize.x) - 1);
            MaxIndexY = (short) (Math.Ceiling(AreaSize.y / SectorSize.y) - 1);

            _sectorsAmountX = (short) (MaxIndexX + 1);
            _sectorsAmountY = (short) (MaxIndexY + 1);

            _sectorsTotalAmount = (int) (_sectorsAmountX * _sectorsAmountY);

            _sectors = new T[_sectorsTotalAmount];

            for (short x = 0; x < _sectorsAmountX; ++x)
            {
                for (short y = 0; y < _sectorsAmountY; ++y)
                {
                    float minX = AreaMin.x + SectorSize.x * x;
                    float maxX = minX + SectorSize.x;
                    float minY = AreaMin.y + SectorSize.y * y;
                    float maxY = minY + SectorSize.y;
                    int arrayIndex = GetArrayIndex(x, y);
                    var sector = new T();
                    sector.Init(arrayIndex, x, y, minX, maxX, minY, maxY);
                    _sectors[arrayIndex] = sector;
                }
            }
        }

        protected int GetArrayIndex(short sectorIndexX, short sectorIndexY)
        {
            return (sectorIndexX * _sectorsAmountY + sectorIndexY);
        }

        public int GetSectorId(short sectorIndexX, short sectorIndexY)
        {
            return GetArrayIndex(sectorIndexX, sectorIndexY);
        }

        public int GetSectorId(Vector2 position)
        {
            short indexX, indexY;
            CalculateSectorIndexes(position, out indexX, out indexY);
            return GetSectorId(indexX, indexY);
        }

        public T GetSector(short indexX, short indexY)
        {
            var index = GetArrayIndex(indexX, indexY);
            if (index >= 0 && index < _sectors.Length)
            {
                return _sectors[index];
            }
            return null;
        }

        public T GetSector(int sectorId)
        {
            return _sectors[sectorId];
        }

        public T GetSector(Vector2 pos)
        {
            short indexX, indexY;
            CalculateSectorIndexes(pos, out indexX, out indexY);
            return GetSector(indexX, indexY);
        }

        public void CalculateSectorIndexes(Vector2 pos, out short indexX, out short indexY)
        {
            indexX = (short) (Math.Ceiling((pos.x - AreaMin.x) / SectorSize.x) - 1f);
            indexY = (short) (Math.Ceiling((pos.y - AreaMin.y) / SectorSize.y) - 1f);
            if (indexX < MinIndexX)
            {
                indexX = MinIndexX;
            }
            else if (indexX > MaxIndexX)
            {
                indexX = MaxIndexX;
            }

            if (indexY < MinIndexY)
            {
                indexY = MinIndexY;
            }
            else if (indexY > MaxIndexY)
            {
                indexY = MaxIndexY;
            }
        }

        public bool IsInside(Vector2 point)
        {
            return AreaMin.x < point.x && AreaMin.y < point.y && AreaMax.x > point.x && AreaMax.y > point.y;
        }


        public delegate bool SectorHandler(T sector);

        /// <summary>
        /// call sectorHandler for every sector which intersected by line from startPoint to endPoint
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="sectorHandler">if true stop, false => continue</param>
        public void ForEachSector(Vector2 startPoint, Vector2 endPoint, SectorHandler sectorHandler)
        {
            short startIndexX, startIndexY, endIndexX, endIndexY;
            CalculateSectorIndexes(startPoint, out startIndexX, out startIndexY);
            CalculateSectorIndexes(endPoint, out endIndexX, out endIndexY);

            var startSector = GetSector(startPoint);
            Sector prevHandledSector = null;
            if (IsInside(startPoint))
            {
                if (sectorHandler(startSector))
                {
                    return;
                }
                prevHandledSector = startSector;
            }

            if (startIndexX == endIndexX && startIndexY == endIndexY)
            {
                return;
            }

            int deltaIndexX = System.Math.Abs(endIndexX - startIndexX);
            int deltaIndexY = System.Math.Abs(endIndexY - startIndexY);

//            DebugRaycastTool.Instance.CollisionPoints.Clear();
            Vector2 currentPosition;
            T currentSector;

            if (deltaIndexX > deltaIndexY)
            {
                // y = ax + b
                float invDeltaX = 1f / (endPoint.x - startPoint.x);
                float a = (endPoint.y - startPoint.y) * invDeltaX;
                float b = (startPoint.y * endPoint.x - startPoint.x * endPoint.y) * invDeltaX;
                float stepX = SectorSize.x * System.Math.Sign(endIndexX - startIndexX);
                float miniStepX = stepX * 0.2f;
                float x = stepX > 0f ? startSector.MaxX : startSector.MinX;
                for (int i = 0; i < deltaIndexX; ++i)
                {
                    var y = a * x + b;
                    currentPosition = new Vector2(x - miniStepX, y);
                    if (IsInside(currentPosition))
                    {
                        currentSector = GetSector(currentPosition);
                        if (currentSector != prevHandledSector)
                        {
                            if (sectorHandler(currentSector))
                            {
                                return;
                            }
                            prevHandledSector = currentSector;
                        }
                    }

//                    DebugRaycastTool.Instance.CollisionPoints.Add(new Vector2(x, y));
                    currentPosition = new Vector2(x + miniStepX, y);
                    if (IsInside(currentPosition))
                    {
                        currentSector = GetSector(currentPosition);
                        if (currentSector != prevHandledSector)
                        {
                            if (sectorHandler(currentSector))
                            {
                                return;
                            }
                            prevHandledSector = currentSector;
                        }
                    }
                    x += stepX;
                }
            }
            else //(deltaIndexY > 0)
            {
                // x = yx +b 
                float invDeltaY = 1f / (endPoint.y - startPoint.y);
                float a = (endPoint.x - startPoint.x) * invDeltaY;
                float b = (startPoint.x * endPoint.y - startPoint.y * endPoint.x) * invDeltaY;
                float stepY = SectorSize.y * System.Math.Sign(endIndexY - startIndexY);
                float miniStepY = stepY * 0.2f;
                float y = stepY > 0f ? startSector.MaxY : startSector.MinY;
                for (int i = 0; i < deltaIndexY; ++i)
                {
                    var x = a * y + b;

                    currentPosition = new Vector2(x, y - miniStepY);
                    if (IsInside(currentPosition))
                    {
                        currentSector = GetSector(currentPosition);
                        if (currentSector != prevHandledSector)
                        {
                            if (sectorHandler(currentSector))
                            {
                                return;
                            }
                            prevHandledSector = currentSector;
                        }
                    }

//                    DebugRaycastTool.Instance.CollisionPoints.Add(new Vector2(x, y));

                    currentPosition = new Vector2(x, y + miniStepY);
                    if (IsInside(currentPosition))
                    {
                        currentSector = GetSector(currentPosition);
                        if (currentSector != prevHandledSector)
                        {
                            if (sectorHandler(currentSector))
                            {
                                return;
                            }
                            prevHandledSector = currentSector;
                        }
                    }

                    y += stepY;
                }
            }

            if (IsInside(endPoint))
            {
                var endSector = GetSector(endPoint);
                if (endSector != prevHandledSector)
                {
                    sectorHandler(endSector);
                }
            }
        }
        
        public IEnumerable<T> GetNeighbors(T sector)
        {
            var right = GetSector(sector.IndexX, (short) (sector.IndexY + 1));
            if (right != null)
            {
                yield return right;
            }
            var upRight = GetSector((short)(sector.IndexX-1), (short) (sector.IndexY + 1));
            if (upRight != null)
            {
                yield return upRight;
            }
            var upLeft = GetSector((short)(sector.IndexX-1), (short) (sector.IndexY - 1));
            if (upLeft != null)
            {
                yield return upLeft;
            }

            var up = GetSector((short) (sector.IndexX - 1), sector.IndexY);
            if (up != null)
            {
                yield return up;
            }

            var left = GetSector(sector.IndexX, (short) (sector.IndexY - 1));
            if (left != null)
            {
                yield return left;
            }

            var down = GetSector((short) (sector.IndexX + 1), sector.IndexY);
            if (down != null)
            {
                yield return down;
            }
            
            var downRight = GetSector((short)(sector.IndexX+1), (short) (sector.IndexY + 1));
            if (downRight != null)
            {
                yield return downRight;
            }
            var downLeft = GetSector((short)(sector.IndexX+1), (short) (sector.IndexY - 1));
            if (downLeft != null)
            {
                yield return downLeft;
            }
        }
    }
}