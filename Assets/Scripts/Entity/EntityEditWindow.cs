using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CustomTilemap;

public class EntityEditWindow : MonoBehaviour {
    [SerializeField] Transform _fieldPrefab = null;
    [SerializeField] Text _titleText = null;
    Transform _contentParent;
    ObjectPool<Transform> _fieldObjPool;
    List<Transform> _currentFields;
    
    void Start() {
        _contentParent = transform.GetChild(0);
        _fieldObjPool = new ObjectPool<Transform>(5, () => Instantiate(_fieldPrefab, _contentParent));
        _currentFields = new List<Transform>();
    }

    public void EnableWindow(EntityLayer.EntityObject selected) {
        _contentParent.gameObject.SetActive(true);

        _titleText.text = selected.EntityName;

        foreach (var fieldObj in _currentFields) {
            fieldObj.SetParent(null);
            _fieldObjPool.ReturnObject(fieldObj);
        }
        _currentFields.Clear();

        foreach (var field in selected.Fields) {
            AddField(selected, field.Key);
        }
    }

    void AddField(EntityLayer.EntityObject selected, string fieldName) {
        var fieldObj = _fieldObjPool.GetObject();
        var inputField = fieldObj.GetComponentInChildren<InputField>();
        
        fieldObj.GetComponentInChildren<Text>().text = fieldName;
        inputField.onValueChanged.RemoveAllListeners();
        inputField.onValueChanged.AddListener(delegate { OnInputFieldValueChanged(selected, fieldName, inputField.text); });    

        _currentFields.Add(fieldObj);
    }

    void OnInputFieldValueChanged(EntityLayer.EntityObject selected, string fieldName, string text) {
        selected.Fields[fieldName] = text;
    }
}
