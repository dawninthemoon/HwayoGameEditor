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
        var tilesetMaterials = Resources.LoadAll<Material>("Tileset/");
        var tilesetSprites = Resources.LoadAll<Sprite>("Tileset/");
        _tilesetPreview.color = Color.white;

        if (tilesetMaterials == null || tilesetMaterials.Length == 0) return;

        _tilesetModel.SetTilesets(tilesetMaterials, tilesetSprites);

        for (int i = 0; i < tilesetMaterials.Length; ++i) {
            int index = i;
            var button = Instantiate(_tilesetButtonPrefab, _scrollviewContent);
            button.GetComponentInChildren<Text>().text = tilesetMaterials[i].name;
            button.onClick.AddListener(() => {
                _tilesetModel.SetTilesetIndex(index);
            });

            Dropdown.OptionData optionData = new Dropdown.OptionData(tilesetSprites[i].name);
            _editorView.LayerTilesetNameDropdown.options.Add(optionData);
        }

        _tilesetModel.SetTilesetIndex(0);
        gameObject.SetActive(false);
    }
}
