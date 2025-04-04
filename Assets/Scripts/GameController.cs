using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI goldAmountText;
    
    public int goldAmount;
    
    public static GameController instance;

    private GameController(){}

    private void Awake()
    {
       DontDestroyOnLoad(this);
       if (instance == null)
       {
           instance = this;
       }
       else
       {
           Destroy(gameObject);
       }

       RestorePlayerPrefs();
    }

    private void RestorePlayerPrefs()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            PlayerPrefs.DeleteAll();
            return;
        }
        
        if (PlayerPrefs.GetInt(PlayerPrefsConst.Gold) > 0)
        {
            goldAmount = PlayerPrefs.GetInt(PlayerPrefsConst.Gold);
            goldAmountText.text = "x " + goldAmount;
        }
    }

    public void AddCoins(int amount)
    {
        goldAmount += amount;
        goldAmountText.text = "x " + goldAmount;
        
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
    
}
