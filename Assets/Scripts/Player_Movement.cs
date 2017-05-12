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
    private bool nullifyInput = false;
    private bool frozen = false;
    private float storeY = 0;
    private int? hzInput = null;
    private int? vtInput = null;
    private bool jump;
    public bool useKeyboard;
    private Animator anim;
    GameObject cam;
    public AudioClip audioCLip;
    public AudioSource audioSrc;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        audioSrc = GetComponent<AudioSource>();
    }

    void Start()
    {
        cam = GameManager.instance.mainCamera;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {       
        justJumped = false;
        if (useKeyboard)
            KeyboardInput();
        if (!frozen)
        {
            anim.speed = 1;
            if (!nullifyInput)
                Movement();
            else
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
        else
        {
            anim.speed = 0;
            rb.velocity = Vector3.zero;
            //freez animations here!!
        }

        hzInput = null;
        vtInput = null;
        grounded = false;
    }

    public void HorizontalInput(int _val)
    {
        hzInput = _val;
    }

    public void SetJump(bool b) { jump = b; }

    public void VerticalInput(int _val)
    {
        vtInput = _val;
    }

    void KeyboardInput()
    {
        if (Input.GetKey(KeyCode.A))
            HorizontalInput(-1);
        if (Input.GetKey(KeyCode.D))
            HorizontalInput(1);
        if (Input.GetKey(KeyCode.S))
            VerticalInput(-1);
        if (Input.GetKey(KeyCode.W))
            VerticalInput(1);
        if (Input.GetKeyDown(KeyCode.Space))
            SetJump(true);

    }

    void Movement()
    {

        //moveX = Input.GetButton("Horizontal") ? moveSpeed * Input.GetAxis("Horizontal") : 0;
        moveX = hzInput != null ? moveSpeed * (float)hzInput : 0;
        moveY = rb.velocity.y;
        //moveZ = Input.GetButton("Vertical") ? moveSpeed * Input.GetAxis("Vertical") : 0;
        moveZ = vtInput != null ? moveSpeed * (float)vtInput : 0;

        if ((anim.GetInteger("States") == 2 && grounded) && !justJumped)
        {
            anim.SetInteger("States", 0);
        }
        if ((moveX != 0 || moveZ != 0) && grounded)
        {
            anim.SetInteger("States", 1);
        }
        else if (grounded)
        {
            anim.SetInteger("States", 0);
        }



        if (DEBUG_MULTIJUMP)
        {
            if (jump && jumpCount > 0)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
                jumpCount--;
                jump = false;
                justJumped = true;
                anim.SetInteger("States", 2);
                audioSrc.Play();

                ScoreManager.instance.IncreaseJumps();

            }
        }
        else
        {
            if (jump && grounded)
            {
                jump = false;
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Acceleration);
                justJumped = true;
                audioSrc.Play();
                anim.SetInteger("States", 2);

                ScoreManager.instance.IncreaseJumps();
            }
        }
		switch (GameManager.instance.m_CameraState) 
		{
		case CameraState.Above:
                FixVertCam(true);
			    break;
		case CameraState.Below:
                FixVertCam(false);
                break;
		case CameraState.Front:
			rb.velocity = new Vector3(-moveX, moveY, 0);
			    break;
		case CameraState.Behind:
			rb.velocity = new Vector3(moveX, moveY, 0);
			    break;
		case CameraState.Left:
			rb.velocity = new Vector3(0, moveY, -moveX);
			    break;
		case CameraState.Right:
			rb.velocity = new Vector3(0, moveY, moveX);
			    break;
		}
		//rb.velocity = new Vector3(moveX, moveY, moveZ);

    }

    void FixVertCam(bool _top)
    {
 
        if (cam.transform.up.x < -0.5f)
        {
            if (_top)
                rb.velocity = new Vector3(-moveZ, moveY, moveX);
            else
                rb.velocity = new Vector3(-moveZ, moveY, -moveX);
        }
        if (cam.transform.up.x > 0.5f)
        {
            if (_top)
                rb.velocity = new Vector3(moveZ, moveY, -moveX);
            else
                rb.velocity = new Vector3(moveZ, moveY, moveX);
        }
        if (cam.transform.up.z < -0.5f)
        {
            if (_top)
                rb.velocity = new Vector3(-moveX, moveY, -moveZ);
            else
                rb.velocity = new Vector3(moveX, moveY, -moveZ);
        }
        if (cam.transform.up.z > 0.5f)
        {
            if (_top)
                rb.velocity = new Vector3(moveX, moveY, moveZ);
            else
                rb.velocity = new Vector3(-moveX, moveY, moveZ);
        }

    }

    public void SetGrounded(bool _val)
    {
        grounded = _val;
        if (DEBUG_MULTIJUMP && grounded && !justJumped)
        {            
            jumpCount = jumpMax;
        }
    }

    public void FreezeUnfreeze(bool _freeze)
    {
        if (_freeze)
            storeY = rb.velocity.y;
        else
            rb.velocity = new Vector3(0, storeY, 0);

        frozen = _freeze;
    }

    public void DisableInput(bool _set = true)
    {
        nullifyInput = _set;
    }
}
