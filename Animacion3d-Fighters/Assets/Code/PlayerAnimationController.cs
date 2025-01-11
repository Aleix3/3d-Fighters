using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    public int health = 100;
    public GameObject opponent;
    private bool isAttacking = false;
    private bool hasWon = false;

    public BoxCollider slowAttackHitbox;
    public BoxCollider fastAttackHitbox;
    public BoxCollider lowAttackHitbox;
    public GameObject gameOverCanvas;

    public int playerId = 1;

    public AudioSource audioSource;

    public AudioClip attackSound;
    public AudioClip slowAttackSound;
    public AudioClip lowAttackSound;
    public AudioClip slowLowAttackSound;
    public AudioClip walkingSound;
    public AudioClip jumpSound;
    public AudioClip crouchSound;
    public AudioClip winSound;
    public AudioClip loseSound;

    private bool isWalking = false;
    private bool isCrouching = false;
    private bool isJumping = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (health <= 0 && !hasWon)
        {
            StartCoroutine(ShowGameOverCanvasWithDelay());
        }

        CheckForVictory();
        if (!hasWon) HandleInput();
    }

    private IEnumerator ShowGameOverCanvasWithDelay()
    {
        yield return new WaitForSeconds(5f);
        gameOverCanvas.SetActive(true);
    }

    private void CheckForVictory()
    {
        if (opponent != null)
        {
            PlayerAnimationController opponentController = opponent.GetComponent<PlayerAnimationController>();

            if (opponentController != null && opponentController.health <= 0 && !hasWon)
            {
                hasWon = true;
                animator.SetTrigger("Win");
                PlaySound(winSound);
                Debug.Log($"{gameObject.name} ha ganado.");
            }
        }
    }

    private void HandleInput()
    {
        string horizontalAxis = $"Horizontal";
        string verticalAxis = $"Vertical";
        string blockButton = $"Fire2";
        string quickAttackButton = $"Fire3";
        string slowAttackButton = $"Fire1";

        float moveHorizontal = Input.GetAxis(horizontalAxis);
        float moveVertical = Input.GetAxis(verticalAxis);
        bool block = Input.GetButton(blockButton);
        bool quickAttack = Input.GetButtonDown(quickAttackButton);
        bool slowAttack = Input.GetButtonDown(slowAttackButton);
        bool jump = Input.GetButtonDown(blockButton);

        if (moveHorizontal != 0)
        {
            animator.SetBool("MoveRight", moveHorizontal > 0);
            animator.SetBool("MoveLeft", moveHorizontal < 0);

            if (!isWalking)
            {
                PlaySound(walkingSound);
                isWalking = true;
            }
        }
        else
        {
            animator.SetBool("MoveRight", false);
            animator.SetBool("MoveLeft", false);
            isWalking = false;
        }

        if (moveVertical < -0.5f && block && !isCrouching)
        {
            isCrouching = true;
            PlaySound(crouchSound);
            animator.SetTrigger("DodgeHighAttack");
            StartCoroutine(ResetCrouch());
        }

        else if (jump && !isJumping)
        {
            isJumping = true;
            PlaySound(jumpSound);
            animator.SetTrigger("DodgeAttack");
            StartCoroutine(ResetJump());
        }

        if (moveVertical < -0.5f && quickAttack)
        {
            animator.SetTrigger("LowAttack");
            TriggerAttack("LowAttack", lowAttackHitbox, lowAttackSound);
        }
        else if (moveVertical < -0.5f && slowAttack)
        {
            animator.SetTrigger("SlowLowAttack");
            TriggerAttack("SlowLowAttack", lowAttackHitbox, slowLowAttackSound);
        }
        else if (quickAttack)
        {
            animator.SetTrigger("FastAttack");
            TriggerAttack("FastAttack", fastAttackHitbox, attackSound);
        }
        else if (slowAttack)
        {
            animator.SetTrigger("SlowAttack");
            TriggerAttack("SlowAttack", slowAttackHitbox, slowAttackSound);
        }
    }

    private IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(1f);
        isJumping = false;
    }

    private IEnumerator ResetCrouch()
    {
        yield return new WaitForSeconds(1f);
        isCrouching = false;
    }

    private void TriggerAttack(string attackTrigger, BoxCollider attackHitbox, AudioClip sound)
    {
        if (attackHitbox == null) return;

        isAttacking = true;
        PlaySound(sound);
        attackHitbox.enabled = true;
        Invoke(nameof(ResetAttack), 0.5f);
    }

    private void ResetAttack()
    {
        isAttacking = false;

        if (slowAttackHitbox != null) slowAttackHitbox.enabled = false;
        if (fastAttackHitbox != null) fastAttackHitbox.enabled = false;
        if (lowAttackHitbox != null) lowAttackHitbox.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.gameObject == opponent)
        {
            PlayerAnimationController opponentController = opponent.GetComponent<PlayerAnimationController>();
            if (opponentController != null)
            {
                opponentController.TakeDamage(100);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} recibi� {damage} de da�o. Vida restante: {health}");

        if (health <= 0)
        {
            animator.SetTrigger("Lose");
            PlaySound(loseSound);
            Debug.Log($"{gameObject.name} ha sido derrotado.");
        }
    }

    public void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}