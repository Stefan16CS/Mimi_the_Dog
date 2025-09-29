using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private float moving;
    
    void Start()
    {
        moving = Mimi.currentSpeed;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moving > 0 && rb.linearVelocityY == 0)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }

        if (rb.linearVelocityY == 0)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", false);
        }

        if (rb.linearVelocityY > 0)
        {
            anim.SetBool("isJumping", true);
        }

        if (rb.linearVelocityY < 0)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", true);
        }
    }
}
