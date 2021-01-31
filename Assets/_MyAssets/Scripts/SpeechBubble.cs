using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeechBubble : MonoBehaviour
{
    [SerializeField]
    private TMP_Text bubbleText;
    [SerializeField]
    private RectTransform bubbleBody;
    [SerializeField]
    private int maxCharCountOnLine = 30;
    [SerializeField]
    [Range(0f, 1f)]
    private float minXNormalizedEdgeOfBody, maxXNormalizedEdgeOfBody;

    public void SetText(string text)
    {
        text = SplitTextByCharacterCountAtWordBreak(text);
        bubbleText.text = text;
        StartCoroutine(AdjustPositionForScreenEdges());
    }

    private IEnumerator AdjustPositionForScreenEdges()
    {
        yield return null;

        Rect screenRect = new Rect(0f, 0f, Screen.width, Screen.height);

        Vector3[] objectCorners = new Vector3[4];
        bubbleBody.GetWorldCorners(objectCorners);

        //have to do this gradual movement thing because setting the position directly to 0 + (width / 2) for some reason doesn't actually put the left edge at 0 depending on resolution

        //keep moving right until left edge is fully visible
        while (!screenRect.Contains(objectCorners[0]))
        {
            bubbleBody.position += new Vector3(0.01f, 0f, 0f);
            bubbleBody.GetWorldCorners(objectCorners);
        }

        //keep moving left until right edge is fully visible
        while (!screenRect.Contains(objectCorners[3]))
        {
            bubbleBody.position -= new Vector3(0.01f, 0f, 0f);
            bubbleBody.GetWorldCorners(objectCorners);
        }
    }

    private string SplitTextByCharacterCountAtWordBreak(string text)
    {
        string[] splitText = text.Split();
        string modifiedText = "";
        int charCount = 0;
        for (int i = 0; i < splitText.Length; i++)
        {
            modifiedText += splitText[i];
            charCount += splitText[i].Length;
            if (charCount >= maxCharCountOnLine)
            {
                modifiedText += "\n";
                charCount = 0;
            }
            else
                modifiedText += " ";
        }
        return modifiedText;
    }
}
