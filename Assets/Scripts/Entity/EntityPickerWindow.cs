using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomTilemap;

public class EntityPickerWindow : MonoBehaviour {
    [SerializeField] Transform _contentTrnasform = null;
    [SerializeField] Button _buttonPrefab = null;
    [SerializeField] EntityModel _entityModel = null;
    public OnPicked _onPicked;
    Dictionary<int, Button> _buttons;
    Image _selectedEntityButtonImage;

    void Start() {
        _buttons = new Dictionary<int, Button>();
        _entityModel.SetOnEntityAdded(CreateButton);

        foreach (var entity in _entityModel.EntityDictionary.Values) {
            CreateButton(entity.EntityID, entity);
        }
    }

    public void SetOnEntityPicked(OnPicked callback) {
        _onPicked = callback;
    }

    void CreateButton(int id, Entity entity) {
        var button = Instantiate(_buttonPrefab, _contentTrnasform);
        _buttons.Add(id, button);

        int index = entity.TextureIndex;
        Color color = entity.EntityColor;
        
        if (_entityModel.SelectedIndex == -1) {
            OnButtonClick();
        }
        button.GetComponentInChildren<Text>().text = entity.EntityName;

        button.onClick.AddListener(() => { 
            OnButtonClick();
        });

        void OnButtonClick() {
            _onPicked(id);
            _entityModel.SelectedIndex = id;
            HighlightImage(button.GetComponent<Image>());
        }
    }

    void HighlightImage(Image buttonImage) {
        if (_selectedEntityButtonImage != null) {
            _selectedEntityButtonImage.color = new Color(0.2117647f, 0.2f, 0.2745098f);
            _selectedEntityButtonImage.GetComponentInChildren<Text>().color = Color.white;
        }
        buttonImage.color = new Color(1f, 0.7626624f, 0.2122642f);
        buttonImage.GetComponentInChildren<Text>().color = Color.black;
        _selectedEntityButtonImage = buttonImage;
    }
}