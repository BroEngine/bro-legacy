using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class TilesGenerator : MonoBehaviour
    {
        [SerializeField] private List<Sprite> _tiles;
        [SerializeField] private float _heightDelta;
        [SerializeField] private float _widthDelta;
        [SerializeField] private float _heightOffset;
        [SerializeField] private float _widthOffset;
        [SerializeField] private int _count;

        public void GenerateTileMap()
        {
            var isLight = false;

            for (int i = 0; i < _count; i++)
            {
                isLight = !isLight;
                for (int j = 0; j < _count; j++)
                {
                    var sprite = isLight ? _tiles[0] : _tiles[1];
                    var startXOffset = i * _widthOffset;
                    var positionX = startXOffset + gameObject.transform.position.x + _widthDelta * j;
                    var positionY = gameObject.transform.position.y + _heightDelta * i + _heightOffset*j;
                    var position = new Vector3(positionX, positionY, 0f);
                    var objectName = $"desert_tile_{i}_{j}";
                    isLight = !isLight;

                    var go = new GameObject(objectName);
                    var spriteRenderer = go.AddComponent<SpriteRenderer>();
                    spriteRenderer.sprite = sprite;
                    go.transform.position = position;
                    go.transform.parent = transform;
                }
            }
        }
    }  
}

