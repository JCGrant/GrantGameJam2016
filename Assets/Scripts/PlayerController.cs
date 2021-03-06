using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Transform groundCheck;

    public float maxSpeed;
    public float moveForce;
    public float jumpForce;
    public float thrustForce;

    public int maxThrustPoints;
    public int thrustPoints;

    public bool hasWon;


    private Rigidbody2D rb;
    private int groundLayerMask;
    private GameObject thrustFlame;
    private Vector3 playerScale;

    private bool jumping;
    private bool grounded;
    private bool thrusting;
    private bool facingRight;


    void Start() {
        rb = GetComponent<Rigidbody2D>();
        groundLayerMask = 1 << LayerMask.NameToLayer("Ground");
        jumping = false;
        grounded = false;
        thrustFlame = transform.Find("Thrust Flame").gameObject;
        thrustPoints = maxThrustPoints;
        hasWon = false;
        playerScale = transform.localScale;
    }

    void Update() {
        grounded = Physics2D.Linecast(
            transform.position,
            groundCheck.position,
            groundLayerMask
        );

        if (Input.GetButtonDown("Jump") && grounded) {
            jumping = true;
        }

        thrusting = Input.GetKey(KeyCode.LeftShift) && thrustPoints > 0;
        thrustFlame.SetActive(thrusting);

        Vector3 cameraPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        facingRight = cameraPosition.x - transform.position.x >= 0;
        int flipScale = facingRight ? 1 : -1;
        Vector3 newScale = playerScale;
        newScale.x = flipScale * playerScale.x;
        transform.localScale = newScale;
    }

    void FixedUpdate() {
        float h = Input.GetAxis("Horizontal");

        if (Mathf.Abs(rb.velocity.x) < maxSpeed) {
            rb.AddForce(Vector2.right * h * moveForce);
        }

        if (jumping) {
            rb.AddForce(Vector2.up * jumpForce);
            jumping = false;
        }

        if (thrusting) {
            Vector2 facingDirection = facingRight ? Vector2.right : Vector2.left;
            rb.AddForce((facingDirection + Vector2.up) * thrustForce);
            thrustPoints -= 1;
        }
    }
        
    void OnTriggerEnter2D(Collider2D other) {
        switch (other.gameObject.name) {
        case "Super Boost":
            other.gameObject.SetActive(false);
            thrustPoints += 50;
            break;
        case "Battery":
            other.gameObject.SetActive(false);
            hasWon = true;
            break;
        }
    }
}