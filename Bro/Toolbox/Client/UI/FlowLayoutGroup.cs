﻿#if ( UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII )

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Bro.Toolbox.Client.UI
{

    [AddComponentMenu("Layout/Flow Layout Group", 153)]
    public class FlowLayoutGroup : LayoutGroup
    {
        [SerializeField] private bool _takeTotalHeight = true;

     

        protected Vector2 m_CellSize = new Vector2(100, 100);

        public Vector2 cellSize
        {
            get { return m_CellSize; }
            set { SetProperty(ref m_CellSize, value); }
        }

        [SerializeField] protected Vector2 m_Spacing = Vector2.zero;

        public Vector2 spacing
        {
            get { return m_Spacing; }
            set { SetProperty(ref m_Spacing, value); }
        }

        protected FlowLayoutGroup()
        {
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            int minColumns = 0;
            int preferredColumns = 0;

            minColumns = 1;
            preferredColumns = Mathf.CeilToInt(Mathf.Sqrt(rectChildren.Count));

            SetLayoutInputForAxis(
                padding.horizontal + (cellSize.x + spacing.x) * minColumns - spacing.x,
                padding.horizontal + (cellSize.x + spacing.x) * preferredColumns - spacing.x,
                -1, 0);
        }

        public override void CalculateLayoutInputVertical()
        {
            var minRows = 1; 
            var minSpace = padding.vertical + (cellSize.y + spacing.y) * minRows - spacing.y;
            SetLayoutInputForAxis(minSpace, minSpace, -1, 1);
        }

        public override void SetLayoutHorizontal()
        {
            SetCellsAlongAxis(0);
        }

        public override void SetLayoutVertical()
        {
            SetCellsAlongAxis(1);
        }

        int cellsPerMainAxis, actualCellCountX, actualCellCountY;
        int positionX;
        int positionY;
        float totalWidth = 0;
        float totalHeight = 0;

        float lastMaxHeight = 0;

        private void SetCellsAlongAxis(int axis)
        {
            // Normally a Layout Controller should only set horizontal values when invoked for the horizontal axis
            // and only vertical values when invoked for the vertical axis.
            // However, in this case we set both the horizontal and vertical position when invoked for the vertical axis.
            // Since we only set the horizontal position and not the size, it shouldn't affect children's layout,
            // and thus shouldn't break the rule that all horizontal layout must be calculated before all vertical layout.

            float width = rectTransform.rect.size.x;
            float height = rectTransform.rect.size.y;

            int cellCountX = 1;
            int cellCountY = 1;

            if (cellSize.x + spacing.x <= 0)
                cellCountX = int.MaxValue;
            else
                cellCountX = Mathf.Max(1,
                    Mathf.FloorToInt((width - padding.horizontal + spacing.x + 0.001f) / (cellSize.x + spacing.x)));

            if (cellSize.y + spacing.y <= 0)
                cellCountY = int.MaxValue;
            else
                cellCountY = Mathf.Max(1,
                    Mathf.FloorToInt((height - padding.vertical + spacing.y + 0.001f) / (cellSize.y + spacing.y)));

            cellsPerMainAxis = cellCountX;
            actualCellCountX = Mathf.Clamp(cellCountX, 1, rectChildren.Count);
            actualCellCountY =
                Mathf.Clamp(cellCountY, 1, Mathf.CeilToInt(rectChildren.Count / (float) cellsPerMainAxis));

            Vector2 requiredSpace = new Vector2(
                actualCellCountX * cellSize.x + (actualCellCountX - 1) * spacing.x,
                actualCellCountY * cellSize.y + (actualCellCountY - 1) * spacing.y
            );
            Vector2 startOffset = new Vector2(
                GetStartOffset(0, requiredSpace.x),
                GetStartOffset(1, requiredSpace.y)
            );

            totalWidth = 0;
            totalHeight = 0;
            Vector2 currentSpacing = Vector2.zero;
            for (int i = 0; i < rectChildren.Count; i++)
            {
                SetChildAlongAxis(rectChildren[i], 0, startOffset.x + totalWidth /*+ currentSpacing[0]*/,
                    rectChildren[i].rect.size.x);
                SetChildAlongAxis(rectChildren[i], 1, startOffset.y + totalHeight /*+ currentSpacing[1]*/,
                    rectChildren[i].rect.size.y);

                currentSpacing = spacing;

                totalWidth += rectChildren[i].rect.width + currentSpacing[0];

                if (_takeTotalHeight)
                {
                    if (rectChildren[i].rect.height > lastMaxHeight)
                    {
                        lastMaxHeight = rectChildren[i].rect.height;
                    }
                }
                else
                {
                    lastMaxHeight = rectChildren[i].rect.height;
                }

                if (i < rectChildren.Count - 1)
                {
                    if (totalWidth + rectChildren[i + 1].rect.width + currentSpacing[0] > width)
                    {
                        totalWidth = 0;
                        totalHeight += lastMaxHeight + currentSpacing[1];
                        lastMaxHeight = 0;
                    }
                }
            }
        }

        public void ReDraw()
        {
            SetDirty();
            StartCoroutine(DelayedSetDirty(rectTransform));
        }
        
        IEnumerator DelayedSetDirty(RectTransform rectanagleTransform)
        {
            yield return null;
            if (!CanvasUpdateRegistry.IsRebuildingLayout())
            {
                LayoutRebuilder.MarkLayoutForRebuild(rectanagleTransform);
            }
        }
        
    }
}

#endif