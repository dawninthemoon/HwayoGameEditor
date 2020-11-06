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
    [SerializeField] EntityPickerWindow _entityPickerWindow = null;
    [SerializeField] LayerPicker _layerPicker = null;
    [SerializeField] TileLayerWindow _tileLayerWindow = null;
    int _selectedTileIndex = 0;
    SlideableUI[] _projectWindows;

    void Awake() {
        _layerPicker.SetOnTilesetChanged(OnTilesetChanged);
        _tileLayerWindow.SetOnTilesetChanged(_layerPicker.ChangeTileset);

        _tilemapPickerWindow.AddOnTilemapPicked((int index) => { _selectedTileIndex = index; });
        _entityPickerWindow.SetOnEntityPicked((int index) => { _selectedTileIndex = index; });
    }

    void Start() {
        var projectWindows = GameObject.FindGameObjectsWithTag("ProjectWindow");
        _projectWindows = new SlideableUI[projectWindows.Length];
        for (int i = 0; i < projectWindows.Length; ++i) {
            _projectWindows[i] = projectWindows[i].GetComponent<SlideableUI>();
            _projectWindows[i].Initalize();
        }
    }

    public void DisableAllProjectWindows() {
        foreach (var window in _projectWindows) {
            window.SlideIn();
        }
    }

    public void OnTilesetChanged(TileLayer tileLayer) {
        var spr = _tilesetModel.GetTilesetSprite(tileLayer.TilesetName);
        _editorView.TilesetPreivewImage.sprite = spr;
        _editorView.TilesetPickerImage.sprite = spr;
        _editorView.TilesetPickerImage.rectTransform.sizeDelta = new Vector2(spr.texture.width, spr.texture.height);
        _tilemapPickerWindow.CalculateGridPos();
        _selectedTileIndex = _tilemapPickerWindow.ResetSelectedTile();
    }

    void Update() {
        if (GridScaler.ScalerDraging) return;
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)) return;
        
        bool isOnUI = CheckMousePositionIsOnUI();

        if (!isOnUI && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))) {
            DisableAllProjectWindows();
            _editorView.EntityEditWindow.DisableWindow();
        }

        if (_layerModel.SelectedLayerID == -1) return;
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
