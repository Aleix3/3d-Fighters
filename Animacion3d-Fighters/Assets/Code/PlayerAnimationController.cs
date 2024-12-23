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
        if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.Space))
        {
            PlayAnimation("DodgeHighAttack");
        }

        else if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.J))
        {
            PlayAnimation("LowAttack");
        }

        else if (Input.GetKeyDown(KeyCode.J))
        {
            PlayAnimation("FastAttack");
        }

        else if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayAnimation("DodgeAttack");
        }
    }

    private void PlayAnimation(string animationTrigger)
    {
        animator.SetTrigger(animationTrigger);
    }
}