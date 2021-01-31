using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    #region Static Instance Handling
    private static ScoreManager instance;
    public static ScoreManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.FindObjectOfType<ScoreManager>();
            if (instance == null)
                Debug.LogError("Could not find ScoreManager");

            return instance;
        }
    }
    void OnDisable()
    {
        instance = null;
    }
    #endregion


    [SerializeField]
    private TMP_Text TimeText;
    [SerializeField]
    private Image CrowdAngerMeter;
    [SerializeField]
    private GameObject GameOverScreen, ContinueButton, TopText2;
    [SerializeField]
    private TMP_Text TopText, SurvivalText;

    private bool paused, gameOver;
    private float gameStartTime;

    private void Awake() {
        GameOverScreen.SetActive(false);
        paused = gameOver = false;
        Time.timeScale = 1f;
        gameStartTime = Time.time;
    }

    private void Update()
    {
        float s = Time.time - gameStartTime;
        int minutes = Mathf.FloorToInt(s / 60f);
        int seconds = Mathf.FloorToInt(s % 60f);
        TimeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePauseGame();
    }

    public void TogglePauseGame(bool gameOver = false)
    {
        Debug.Log("Pause Toggle");
        if (!paused)
        {
            //inputFieldFocusKeeper.enabled = false;
            Time.timeScale = 0;
            SurvivalText.text = "You've Survived for:\n" + TimeText.text;
            GameOverScreen.SetActive(true);
            paused = true;
        }
        else
        {
            //inputFieldFocusKeeper.enabled = true;
            Time.timeScale = 1f;
            GameOverScreen.SetActive(false);
            paused = false;
        }
    }

    private void GameOver()
    {
        gameOver = true;
        ContinueButton.SetActive(false);
        TopText.text = "Game Over";
        TopText2.SetActive(true);
        SurvivalText.text = "You Survived for:\n" + TimeText.text;
        TogglePauseGame();
    }

    public void IncreaseCrowdAnger(float byThisMuch)
    {
        if (!gameOver)
        {
            CrowdAngerMeter.fillAmount += byThisMuch;
            if (CrowdAngerMeter.fillAmount >= 1f)
            {
                GameOver();
            }
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restart Game");
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
