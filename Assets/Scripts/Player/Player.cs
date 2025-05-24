using System.Collections;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius;
    [SerializeField] private LayerMask enemyLayer;
    [Range(0, 3)]
    [SerializeField] private float recoverySeconds;

    [Header("UI")] 
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private GameObject gameOverPanel;

    private Rigidbody2D rig;
    private AudioPlayer audioPlayer;
    private Animator anim;
    private bool isJumping;
    private bool isAttacking;
    private bool canDoubleJump;
    private bool isDead;
    private float iFramesCountdown;
    private Health playerHealth;
    
    public static Player instance;

    public TextMeshProUGUI GoldText => goldText;
    public GameObject GameOverPanel => gameOverPanel;
    
    private Player(){}

    private void Awake()
    {
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

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        audioPlayer = GetComponent<AudioPlayer>();
        playerHealth = GetComponent<Health>();
    }

    private void Update()
    {
        if (isDead) return;
        
        Jump();
        Attack();
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            rig.velocity = Vector2.zero;
            return;
        }
        
        Move();
        
        if (iFramesCountdown > 0)
        {
            iFramesCountdown -= Time.deltaTime;
        }
    }
    
    private void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (!isJumping)
            {
                rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                anim.SetInteger(PlayerAnimations.AnimTransition, (int)PlayerAnimations.States.Jump);
                isJumping = true;
                canDoubleJump = true;
                audioPlayer.PlaySFX(audioPlayer.jumpSfx);
            }
            else if (canDoubleJump)
            {
                rig.AddForce(Vector2.up * (jumpForce / 2), ForceMode2D.Impulse);
                // anim.SetInteger(PlayerAnimations.AnimTransition, (int)PlayerAnimations.States.DoubleJump);
                canDoubleJump = false;
                audioPlayer.PlaySFX(audioPlayer.jumpSfx);
            }
        }
    }

    private void Move()
    {
        var movement = Input.GetAxis("Horizontal");
        rig.velocity = new Vector2(movement * moveSpeed, rig.velocity.y);

        if (movement == 0 && !isJumping && !isAttacking)
        {
            anim.SetInteger(PlayerAnimations.AnimTransition, (int)PlayerAnimations.States.Idle);
        }
        if (movement != 0 && !isJumping && !isAttacking)
        {
            anim.SetInteger(PlayerAnimations.AnimTransition, (int)PlayerAnimations.States.Walk);
        }

        if (movement > 0)
        {
            transform.eulerAngles = new Vector2(0, 0);
        }
        else if (movement < 0)
        {
            transform.eulerAngles = new Vector2(0, 180);
        }
    }

    private void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (isAttacking) return;
            
            isAttacking = true;
            anim.SetInteger(PlayerAnimations.AnimTransition, (int)PlayerAnimations.States.Attack);
            audioPlayer.PlaySFX(audioPlayer.attackSfx);
            
            Collider2D hit = Physics2D.OverlapCircle(attackPoint.position, attackRadius, enemyLayer);
            if (hit)
            {
                if (hit.GetComponent<Slime>())
                {
                    hit.GetComponent<Slime>().TakeDamage(1);    
                }
                else if (hit.GetComponent<Goblin>())
                {
                    hit.GetComponent<Goblin>().TakeDamage(1);
                }
                
            }

            StartCoroutine(EndAttack());
        }
    }

    private IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(0.4f);
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (iFramesCountdown > 0 || isDead) return;
        
        playerHealth.TakeDamage(damage);
        
        if (playerHealth.CurrentHealth <= 0)
        {
            anim.SetTrigger(PlayerAnimations.TriggerDiedAnim);
            isDead = true;
            moveSpeed = 0;
            GameController.instance.ShowGameOverScreen();
        }
        else
        {
            anim.SetTrigger(PlayerAnimations.TriggerHitAnim);
            iFramesCountdown = recoverySeconds;
        }
       
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 3)
        {
            isJumping = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            GameController.instance.AddCoins(1);
            other.GetComponent<Animator>().SetTrigger("pick");
            Destroy(other.gameObject, 0.5f);
            audioPlayer.PlaySFX(audioPlayer.coinPickupSfx);
        }

        if (other.CompareTag("Portal"))
        {
            GameController.instance.NextLevel();
        }

        if (other.CompareTag("BottomHole"))
        {
            PlayerStartPosition.instance.RepositionPlayer();
            TakeDamage(2);
        }
    }
}