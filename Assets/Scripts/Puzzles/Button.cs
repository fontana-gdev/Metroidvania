using UnityEngine;

public class Button : MonoBehaviour
{

    [SerializeField] private Animator doorAnim;
    [SerializeField] private float pressRadius;
    
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        CheckIfButtonIsPressed();
    }

    private void CheckIfButtonIsPressed()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, pressRadius);
        if (hit && hit.CompareTag("Stone"))
        {
            OpenDoor();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, pressRadius);
    }

    private void OpenDoor()
    {
        animator.SetBool("pressed", true);
        doorAnim.SetBool("open", true);
    }

    private void CloseDoor()
    {
        animator.SetBool("pressed", false);
        doorAnim.SetBool("open", false);
    }
}