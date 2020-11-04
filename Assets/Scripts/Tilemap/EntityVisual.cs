using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTilemap {
    public class EntityVisual : TilemapVisual {
        private Grid<EntityLayer.EntityObject> _grid;
        protected override void Awake() {
            base.Awake();
        }
        public void SetGrid(Layer tilemap, Grid<EntityLayer.EntityObject> grid) {
            this._grid = grid;
            UpdateHeatMapVisual();
            grid.OnGridObjectChanged += Grid_OnGridValueChanged;
        }

        private void Tilemap_OnLoaded(object sender, System.EventArgs e) {
            _updateMesh = true;
        }

        private void Grid_OnGridValueChanged(object sender, Grid<EntityLayer.EntityObject>.OnGridObjectChangedEventArgs e) {
            _updateMesh = true;
        }

        protected override void LateUpdate() {
            base.LateUpdate();
        }

        public override void UpdateHeatMapVisual() {
            _mesh.Clear();
            MeshUtils.CreateEmptyMeshArrays(_grid.GetWidth() * _grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);
            
            for (int x = 0; x < _grid.GetWidth(); ++x) {
                for (int y = 0; y < _grid.GetHeight(); ++y) {
                    int index = x * _grid.GetHeight() + y;
                    Vector3 quadSize = new Vector3(1, 1) * _grid.GetCellSize();

                    EntityLayer.EntityObject gridObject = _grid.GetGridObject(x, y);
                    int textureIndex = gridObject.GetIndex();
                    Vector2 gridUV00, gridUV11;
                    if (textureIndex == -1) {
                        gridUV00 = Vector2.zero;
                        gridUV11 = Vector2.zero;
                        quadSize = Vector3.zero;
                    } else {
                        UVCoords uvCoords = _uvCoordsDictionary[textureIndex];
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