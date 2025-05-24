using UnityEngine;

public class Slime : MonoBehaviour
{
    private const string AnimHit = "hit";
    private const string AnimDied = "died";
    
    [SerializeField] private int maxHealth;
    [SerializeField] private float moveSpeed;
    [SerializeField] private int damage = 1;
    [SerializeField] private float wallCollisionPoint;
    [SerializeField] private float wallCollisionRadius;
    [Range(0, 3)]
    [SerializeField] private float recoverySeconds;

    private Rigidbody2D rig;
    private Animator anim;
    private int currentHealth;
    private float iFramesCountdown;
    private bool isDead;

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        
        if (iFramesCountdown > 0)
        {
            iFramesCountdown -= Time.deltaTime;
            rig.velocity = Vector2.zero;
            return;
        }
        
        rig.velocity = new Vector2(moveSpeed, rig.velocity.y);
        transform.eulerAngles = new Vector2(0, moveSpeed > 0 ? 180 : 0);
        CheckWallCollision();
    }

    private void CheckWallCollision()
    {
        var colliderPositionX = moveSpeed > 0 ? wallCollisionPoint : -wallCollisionPoint;
        var hitOther =
            Physics2D.OverlapCircle(transform.position + new Vector3(colliderPositionX, 0), wallCollisionRadius);
        if (hitOther && hitOther.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            moveSpeed = -moveSpeed;
        }
    }

    public void TakeDamage(int damageTaken)
    {
        if (iFramesCountdown > 0) return;
        
        currentHealth -= damageTaken;
        if (currentHealth <= 0)
        {
            isDead = true;
            moveSpeed = 0;
            rig.velocity = Vector2.zero;
            anim.SetTrigger(AnimDied);
            Destroy(gameObject, 1f);
        }
        else
        {
            anim.SetTrigger(AnimHit);
            iFramesCountdown = recoverySeconds;
        }
    }

    private void OnDrawGizmosSelected()
    {
        var colliderPositionX = moveSpeed > 0 ? wallCollisionPoint : -wallCollisionPoint;
        Gizmos.DrawWireSphere(transform.position + new Vector3(colliderPositionX, 0), wallCollisionRadius);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!isDead && other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>().TakeDamage(damage);
        }
    }
}