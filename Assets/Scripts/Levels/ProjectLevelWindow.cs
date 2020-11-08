using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomTilemap;

public class ProjectLevelWindow : SlideableUI {
    [SerializeField] Transform _contentTransform = null;
    [SerializeField] GameObject _inputFieldObject = null;
    [SerializeField] Button _levelButtonPrefab = null;
    [SerializeField] LevelModel _levelModel = null;
    [SerializeField] InputField _inputField = null;
    [SerializeField] EntityEditWindow _entityEditWindow = null;
    [SerializeField] GameObject _warningTextObj = null;
    Dictionary<int, Button> _buttons = new Dictionary<int, Button>();
    Level _tempLevel;
    Button _tempButton;
    static string DefaultLevelName = "Default Level";
    int _numOfLevels;
    int _selectedLevelID = -1;
    Image _selectedButtonImage;
    bool _ignoreCallback;

    public override void Initalize() {
        base.Initalize();

        int max = -1;
        foreach (var level in _levelModel.LevelDictionary.Values) {
            CreateButtonByLevel(level);
            max = Mathf.Max(level.LevelID, max);
        }
        _numOfLevels = max + 1;

        _selectedLevelID = LevelModel.CurrentLevelID;
        HighlightButton(_buttons[LevelModel.CurrentLevelID]);
        _inputFieldObject.SetActive(true);
        gameObject.SetActive(false);
    }

    void OnLevelButtonDown(Level level, Button button) {
        _tempLevel = level;
        _tempButton = button;
        _warningTextObj.SetActive(true);
    }

    public void OnContinueButtonDown() {
        _ignoreCallback = true;
        HighlightButton(_tempButton);
        _inputField.text = _tempLevel.LevelName;
        _inputFieldObject.SetActive(true);
        _levelModel.ChangeLevel(_selectedLevelID);
        _entityEditWindow.DisableWindow();
        _warningTextObj.SetActive(false);
        _ignoreCallback = false;
    }

    public void OnCancelButtonDown() {
        _tempLevel = null;
        _tempButton = null;
        _warningTextObj.SetActive(false);
    }

    public void CreateLevel() {
        string levelName = DefaultLevelName;
        Level level = new Level(_numOfLevels, levelName);

        CreateButtonByLevel(level);
        _levelModel.AddLevel(level);

        ++_numOfLevels;
    }

    void CreateButtonByLevel(Level level) {
        var button = Instantiate(_levelButtonPrefab, _contentTransform);
        button.GetComponentInChildren<Text>().text = level.LevelName;
        button.gameObject.name = level.LevelName;

        button.onClick.AddListener(() => {
            _selectedLevelID = level.LevelID;
            OnLevelButtonDown(level, button);
        });
        _buttons.Add(level.LevelID, button);
    }

    public void DeleteSelectedButton() {
        if (_buttons.TryGetValue(_selectedLevelID, out Button button)) {
            button.transform.SetParent(null);
            button.gameObject.SetActive(false);
            DestroyImmediate(button);

            _buttons.Remove(_selectedLevelID);
            _levelModel.RemoveLevelByID(_selectedLevelID);
        }
        else {
            Debug.Log("Button ID not exist");
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

    public void OnLevelNameChanged() {
        if (_ignoreCallback) return;
        var level = _levelModel.GetLevelByID(_selectedLevelID);
        level.LevelName = _inputField.text;
        _buttons[level.LevelID].GetComponentInChildren<Text>().text = level.LevelName;
        _buttons[level.LevelID].gameObject.name = level.LevelName;
    }
}
