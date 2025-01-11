using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;
    public int health = 100; // Vida del jugador
    public GameObject opponent; // Referencia al oponente
    private bool isAttacking = false; // Para verificar si el ataque est� activo

    public BoxCollider slowAttackHitbox;
    public BoxCollider fastAttackHitbox;
    public BoxCollider lowAttackHitbox;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); // Stick izquierdo para moverse
        bool blockButton = Input.GetButton("Fire2");        // Bot�n Fire2 para bloquear
        bool quickAttackButton = Input.GetButtonDown("Fire3"); // Bot�n Fire1 para ataque r�pido
        bool slowAttackButton = Input.GetButtonDown("Fire1");  // Bot�n Fire3 para ataque lento
        bool downInput = Input.GetAxis("Vertical") < -0.5f; // Stick abajo
        bool upInput = Input.GetAxis("Vertical") > 0.5f;    // Stick arriba

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
        if (downInput && blockButton)
        {
            PlayAnimation("DodgeHighAttack");
        }
        else if (upInput && blockButton)
        {
            PlayAnimation("DodgeAttack");
        }

        // Ataques
        else if (downInput && quickAttackButton)
        {
            PlayAnimation("LowAttack");
            TriggerAttack("LowAttack", lowAttackHitbox);
        }
        else if (downInput && slowAttackButton)
        {
            PlayAnimation("SlowLowAttack");
            TriggerAttack("SlowLowAttack", lowAttackHitbox);
        }
        else if (quickAttackButton)
        {
            PlayAnimation("FastAttack");
            TriggerAttack("FastAttack", fastAttackHitbox);
        }
        else if (slowAttackButton)
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
        Invoke(nameof(ResetAttack), 0.5f); // Ajusta el tiempo seg�n la duraci�n de la animaci�n
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
                opponentController.TakeDamage(100); // Da�o total para testear
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} recibi� {damage} de da�o. Vida restante: {health}");

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
}
