using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputFieldFocusKeeper : MonoBehaviour
{
    private TMP_InputField inputField;
    
    private void Awake() {
        inputField = GetComponent<TMP_InputField>();
        ReFocus();
    }

    private void OnEnable() {
        inputField.onValueChanged.AddListener(OnValueChanged);
        inputField.onDeselect.AddListener(OnDeselect);
    }
    private void OnDisable() {
        inputField.onValueChanged.RemoveListener(OnValueChanged);
        inputField.onDeselect.RemoveListener(OnDeselect);
    }

    private void OnValueChanged(string newValue)
    {
        ReSize();
    }
    private void OnDeselect(string value)
    {
        ReFocus();
    }



    private void ReFocus()
    {
        inputField.Select();
        inputField.ActivateInputField();
        inputField.caretPosition = inputField.text.Length;
        //Debug.Log("InputFieldFocusKeeper: re-focused");
    }

    private void ReSize()
    {
        inputField.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CustomSubmit();
        }
    }

    private void CustomSubmit()
    {
        TerminalHandler.Instance.HandleInput(inputField.text.TrimEnd());
        inputField.text = "";
        ReFocus();
        ReSize();
    }
}
