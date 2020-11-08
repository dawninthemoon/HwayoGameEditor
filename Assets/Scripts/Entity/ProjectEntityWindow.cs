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
    Image _selectedEntityButtonImage, _selectedFieldButtonImage;
    bool _ignoreCallback;
    ObjectPool<Button> _fieldButtonPool;
    List<Button> _entityFieldButtons = new List<Button>();
    string _prevFieldName = null;

    public override void Initalize() {
        base.Initalize();
        _fieldInputField = _fieldInputFieldRect.GetComponentInChildren<InputField>();
        _fieldInputField.text = DefaultFieldName + " 0";
        if (_fieldButtonPool == null)
            _fieldButtonPool = new ObjectPool<Button>(20, () => {
                var button = Instantiate(_fieldButtonPrefab, _fieldPickerContentTransform);
                button.transform.SetParent(null);
                return button;
            });
        gameObject.SetActive(false);
    }

    public void LoadEntites() {
        foreach (var button in _entityFieldButtons) {
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
        string fieldName = DefaultFieldName;
        SetField(fieldName);
        ++entity.FieldSequence;

        entity.AddField(fieldName);
    }

    public void RemoveField() {
        if (_selectedFieldID == -1) return;

        var button = _entityFieldButtons[_selectedFieldID];
        button.transform.SetParent(null);
        _fieldButtonPool.ReturnObject(button);
        _entityFieldButtons.RemoveAt(_selectedFieldID);

        var entity = _entityModel.GetEntityByID(_selectedEntityID);
        entity.Fields.RemoveAt(_selectedFieldID);
    }

    void SetField(string fieldName) {
        if (_fieldButtonPool == null) {
            _fieldButtonPool =_fieldButtonPool = new ObjectPool<Button>(20, () => {
                var btn = Instantiate(_fieldButtonPrefab, _fieldPickerContentTransform);
                btn.transform.SetParent(null);
                return btn;
            });
        }
            
        var button = _fieldButtonPool.GetObject();
        button.transform.SetParent(_fieldPickerContentTransform);
        button.transform.localScale = Vector3.one;
        button.image.color = new Color(0.8980392f, 0.2470588f, 0f);

        button.GetComponentInChildren<Text>().text = fieldName;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => {
            _ignoreCallback = true;
            _fieldInputFieldRect.SetActive(true);
            _selectedFieldID = button.transform.GetSiblingIndex();
            _fieldInputField.text = fieldName;
            _prevFieldName = fieldName;
            _ignoreCallback = false;
            if (_selectedFieldButtonImage != null)
                _selectedFieldButtonImage.color = new Color(0.8980392f, 0.2470588f, 0f);
            _selectedFieldButtonImage = button.image;
            _selectedFieldButtonImage.color = new Color(0.5943396f, 0.1635083f, 0f);
        });
        _entityFieldButtons.Add(button);
    }

    public void OnFieldNameChanged() {
        if (_ignoreCallback) return;
        string fieldName = _fieldInputField.text;
        var entity = _entityModel.GetEntityByID(_selectedEntityID);
        entity.ChangeFieldName(_prevFieldName, fieldName);
        _selectedFieldButtonImage.GetComponentInChildren<Text>().text = fieldName;
        _prevFieldName = fieldName;
    }

    public void CreateEntity() {
        var entity = new Entity(DefaultEntityName, 0, _numOfEntities, Color.white);
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

        foreach (var button in _entityFieldButtons) {
            button.transform.SetParent(null);
            _fieldButtonPool.ReturnObject(button);
        }
        _entityFieldButtons.Clear();
        for (int i = 0; i < entity.Fields.Count; ++i) {
            SetField(entity.Fields[i]);
        }

        _ignoreCallback = false;
    }

    public void OnEntityNameChanged(InputField inputField) {
        if (_ignoreCallback) return;
        Entity entity = _entityModel.GetEntityByID(_selectedEntityID);
        entity.EntityName = inputField.text;
        _selectedEntityButtonImage.GetComponentInChildren<Text>().text = inputField.text;
        _entityModel.OnEntityNameChanged(entity.EntityID, entity.EntityName);
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