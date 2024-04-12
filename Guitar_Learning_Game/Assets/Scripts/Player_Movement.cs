using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    public float movespeed; 
    float speedx, speedy;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
void Update()
{
    speedx = Input.GetAxisRaw("Horizontal") * movespeed;
    speedy = Input.GetAxisRaw("Vertical") * movespeed;

    if (speedx != 0 || speedy != 0)
    {
        // If there's input, move the player and set the animator's speed.
        rb.velocity = new Vector2(speedx, speedy);
        animator.SetFloat("Speed", Mathf.Abs(movespeed));
    }
    else
    {
        // If there's no input, stop the player and set the animator's speed to 0.
        rb.velocity = Vector2.zero;
        animator.SetFloat("Speed", 0);
    }
}
}
