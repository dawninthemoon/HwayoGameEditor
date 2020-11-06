using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomTilemap;

public class ProjectTilesetsWindow : SlideableUI {
    [SerializeField] Transform _scrollviewContent = null;
    [SerializeField] Button _tilesetButtonPrefab = null;
    [SerializeField] Image _tilesetPreview = null;
    [SerializeField] TilesetModel _tilesetModel = null;
    [SerializeField] EditorView _editorView = null;

    public override void Initalize() {
        base.Initalize();
        _tilesetPreview.color = Color.white;

        var tilesetMaterials = _tilesetModel.CurrentTilesetMaterials;
        int index = 0;
        foreach (var pair in _tilesetModel.CurrentTilesetMaterials) {
            var button = Instantiate(_tilesetButtonPrefab, _scrollviewContent);
            button.GetComponentInChildren<Text>().text = pair.Value.name;
            
            int temp = index;
            button.onClick.AddListener(() => {
                _tilesetModel.SetTilesetIndex(temp);
            });
            ++index;

            Dropdown.OptionData optionData = new Dropdown.OptionData(pair.Key);
            _editorView.LayerTilesetNameDropdown.options.Add(optionData);
        }

        _tilesetModel.SetTilesetIndex(0);
        gameObject.SetActive(false);
    }
}
