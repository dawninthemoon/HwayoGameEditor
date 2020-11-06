using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HSVPicker;
using CustomTilemap;

public class ProjectEntityWindow : SlideableUI {
    [SerializeField] Transform _entityPickerContentTransform = null, _fieldPickerContentTransform = null;
    [SerializeField] RawImage _editorVisualPreview = null;
    [SerializeField] EntityModel _entityModel = null;
    [SerializeField] EditorView _editorView = null;
    [SerializeField] Button _entityButtonPrefab = null;
    [SerializeField] Button _fieldButtonPrefab = null;
    [SerializeField] GameObject _fieldInputFieldRect = null;
    [SerializeField] GameObject _entityInfoRect = null;
    InputField _fieldInputField;
    float[] _uvRectXArr = new float[3] { 0f, 0.34f, 0.67f };
    static readonly string DefaultEntityName = "Default Entity";
    static readonly string DefaultFieldName = "Default Field";
    int _numOfEntities;
    int _selectedEntityID = -1;
    int _selectedFieldID = -1;
    Image _selectedEntityButtonImage;
    bool _ignoreCallback;
    ObjectPool<Button> _fieldButtonPool;
    Dictionary<int, Button> _entityFieldButtons = new Dictionary<int, Button>();
    string _prevFieldName = null;

    public override void Initalize() {
        base.Initalize();
        _fieldInputField = _fieldInputFieldRect.GetComponentInChildren<InputField>();
        _fieldInputField.text = DefaultFieldName + " 0";
        _fieldButtonPool = new ObjectPool<Button>(5, () => Instantiate(_fieldButtonPrefab, _fieldPickerContentTransform));
        gameObject.SetActive(false);
    }

    public void LoadEntites() {
        foreach (var button in _entityFieldButtons.Values) {
            button.transform.SetParent(null);
            _fieldButtonPool.ReturnObject(button);
        }
        _entityFieldButtons.Clear();
        for (int i = 0; i < _entityPickerContentTransform.childCount; ++i) {
            var button = _entityPickerContentTransform.GetChild(i);
            button.SetParent(null);
            DestroyImmediate(button.gameObject);
            --i;
        }

        _selectedEntityID = _selectedFieldID = -1;
        int max = -1;
        foreach (var entity in _entityModel.EntityDictionary.Values) {
            CreateButtonByEntity(entity);
            max = Mathf.Max(entity.EntityID, max);
        }
        _numOfEntities = max + 1;
    }

    public void AddField() {
        Entity entity = _entityModel.GetEntityByID(_selectedEntityID);
        int fieldID = entity.GetFieldCount();
        string fieldName = DefaultFieldName + " " + fieldID;
        SetField(fieldName, fieldID);

        entity.AddField(fieldName);
        _entityModel.SaveEntites();
    }

    public void RemoveField() {
        if (_selectedFieldID == -1) return;

        var button = _entityFieldButtons[_selectedFieldID];
        button.transform.SetParent(null);
        _fieldButtonPool.ReturnObject(button);
        _entityFieldButtons.Remove(_selectedEntityID);

        var entity = _entityModel.GetEntityByID(_selectedFieldID);
        entity.Fields.Clear();
        foreach (var btn in _entityFieldButtons.Values) {
            entity.Fields.Add(btn.GetComponentInChildren<Text>().text);
        }
        _entityModel.SaveEntites();
    }

