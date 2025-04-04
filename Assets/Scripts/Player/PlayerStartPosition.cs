using UnityEngine;

public class PlayerStartPosition : MonoBehaviour
{
    
    private Transform playerTransform;
    
    public static PlayerStartPosition instance;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        if (playerTransform != null)
        {
            RepositionPlayer();
        }
    }

    public void RepositionPlayer()
    {
        playerTransform.position = transform.position;
    }

}
