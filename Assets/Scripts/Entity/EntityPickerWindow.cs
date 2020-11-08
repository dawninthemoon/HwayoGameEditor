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

    void Awake() {
        _buttons = new Dictionary<int, Button>();
    }

    public void SetOnEntityPicked(OnPicked callback) {
        _onPicked = callback;
    }

    public void ChangeEntityName(int id, string name) {
        if (_buttons.TryGetValue(id, out Button button)) {
            button.GetComponentInChildren<Text>().text = name;
        }
    }

    public void AddButton(Entity entity) {
        var button = Instantiate(_buttonPrefab, _contentTrnasform);
        _buttons.Add(entity.EntityID, button);

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
            _onPicked(entity.EntityID);
            _entityModel.SelectedIndex = entity.EntityID;
            HighlightImage(button.GetComponent<Image>());
        }
    }

    public void DeleteAllButtons() {
        foreach (var button in _buttons.Values) {
            button.transform.SetParent(null);
            button.gameObject.SetActive(false);
            DestroyImmediate(button);
        }
        _buttons.Clear();
    }

    public void DeleteButton(int curID) {
        if (_buttons.TryGetValue(curID, out Button button)) {
            _buttons.Remove(curID);
                
            button.transform.SetParent(null);
            button.gameObject.SetActive(false);
            DestroyImmediate(button);
        }
        else
            Debug.Log("Key not exists");
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