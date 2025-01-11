using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    public int health = 100; // Vida del jugador
    public GameObject opponent; // Referencia al oponente
    private bool isAttacking = false; // Para verificar si el ataque está activo

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
        HandleInput();

        if (health <= 0)
        {
            
            StartCoroutine(ShowGameOverCanvasWithDelay());
        }
    }

    private IEnumerator ShowGameOverCanvasWithDelay()
    {
        yield return new WaitForSeconds(5f);
        gameOverCanvas.SetActive(true);
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
        bool block = Input.GetButton(blockButton);           // Botón de bloqueo
        bool quickAttack = Input.GetButtonDown(quickAttackButton); // Botón de ataque rápido
        bool slowAttack = Input.GetButtonDown(slowAttackButton);   // Botón de ataque lento
        bool downInput = Input.GetAxis(verticalAxis) < -0.5f; // Stick abajo
        bool upInput = Input.GetAxis(verticalAxis) > 0.5f;    // Stick arriba

        // Movimiento
        if (moveHorizontal > 0)
        {
            PlayAnimation("MoveRight");
        }
        else if (moveHorizontal < 0)
        {
            PlayAnimation("MoveLeft");
        }
        else
        {
            PlayAnimation("Idle");
        }

        // Esquivar y Evadir
        if (downInput && Input.GetButtonDown(blockButton))
        {
            PlayAnimation("DodgeHighAttack");
        }
        else if (upInput && Input.GetButtonDown(blockButton))
        {
            PlayAnimation("DodgeAttack");
        }


        // Ataques
        else if (downInput && quickAttack)
        {
            PlayAnimation("LowAttack");
            TriggerAttack("LowAttack", lowAttackHitbox);
        }
        else if (downInput && slowAttack)
        {
            PlayAnimation("SlowLowAttack");
            TriggerAttack("SlowLowAttack", lowAttackHitbox);
        }
        else if (quickAttack)
        {
            PlayAnimation("FastAttack");
            TriggerAttack("FastAttack", fastAttackHitbox);
        }
        else if (slowAttack)
        {
            PlayAnimation("SlowAttack");
            TriggerAttack("SlowAttack", slowAttackHitbox);
        }
    }

    private void PlayAnimation(string animationTrigger)
    {
        animator.SetTrigger(animationTrigger);
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

    private void OnTriggerEnter(Collider other)
    {
        // Comprueba si se golpea al oponente
        if (isAttacking && other.gameObject == opponent)
        {
            // Quita toda la vida al oponente (solo para pruebas)
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
            PlayAnimation("Lose");
            Debug.Log($"{gameObject.name} ha sido derrotado.");
        }
    }

    private void ResetAttack()
    {
        isAttacking = false;

        // Desactivar todas las hitboxes
        if (slowAttackHitbox != null) slowAttackHitbox.enabled = false;
        if (fastAttackHitbox != null) fastAttackHitbox.enabled = false;
        if (lowAttackHitbox != null) lowAttackHitbox.enabled = false;
    }

    public void RestartGame()
    {
        // Obtener el nombre de la escena actual
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Reiniciar la escena cargándola de nuevo
        SceneManager.LoadScene(currentSceneName);
    }
}