    void SetField(string fieldName, int id) {
        var button = _fieldButtonPool.GetObject();
        button.transform.SetParent(_fieldPickerContentTransform);

        button.GetComponentInChildren<Text>().text = fieldName;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            _ignoreCallback = true;
            _fieldInputFieldRect.SetActive(true);
            _selectedFieldID = id;
            _fieldInputField.text = fieldName;
            _prevFieldName = fieldName;
            _ignoreCallback = false;
        });
        _entityFieldButtons.Add(id, button);
    }

    public void OnFieldNameChanged() {
        if (_ignoreCallback) return;
        string fieldName = _fieldInputField.text;
        var entity = _entityModel.GetEntityByID(_selectedEntityID);
        entity.ChangeFieldName(_prevFieldName, fieldName);

        foreach (var button in _entityFieldButtons.Values) {
            var text = button.GetComponentInChildren<Text>();
            if (text.text.Equals(_prevFieldName)) {
                text.text = fieldName;
                break;
            }
        }
        _prevFieldName = fieldName;
    }

    public void CreateEntity() {
        var entity = new Entity(DefaultEntityName + " " + _numOfEntities.ToString(), 0, _numOfEntities, Color.white);
        CreateButtonByEntity(entity);
        _entityModel.AddEntity(entity, _numOfEntities++);
    }

    void CreateButtonByEntity(Entity entity) {
        var button = Instantiate(_entityButtonPrefab, _entityPickerContentTransform);
        button.GetComponentInChildren<Text>().text = entity.EntityName;

        int index = entity.EntityID;
        button.onClick.AddListener(() => {
            _selectedEntityID = index;
            OnEntityButtonDown(entity);
            HighlightEntity(button.GetComponent<Image>());
        });

        if (index == 0 || _entityModel.IsEntityEmpty()) {
            button.onClick.Invoke();
        }
    }

    public void DeleteSelectedEntity() {
        if (_selectedEntityID == -1) return;
        _entityModel.DeleteEntityByID(_selectedEntityID);

        var button = _selectedEntityButtonImage.GetComponent<Button>();
        button.transform.SetParent(null);
        button.gameObject.SetActive(false);

        DestroyImmediate(button);
        _selectedEntityID = -1;
    }

    void HighlightImage(Image buttonImage) {
        if (_selectedEntityButtonImage != null) {
            _selectedEntityButtonImage.color = new Color(0.2117647f, 0.2f, 0.2745098f);
            _selectedEntityButtonImage.GetComponentInChildren<Text>().color = Color.white;
        }
        buttonImage.color = new Color(1f, 0.7626624f, 0.2122642f);
        buttonImage.GetComponentInChildren<Text>().color = Color.black;
    }

    void HighlightEntity(Image buttonImage) {
        HighlightImage(buttonImage);
        _selectedEntityButtonImage = buttonImage;
    }

    void HighlightInputField(Image buttonImage) {
        HighlightImage(buttonImage);
    }

    void OnEntityButtonDown(Entity entity) {
        _ignoreCallback = true;

        _entityInfoRect.SetActive(true);

        _editorView.EntityColorPicker.CurrentColor = entity.EntityColor;
        _editorView.EntityNameInputField.text = entity.EntityName;
        _editorView.EntityVisualDropdown.value = entity.TextureIndex;
        _editorVisualPreview.color = entity.EntityColor;

        var uvRect = _editorVisualPreview.uvRect;
        uvRect.x = _uvRectXArr[entity.TextureIndex];
        _editorVisualPreview.uvRect = uvRect;

        foreach (var button in _entityFieldButtons.Values) {
            button.transform.SetParent(null);
            _fieldButtonPool.ReturnObject(button);
        }
        _entityFieldButtons.Clear();
        for (int i = 0; i < entity.Fields.Count; ++i) {
            SetField(entity.Fields[i], i);
        }

        _ignoreCallback = false;
    }

    public void OnEntityNameChanged(InputField inputField) {
        if (_ignoreCallback) return;
        Entity entity = _entityModel.GetEntityByID(_selectedEntityID);
        entity.EntityName = inputField.text;
        _selectedEntityButtonImage.GetComponentInChildren<Text>().text = inputField.text;
    }

    public void OnEditorVisualDropdownChanged(Dropdown dropdown) {
        if (_ignoreCallback) return;
        var uvRect = _editorVisualPreview.uvRect;
        uvRect.x = _uvRectXArr[dropdown.value];
        _editorVisualPreview.uvRect = uvRect;

        Entity entity = _entityModel.GetEntityByID(_selectedEntityID);
        entity.TextureIndex = dropdown.value;
    }

    public void OnColorChanged(ColorPicker colorPicker) {
        if (_ignoreCallback) return;
        _editorVisualPreview.color = colorPicker.CurrentColor;
        Entity entity = _entityModel.GetEntityByID(_selectedEntityID);
        if (entity == null) return;
        entity.EntityColor = colorPicker.CurrentColor;
    }
}