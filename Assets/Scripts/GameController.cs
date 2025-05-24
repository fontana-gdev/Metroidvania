using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int goldAmount;

    public static GameController instance;

    private GameController() {}

    private void Awake()
    {
        // Time.timeScale = 1;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
        {
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(this);
        }

    }

    private void Start()
    {
        RestorePlayerPrefs();
    }

    private void RestorePlayerPrefs()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            PlayerPrefs.DeleteAll();
            return;
        }

        if (PlayerPrefs.GetInt(PlayerPrefsConst.Gold) > 0)
        {
            goldAmount = PlayerPrefs.GetInt(PlayerPrefsConst.Gold);
            Player.instance.GoldText.text = "x " + goldAmount;
        }
    }

    public void AddCoins(int amount)
    {
        goldAmount += amount;
        Player.instance.GoldText.text = "x " + goldAmount;

        PlayerPrefs.SetInt(PlayerPrefsConst.Gold, goldAmount);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowGameOverScreen()
    {
        // Time.timeScale = 0;
        Player.instance.GameOverPanel.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}