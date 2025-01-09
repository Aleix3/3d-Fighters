using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;

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
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.J))
        {
            PlayAnimation("SlowLowAttack");
        }

        else if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.Space))
        {
            PlayAnimation("DodgeHighAttack");
        }

        else if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.J))
        {
            PlayAnimation("LowAttack");
        }

        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.J))
        {
            PlayAnimation("SlowAttack");
        }

        else if (Input.GetKeyDown(KeyCode.J))
        {
            PlayAnimation("FastAttack");
        }

        else if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayAnimation("DodgeAttack");
        }

        else if (Input.GetKeyDown(KeyCode.N))
        {
            PlayAnimation("Win");
        }

        else if (Input.GetKeyDown(KeyCode.M))
        {
            PlayAnimation("Lose");
        }

        else if (Input.GetKey(KeyCode.A))
        {
            PlayAnimation("MoveLeft");
        }

        else if (Input.GetKey(KeyCode.D))
        {
            PlayAnimation("MoveRight");
        }
    }

    private void PlayAnimation(string animationTrigger)
    {
        animator.SetTrigger(animationTrigger);
    }
}