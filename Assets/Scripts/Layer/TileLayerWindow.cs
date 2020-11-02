using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CustomTilemap {
    public class TileLayerWindow : MonoBehaviour {
        [SerializeField] LayerModel _layerModel = null;
        [SerializeField] TilesetModel _tilesetModel = null;
        [SerializeField] EditorView _editorView = null;
        ProjectLayerWindow _projectLayerWindow;
        bool _ignoreCallback;

        void Start() {
            _projectLayerWindow = GetComponentInParent<ProjectLayerWindow>();
        }

        public void SetDropdownValueWithIgnoreCallback(Layer tilemapLayer) {
            _ignoreCallback = true;
            _editorView.LayerTilesetNameDropdown.value = tilemapLayer.DropdownValue;
            _ignoreCallback = false;
        }

        public void SetInputFieldTextWithIgnoreCallback(Layer tilemapLayer) {
            _ignoreCallback = true;
            _editorView.LayerNameInputField.text = tilemapLayer.LayerName;
            _ignoreCallback = false;
        }

        public void OnTilesetDropdownChanged(Dropdown dropdown) {
            if (_ignoreCallback) return;

            string tilesetName = dropdown.options[dropdown.value].text;
            var layer = _layerModel.GetCurrentTilemapLayer();
            layer.DropdownValue = dropdown.value;
            _tilesetModel.ChangeTileset(tilesetName, layer.LayerIndex);
            _editorView.TilesetPickerImage.sprite = _tilesetModel.GetTilesetSprite(tilesetName);
        }

        public void OnLayerNameInputFieldChanged(InputField inputField) {
            if (_ignoreCallback) return;

            var layer = _layerModel.GetCurrentTilemapLayer();
            layer.LayerName = inputField.text;
            Button button = _projectLayerWindow.GetButtonByIndex(layer.LayerIndex);
            button.GetComponentInChildren<Text>().text = layer.LayerName;
        }
    }
}