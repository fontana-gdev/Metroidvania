using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Range(1, 10)] [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;
    [SerializeField] private Image[] heartsSprites;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    public int CurrentHealth => currentHealth;
    
    private void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        for (int i = 0; i < heartsSprites.Length; i++)
        {
            if (i < maxHealth)
            {
                heartsSprites[i].enabled = true;
            }
            else
            {
                heartsSprites[i].enabled = false;
            }

            if (i > currentHealth - 1)
            {
                heartsSprites[i].sprite = emptyHeart;
            }
            else
            {
                heartsSprites[i].sprite = fullHeart;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }
}