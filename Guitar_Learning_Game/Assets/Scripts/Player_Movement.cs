using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator animator;
    private float moveSpeed = 10f; // Adjust this value to change the player's movement speed
    float speedx, speedy;
    private Rigidbody2D rb;
    private bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {    
        if(canMove) {
            // Only process input if the player is allowed to move.
            speedx = Input.GetAxisRaw("Horizontal") * moveSpeed;
            speedy = Input.GetAxisRaw("Vertical") * moveSpeed;
            
            // Any movement? (Avoid redundant setting of rb.velocity if not necessary)
            if (speedx != 0 || speedy != 0)
            {
                rb.velocity = new Vector2(speedx, speedy);
                animator.SetFloat("Speed", Mathf.Abs(speedx + speedy));
            }
            else
            {
                rb.velocity = Vector2.zero;
                animator.SetFloat("Speed", 0);
            }
        }
        else
        {
            // If the player cannot move, ensure the velocity and animation speed are set to zero.
            if (rb.velocity != Vector2.zero)
            {
                rb.velocity = Vector2.zero;
                animator.SetFloat("Speed", 0);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        // Check if the player has collided with an object tagged as "Environment"
        if (other.gameObject.CompareTag("Environment")) {
            canMove = true;
            Debug.Log("player can move");
        }
        if(other.gameObject.CompareTag("Border")) {
        // Collided with the border, prevent movement
        canMove = false;
        }
    }
}
