using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CustomTilemap;
using Aroma;

public class EditorMain : MonoBehaviour {
    [SerializeField] LayerModel _layerModel = null;
    [SerializeField] TilesetModel _tilesetModel = null;
    [SerializeField] EditorView _editorView = null;
    [SerializeField] TilemapPickerWindow _tilemapPickerWindow = null;
    int _selectedTileIndex = 0;
    SlideableUI[] _projectWindows;

    void Start() {
        var projectWindows = GameObject.FindGameObjectsWithTag("ProjectWindow");
        _projectWindows = new SlideableUI[projectWindows.Length];
        for (int i = 0; i < projectWindows.Length; ++i) {
            _projectWindows[i] = projectWindows[i].GetComponent<SlideableUI>();
            _projectWindows[i].Initalize();
        }
        _layerModel.SetOnTilesetChanged(OnTilesetChanged);

        _tilemapPickerWindow.AddOnTilemapPicked((int index) => {
                _selectedTileIndex = index;
            }
        );
    }

    public void OnTilesetChanged(TilemapVisual tilemapVisual) {
        var spr = _tilesetModel.GetTilesetSprite(tilemapVisual.GetTileSetName());
        _editorView.TilesetPreivewImage.sprite = spr;
        _editorView.TilesetPickerImage.sprite = spr;
        _editorView.TilesetPickerImage.rectTransform.sizeDelta = new Vector2(spr.texture.width, spr.texture.height);
        _tilemapPickerWindow.CalculateGridPos();
        _selectedTileIndex = _tilemapPickerWindow.ResetSelectedTile();
    }

    void Update() {
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) return;

        bool isOnUI = CheckMousePositionIsOnUI();

        if (!isOnUI) {
            foreach (var window in _projectWindows) {
                window.SlideIn();
            }
        }

        if (Input.GetMouseButton(0) && !isOnUI) {
            Vector3 worldPosition = Utility.GetMouseWorldPosition();
            _layerModel.SetTile(worldPosition, _selectedTileIndex);
        }
        if (Input.GetMouseButton(1) && !isOnUI) {
            Vector3 worldPosition = Utility.GetMouseWorldPosition();
            _layerModel.SetTile(worldPosition, -1);
        }
    }

    bool CheckMousePositionIsOnUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
