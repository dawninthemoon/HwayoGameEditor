using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public class TilemapVisual : MonoBehaviour {
        private struct UVCoords {
            public Vector2 uv00;
            public Vector2 uv11;
        }

        [SerializeField] float _tileSize = 1f;

        private Grid _grid;
        private Mesh _mesh;
        private bool _updateMesh;
        private Dictionary<int, UVCoords> _uvCoordsDictionary;
        MeshRenderer _meshRenderer;
        public string GetTileSetName() {
            return _meshRenderer.material.mainTexture.name;
        }
        void Awake() {
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
        
        public void SetGrid(Layer tilemap, Grid grid) {
            this._grid = grid;
            UpdateHeatMapVisual();

            grid.OnGridObjectChanged += Grid_OnGridValueChanged;
        }

        private void Tilemap_OnLoaded(object sender, System.EventArgs e) {
            _updateMesh = true;
        }

        private void Grid_OnGridValueChanged(object sender, Grid.OnGridObjectChangedEventArgs e) {
            _updateMesh = true;
        }

        private void LateUpdate() {
            if (_mesh == null) return;
            if (_updateMesh) { 
                _updateMesh = false;
                UpdateHeatMapVisual();
            }
        }

        private void UpdateHeatMapVisual() {
            MeshUtils.CreateEmptyMeshArrays(_grid.GetWidth() * _grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

            for (int x = 0; x < _grid.GetWidth(); x++) {
                for (int y = 0; y < _grid.GetHeight(); y++) {
                    int index = x * _grid.GetHeight() + y;
                    Vector3 quadSize = new Vector3(1, 1) * _grid.GetCellSize();

                    Layer.TileObject gridObject = _grid.GetGridObject(x, y);
                    int tileIndex = gridObject.GetTileIndex();
                    Vector2 gridUV00, gridUV11;
                    if (tileIndex == -1) {
                        gridUV00 = Vector2.zero;
                        gridUV11 = Vector2.zero;
                        quadSize = Vector3.zero;
                    } else {
                        UVCoords uvCoords = _uvCoordsDictionary[tileIndex];
                        gridUV00 = uvCoords.uv00;
                        gridUV11 = uvCoords.uv11;
                    }
                    MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, _grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridUV00, gridUV11);
                }
            }

            _mesh.vertices = vertices;
            _mesh.uv = uv;
            _mesh.triangles = triangles;
        }
    }
}

