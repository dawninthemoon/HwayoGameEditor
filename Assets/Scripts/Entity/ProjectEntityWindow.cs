﻿using System.Collections;
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
    [SerializeField] InputField _fieldInputFieldPrefab = null;
    float[] _uvRectXArr = new float[3] { 0f, 0.34f, 0.67f };
    static readonly string DefaultEntityName = "Default Entity";
    static readonly string DefaultFieldName = "Field";
    int _numOfEntities;
    int _selectedEntityID = -1;
    Image _selectedEntityButtonImage;
    bool _ignoreCallback;

    public override void Initalize() {
        base.Initalize();
        gameObject.SetActive(false);
    }

    public void AddField() {
        Entity entity = _entityModel.GetEntityByID(_selectedEntityID);
        int fieldID = entity.GetFieldCount();
        string fieldName = DefaultFieldName + " " + fieldID;

        var inputField = Instantiate(_fieldInputFieldPrefab, _fieldPickerContentTransform);
        inputField.text = fieldName;

        entity.AddField(fieldName);
    }

    public void CreateEntity() {
        var entity = new Entity(DefaultEntityName + " " + _numOfEntities.ToString(), 0, Color.white);

        var button = Instantiate(_entityButtonPrefab, _entityPickerContentTransform);
        button.GetComponentInChildren<Text>().text = DefaultEntityName + " " + _numOfEntities.ToString();

        if (_entityModel.IsEntityEmpty()) {
            _selectedEntityID = 0;
            HighlightEntity(button.GetComponent<Image>());
        }

        int index = _numOfEntities;
        button.onClick.AddListener(() => {
            _selectedEntityID = index;
            OnEntityButtonDown(entity);
            HighlightEntity(button.GetComponent<Image>());
        });

        _entityModel.AddEntity(entity, index);
        ++_numOfEntities;
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
        _editorView.EntityColorPicker.CurrentColor = entity.EntityColor;
        _editorView.EntityNameInputField.text = entity.EntityName;
        _editorView.EntityVisualDropdown.value = entity.TextureIndex;
        _editorVisualPreview.color = entity.EntityColor;
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
        entity.EntityColor = colorPicker.CurrentColor;
    }
}