using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class TerminalHandler : MonoBehaviour
{
    #region Static Instance Handling
    private static TerminalHandler instance;
    public static TerminalHandler Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<TerminalHandler>();
            if (instance == null)
                Debug.LogError("Could not find TerminalHandler");

            return instance;
        }
    }
    void OnDisable()
    {
        instance = null;
    }
    #endregion


    public TMP_InputField TerminalInputField;
    public TMP_Text TerminalInputStartChar;
    public GameObject TerminalMessageLinePrefab;
    public ScrollRect TerminalScrollRect;
    public int TerminalFontSize;
    public bool CaseSensitiveCommands = false;

    private static Dictionary<string, Func<string[], string>> Commands = new Dictionary<string, Func<string[], string>>
    {
        {"cls", ClearScreen},
        {"test", TestCommand}
    };

    private void Awake() {
        TerminalInputField.pointSize = TerminalFontSize;
        TerminalInputStartChar.fontSize = TerminalFontSize;
        TerminalInputField.textComponent.margin = new Vector4(TerminalFontSize / 1.714285f, 0f, 0f, 0f); //quick and dirty way to update the margin to scale more or less right
    }


    private void Update() {
        ScrollToBottom();
    }

    public void HandleInput(string textVal)
    {
        InsertTerminalLine(textVal);

        string commandReturn = ParseCommand(textVal);
        if (!string.IsNullOrEmpty(commandReturn))
            InsertTerminalLine(commandReturn, false);
    }

    private void InsertTerminalLine(string textVal, bool includeLineStartChar = true)
    {
        TMP_Text newText = Instantiate(TerminalMessageLinePrefab, TerminalScrollRect.transform).GetComponent<TMP_Text>();
        newText.fontSize = TerminalFontSize;
        newText.text = (includeLineStartChar ? ">" : "")+textVal;
        TerminalInputField.transform.SetAsLastSibling();
        ScrollToBottom();
    }
    

    private string ParseCommand(string val)
    {
        val = val.Trim();
        if (!CaseSensitiveCommands)
            val = val.ToLower();

        List<string> splitVal = val.Split().ToList(); //split value by whitespace
        string commandString = splitVal[0]; //get first value. We assume it's a command
        splitVal.RemoveAt(0); //remove the first value. We assume the rest are arguments

        if (!string.IsNullOrEmpty(commandString))
        {
            Func<string[], string> command = (Commands.ContainsKey(commandString) ? Commands[commandString] : null);
            if (command != null)
                return command.Invoke(splitVal.ToArray());
        }
        return null;
    }

    public void ScrollToBottom()
    {
        TerminalScrollRect.normalizedPosition = new Vector2(0, 0);
    }






    private static string ClearScreen(string[] args)
    {
        foreach (Transform t in Instance.TerminalScrollRect.transform)
        {
            if (t.tag != "TerminalInput")
                Destroy(t.gameObject);
        }

        return null;
    }
    private static string TestCommand(string[] args)
    {
        if(args.Length > 0)
            return "Test command ran with arguments: " + string.Join(", ", args);
        else
            return "Test command ran";
    }
}
