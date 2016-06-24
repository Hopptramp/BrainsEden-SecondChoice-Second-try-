using UnityEngine;
using System.Collections;

public class Player_Movement : MonoBehaviour
{

    Rigidbody rb;
    [SerializeField] private int moveSpeed = 50;
    private bool grounded = false;
    [SerializeField] private bool DEBUG_MULTIJUMP;
    private int jumpCount = 0;
    [SerializeField] private int jumpMax = 0;
    public int jumpForce = 100;
    private float moveX, moveY, moveZ;
    private bool justJumped = false;
    private bool nullifyMovement = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {       
        justJumped = false;
        if (!nullifyMovement)        
            Movement();
        else
            rb.velocity = new Vector3(0, rb.velocity.y, 0);

        grounded = false;
    }

    void Movement()
    {
        moveX = Input.GetButton("Horizontal") ? moveSpeed * Input.GetAxis("Horizontal") : 0;
        moveY = rb.velocity.y;
        moveZ = Input.GetButton("Vertical") ? moveSpeed * Input.GetAxis("Vertical") : 0;
        if (DEBUG_MULTIJUMP)
        {
            if (Input.GetButtonDown("Jump") && jumpCount > 0)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
                jumpCount--;
                justJumped = true;
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump") && grounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
                justJumped = true;             
            }
        }
        rb.velocity = new Vector3(moveX, moveY, moveZ);

    }

    public void SetGrounded(bool _val)
    {
        grounded = _val;
        if (DEBUG_MULTIJUMP && grounded && !justJumped)
        {            
            jumpCount = jumpMax;
        }
    }

    public void DisableInput(bool _set = true)
    {
        nullifyMovement = _set;
    }
}
