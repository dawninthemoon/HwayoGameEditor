using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomTilemap;

public class LayerPicker : MonoBehaviour {
    [SerializeField] Transform _scrollViewContent = null;
    [SerializeField] Button _buttonPrefab = null;
    [SerializeField] TilesetModel _tilesetModel = null;
    [SerializeField] TilemapPickerWindow _tilemapPicker = null;
    [SerializeField] EntityPickerWindow _entityPicker = null;
    Dictionary<int, Button> _buttons = new Dictionary<int, Button>();
    Image _selectedButtonImage;
    OnTilesetChanged _onTilesetChanged;
    public void SetOnTilesetChanged(OnTilesetChanged callback) {
        _onTilesetChanged = callback;
    }
    public void AddLayerButton(LayerModel layerModel, Layer layer) {
        var button = Instantiate(_buttonPrefab, _scrollViewContent);
        button.GetComponentInChildren<Text>().text = layer.LayerName;
        if (layerModel.NumOfLayers() == 1) {
            layerModel.SelectedLayerID = 0;
            HighlightButton(button);

            if (layer is TileLayer) {
                _onTilesetChanged(layer as TileLayer);
            }
            OnButtonClicked(button, layerModel, layer);
        }
        button.onClick.AddListener(() => { OnButtonClicked(button, layerModel, layer); });
        _buttons.Add(layer.LayerID, button);
    }

    public void DeleteLayerButton(int curID) {
        var button = _buttons[curID];
        _buttons.Remove(curID);
            
        button.transform.SetParent(null);
        button.gameObject.SetActive(false);
        DestroyImmediate(button);
    }

    void OnButtonClicked(Button button, LayerModel layerModel, Layer layer) {
        layerModel.SelectedLayerID = layer.LayerID;
        HighlightButton(button);
        if (layer is TileLayer) {
            TileLayer tileLayer = layer as TileLayer;

            _tilemapPicker.gameObject.SetActive(true);
            _entityPicker.gameObject.SetActive(false);

            if (!tileLayer.Visual.GetTileSetName().Equals(tileLayer.TilesetName)) {
                _tilesetModel.ChangeTileset(tileLayer.TilesetName, layer.LayerID);
            }
            _onTilesetChanged(tileLayer);
        }
        else if (layer is EntityLayer) {
            _tilemapPicker.gameObject.SetActive(false);
            _entityPicker.gameObject.SetActive(true);
        }
    }

    void HighlightButton(Button button) {
        if (_selectedButtonImage != null) {
            _selectedButtonImage.color = new Color(0.2117647f, 0.2f, 0.2745098f);
            _selectedButtonImage.GetComponentInChildren<Text>().color = Color.white;
        }
        _selectedButtonImage = button.image;

        button.image.color = new Color(1f, 0.7626624f, 0.2122642f);
        _selectedButtonImage.GetComponentInChildren<Text>().color = Color.black;
    }
}
