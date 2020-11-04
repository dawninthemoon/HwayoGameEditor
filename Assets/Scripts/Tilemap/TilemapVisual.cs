using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public abstract class TilemapVisual : MonoBehaviour {
        protected struct UVCoords {
            public Vector2 uv00;
            public Vector2 uv11;
        }

        [SerializeField] protected float _tileSize = 16f;
        protected Mesh _mesh;
        protected bool _updateMesh;
        protected Dictionary<int, UVCoords> _uvCoordsDictionary;
        MeshRenderer _meshRenderer;
        public string GetTileSetName() {
            return _meshRenderer.material.mainTexture.name;
        }
        protected virtual void Awake() {
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        public void Initalize(Material material) {
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;

            _meshRenderer.material = material;
            Texture texture = material.mainTexture;
            float textureWidth = texture.width;
            float textureHeight = texture.height;

            _uvCoordsDictionary = new Dictionary<int, UVCoords>();
            
            int xCount = Mathf.FloorToInt(textureWidth / _tileSize);
            int yCount = Mathf.FloorToInt(textureHeight / _tileSize);
            float paddingX = 1f / xCount;
            float paddingY = 1f / yCount;

            int index = 0;
            for (int i = 0; i < yCount; ++i) {
                for (int j = 0; j < xCount; ++j) {
                    float x = paddingX * j;
                    float y = paddingY * (yCount - i - 1);

                    _uvCoordsDictionary[index++] = new UVCoords {
                        uv00 = new Vector2(x, y),
                        uv11 = new Vector2(x + paddingX, y + paddingY),
                    };
                }
            }
        }
        
        protected virtual void LateUpdate() {
            if (_mesh == null) return;
            if (_updateMesh) { 
                _updateMesh = false;
                UpdateHeatMapVisual();
            }
        }

        public abstract void UpdateHeatMapVisual();
    }
}

