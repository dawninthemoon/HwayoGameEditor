using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using CustomTilemap;
using Aroma;
using SaveInformations;
using System.IO;

public class EditorMain : MonoBehaviour {
    [SerializeField] LevelModel _levelModel = null;
    [SerializeField] LayerModel _layerModel = null;
    [SerializeField] EntityModel _entityModel = null;
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
        if ((Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S)) ||
            (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKey(KeyCode.S))) {
            SaveAll();
        }

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

    public void ExportAll() {
        var levelDictionary = ES3.Load(LevelModel.LevelDictionaryKey, new Dictionary<int, Level>());
        LevelInfo[] levelInformations = new LevelInfo[levelDictionary.Count];
        int index = 0;
        foreach (var pair in levelDictionary) {
            int id = pair.Key;
            var layerDic = ES3.Load(LayerModel.LayerDictionaryKey + "_" + id.ToString(), new Dictionary<int, Layer>());
            var entityDic = ES3.Load(EntityModel.DictionarySaveKey + "_" + id.ToString(), new Dictionary<int, Entity>());
            int defaultGridSize = PlayerPrefs.HasKey(GridUtility.DefaultGridSizeKey) ? PlayerPrefs.GetInt(GridUtility.DefaultGridSizeKey) : 16;
            var width = ES3.Load(LayerModel.GridWidthKey + "_" + id.ToString(), defaultGridSize);
            var height = ES3.Load(LayerModel.GridHeightKey + "_" + id.ToString(), defaultGridSize);
            var originPosition = ES3.Load(LayerModel.OriginPositionKey + "_" + id.ToString(), Vector3.zero);

            List<TilesetInfo> tilesetInfoList = new List<TilesetInfo>();
            List<EntityInfo> entityInfoList = new List<EntityInfo>();
            List<ColliderInfo> colliderInfoList = new List<ColliderInfo>();
            List<TileInfo> tileInfoList = new List<TileInfo>();
            foreach (var layer in layerDic.Values) {
                if (layer is TileLayer) {
                    TileLayer tileLayer = layer as TileLayer;
                    var tileObjects = tileLayer.GetTileObjects();
                    tileInfoList.Clear();
                    foreach (var obj in tileObjects) {
                        tileInfoList.Add(new TileInfo(obj.y, obj.x, obj.textureIndex));
                    }
                    tilesetInfoList.Add(new TilesetInfo(tileInfoList.ToArray(), tileLayer.TilesetName));
                }
                else if (layer is EntityLayer) {
                    var entityObjects = (layer as EntityLayer).GetTileObjects();
                    foreach (var obj in entityObjects) {
                        var fieldDic = obj.Fields;
                        FieldInfo[] fieldInfo = new FieldInfo[fieldDic.Count];
                        int i = 0;
                        foreach (var fieldPair in fieldDic) {
                            fieldInfo[i++] = new FieldInfo(fieldPair.Key, fieldPair.Value);
                        }
                        entityInfoList.Add(new EntityInfo(obj.y, obj.x, obj.EntityName, fieldInfo));
                    }
                }
                else if (layer is CollisionLayer) {
                    CollisionLayer collisionLayer = layer as CollisionLayer;
                    colliderInfoList.Add(new ColliderInfo(collisionLayer.Tag, collisionLayer.Points.ToArray()));
                }
            }
            levelInformations[index] = 
            new LevelInfo(
                pair.Value.LevelName,
                width,
                height,
                originPosition, 
                tilesetInfoList.ToArray(),
                entityInfoList.ToArray(),
                colliderInfoList.ToArray()
            );
            ++index;
        }
        string jsonText = JsonHelper.ToJson<LevelInfo>(levelInformations, true);
        File.WriteAllText(Application.dataPath + "/Saves/HwayoGameLevel.json", jsonText);
    }

    void SaveAll() {
        _levelModel.SaveLevel();
        _layerModel.SaveAllLayers();
        _entityModel.SaveEntites();
        Debug.Log("Save Complete!");
    }
}
