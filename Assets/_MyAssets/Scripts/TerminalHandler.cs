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
        {"test", TestCommand},
        {"find", FindItemByName},
        {"search", FindItemByName},
        {"help", HelpCommand}
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
            else
                return "Invalid command. Try 'help'";
        }
        return null;
    }

    public void ScrollToBottom()
    {
        TerminalScrollRect.normalizedPosition = new Vector2(0, 0);
    }





    private static string HelpCommand(string[] args)
    {
        if (args.Length == 0)
        {
            string helpString = "If the first word matches a command, it is called with the other words as arguments.\n" +
            "LIST OF COMMANDS:\n" +
            "help: this\n" +
            "cls: clear screen\n" +
            "find/search: display the drawer number where the given item name can be found. (arguments: item name)";

            return helpString;
        }
        else if (args[0] == "help")
            return "help: help?";
        else if (args[0] == "cls")
            return "cls: clears the screen";
        else if (args[0] == "find" || args[0] == "search")
            return "find/search: display the drawer number where the given item name can be found. (arguments: item name)";
        else
            return "help ERROR: Invalid arguments.";
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
    private static string FindItemByName(string[] args)
    {
        if (args.Length == 0)
            return "find ERROR: MissingParameterException";
        else
        {
            
            string combinedArgs = string.Join(" ", args);
            Item item = ItemHandler.Instance.FindFromActiveItemsByName(combinedArgs);
            if (item == null)
                return "Item: \""+combinedArgs+"\" not found";
            else
            {
                return "Item: \"" + combinedArgs + "\" can be found in drawer #" + item.ContainingDrawer.DrawerID;
            }
            
        }
    }
}
