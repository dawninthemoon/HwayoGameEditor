using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Aroma;

public class ProjectSettingWindow : SlideableUI {
    [SerializeField] InputField _inputField = null;

    void Awake() {
        if (!PlayerPrefs.HasKey(GridUtility.DefaultGridSizeKey)) {
             PlayerPrefs.SetInt(GridUtility.DefaultGridSizeKey, 16);
        }
    }

    public override void Initalize() {
        base.Initalize();
        _inputField.text = PlayerPrefs.GetInt(GridUtility.DefaultGridSizeKey).ToString();
        gameObject.SetActive(false);
    }

    public void OnValueChanged(InputField inputField) {
        int value;
        string text = inputField.text;
        if (int.TryParse(text, out value)) {
            PlayerPrefs.SetInt(GridUtility.DefaultGridSizeKey, value);
        }
    }
}
