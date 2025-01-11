using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
//using UnityEngine.SceneManager;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    public int health = 100; // Vida del jugador
    public GameObject opponent; // Referencia al oponente
    private bool isAttacking = false; // Para verificar si el ataque está activo
    private bool hasWon = false; // Para verificar si este jugador ha ganado

    public BoxCollider slowAttackHitbox;
    public BoxCollider fastAttackHitbox;
    public BoxCollider lowAttackHitbox;
    public GameObject gameOverCanvas;

    public int playerId = 1; // Identificador del jugador (1 o 2)

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (health <= 0 && !hasWon)
        {
            // Si este jugador ha sido derrotado
            StartCoroutine(ShowGameOverCanvasWithDelay());
        }

        CheckForVictory();
        if (!hasWon) HandleInput(); // Solo permite entrada si el jugador no ha ganado
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
                // El oponente ha sido derrotado
                hasWon = true;
                animator.SetTrigger("Win"); // Activa la animación de victoria
                Debug.Log($"{gameObject.name} ha ganado.");
            }
        }
    }

    private void HandleInput()
    {
        // Prefijo de entrada según el jugador
        string horizontalAxis = $"Horizontal";
        string verticalAxis = $"Vertical";
        string blockButton = $"Fire2";
        string quickAttackButton = $"Fire3";
        string slowAttackButton = $"Fire1";

        float moveHorizontal = Input.GetAxis(horizontalAxis); // Movimiento horizontal
        float moveVertical = Input.GetAxis(verticalAxis);     // Movimiento vertical
        bool block = Input.GetButton(blockButton);           // Botón de bloqueo
        bool quickAttack = Input.GetButtonDown(quickAttackButton); // Botón de ataque rápido
        bool slowAttack = Input.GetButtonDown(slowAttackButton);   // Botón de ataque lento

        // Movimiento horizontal
        if (moveHorizontal > 0)
        {
            animator.SetBool("MoveRight", true);
            animator.SetBool("MoveLeft", false);
        }
        else if (moveHorizontal < 0)
        {
            animator.SetBool("MoveLeft", true);
            animator.SetBool("MoveRight", false);
        }
        else
        {
            animator.SetBool("MoveRight", false);
            animator.SetBool("MoveLeft", false);
        }

        // Esquivar (movimiento vertical combinado con bloqueo)
        if (moveVertical < -0.5f && Input.GetButtonDown(blockButton))
        {
            animator.SetTrigger("DodgeLow");
        }
        else if (moveVertical > 0.5f && Input.GetButtonDown(blockButton))
        {
            animator.SetTrigger("DodgeHigh");
        }

        // Ataques
        if (moveVertical < -0.5f && quickAttack)
        {
            animator.SetTrigger("LowAttack");
            TriggerAttack("LowAttack", lowAttackHitbox);
        }
        else if (moveVertical < -0.5f && slowAttack)
        {
            animator.SetTrigger("SlowLowAttack");
            TriggerAttack("SlowLowAttack", lowAttackHitbox);
        }
        else if (quickAttack)
        {
            animator.SetTrigger("FastAttack");
            TriggerAttack("FastAttack", fastAttackHitbox);
        }
        else if (slowAttack)
        {
            animator.SetTrigger("SlowAttack");
            TriggerAttack("SlowAttack", slowAttackHitbox);
        }
    }

    private void TriggerAttack(string attackTrigger, BoxCollider attackHitbox)
    {
        if (attackHitbox == null) return;

        isAttacking = true;

        // Activar la hitbox correspondiente
        attackHitbox.enabled = true;

        // Desactivar la hitbox al finalizar el ataque
        Invoke(nameof(ResetAttack), 0.5f); // Ajusta el tiempo según la duración de la animación
    }

    private void ResetAttack()
    {
        isAttacking = false;

        // Desactivar todas las hitboxes
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
                opponentController.TakeDamage(100); // Daño total para testear
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} recibió {damage} de daño. Vida restante: {health}");

        if (health <= 0)
        {
            animator.SetTrigger("Lose");
            Debug.Log($"{gameObject.name} ha sido derrotado.");
        }
    }

    public void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
