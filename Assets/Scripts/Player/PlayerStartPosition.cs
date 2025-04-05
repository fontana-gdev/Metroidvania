using UnityEngine;

public class PlayerStartPosition : MonoBehaviour
{
    public static PlayerStartPosition instance;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        RepositionPlayer();
    }

    public void RepositionPlayer()
    {
        Player.instance.transform.position = transform.position;
    }

}
