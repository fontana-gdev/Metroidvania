using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField] private Animator doorAnim;
    
    private Animator animator;
    private bool on;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        on = !on;
        animator.SetBool("on", on);
        doorAnim.SetBool("open", on);
    }
}
