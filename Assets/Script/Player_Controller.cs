using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private Rigidbody _rb;
    private float _Input;
    [SerializeField]
    private float Speed;
    [SerializeField]
    private float AirSpeed;
    [SerializeField]
    private float Acceleration;
    [SerializeField]
    private float Deceleration;
    [SerializeField]
    private float velPower;
    [SerializeField]
    private float jump;
    [SerializeField]
    private float WallJumpUp;
    [SerializeField]
    private float WallJumpSide;
    [SerializeField]
    private float WallSlide;
    [SerializeField]
    private float Speed2;
    [SerializeField]
    private float jump2;
    [SerializeField]
    private float TransfoTime;
    public bool Grounded;
    private bool Jumping;
    private float yVelocity;
    private float xVelocity;
    private float movementX;
    public GameObject Mesh;
    private bool wallSlide;
    private bool wallJumping;
    private bool Fluid;
    private bool Transforming;
    public Material hard;
    public Material fluid;
    public GameObject Slime;
    //public Animator Anim;
    // Start is called before the first frame update
    void Start()
    {

        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        xVelocity = _rb.velocity.x;
        yVelocity = _rb.velocity.y;

        Clip();
        Move();
        Jump();
        if (!Fluid && !Grounded) WallJump();
        if (Fluid && !Grounded) WallJumpFluid();
    }

    private void Move()
    {
        _Input = Input.GetAxis("Horizontal");
        if (Grounded) wallSlide = false;

        if (Grounded && !wallJumping && !Fluid)
        {
            float targetSpeedX = _Input * Speed;
            float speedDifX = targetSpeedX - _rb.velocity.x;
            float accelRateX = (Mathf.Abs(targetSpeedX) < 0.01f) ? Acceleration : Deceleration;
            movementX = Mathf.Pow(Mathf.Abs(speedDifX) * accelRateX, velPower) * Mathf.Sign(speedDifX);
        }
        if (!Grounded && !wallJumping && !Fluid)
        {
            float targetSpeedX = _Input * AirSpeed;
            float speedDifX = targetSpeedX - _rb.velocity.x;
            float accelRateX = (Mathf.Abs(targetSpeedX) < 0.01f) ? Acceleration : Deceleration;
            movementX = Mathf.Pow(Mathf.Abs(speedDifX) * accelRateX, velPower) * Mathf.Sign(speedDifX);
        }
        if ((Grounded && Fluid))
        {
            float targetSpeedX = _Input * Speed2;
            float speedDifX = targetSpeedX - _rb.velocity.x;
            float accelRateX = (Mathf.Abs(targetSpeedX) < 0.01f) ? Acceleration : Deceleration;
            movementX = Mathf.Pow(Mathf.Abs(speedDifX) * accelRateX, velPower) * Mathf.Sign(speedDifX);
        }
        if (!Grounded && Fluid) movementX = 0;

        _rb.AddForce(movementX * transform.right * Time.deltaTime);

        if (xVelocity > 0 && !wallSlide) Mesh.transform.localEulerAngles = new Vector3(-90, 180, 0);
        if (xVelocity < 0 && !wallSlide) Mesh.transform.localEulerAngles = new Vector3(-90, 0, 0);
    }

    private void Jump()
    {
        if (Physics.Raycast(transform.position - new Vector3(0, 0.45f, 0), -transform.up, 0.2f))
        {
            Grounded = true;
        }

        if (!Physics.Raycast(transform.position - new Vector3(0, 0.45f, 0), -transform.up, 0.2f))
        {
            if (Grounded && !Jumping)
            {
                StartCoroutine(CoyoteTime());
            }
            if (Grounded && Jumping)
            {
                Grounded = false;
            }
        }

        if (Input.GetButton("Jump") && Grounded && !Jumping && !Fluid)
        {
            _rb.velocity = new Vector2(xVelocity, jump);
            Grounded = false;
            Jumping = true;
            StartCoroutine(JumpRefresh());
        }
        if (Input.GetButton("Jump") && Grounded && !Jumping && Fluid)
        {
            _rb.velocity = new Vector2(xVelocity, jump2);
            Grounded = false;
            Jumping = true;
            StartCoroutine(JumpRefresh());
        }

        yVelocity = _rb.velocity.y;
        if (yVelocity < 0) Physics.gravity = new Vector3(0, -30, 0);
        if (yVelocity >= 0 || !Grounded) Physics.gravity = new Vector3(0, -20f, 0);
    }

    private void WallJump()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position - new Vector3(0.45f, 0, 0), -transform.right, out hit, 0.2f) && !hit.collider.CompareTag("Raycast Ignore"))
        {
            wallSlide = true;
            if (wallSlide && Input.GetButtonDown("Jump") && !Jumping)
            {
                _rb.velocity = new Vector2(WallJumpSide, WallJumpUp);
                wallJumping = true;
                wallSlide = false;
                StartCoroutine(WallJumpingTime());
            }
            if (yVelocity > 0 && !wallJumping) _rb.velocity = new Vector3(0, yVelocity, 0);
            if (yVelocity < 0 && !wallJumping) _rb.velocity = new Vector3(0, -WallSlide, 0);
        }
        else if (Physics.Raycast(transform.position + new Vector3(0.45f, 0, 0), transform.right, out hit, 0.2f) && !hit.collider.CompareTag("Raycast Ignore"))
        {
            wallSlide = true;
            if (wallSlide && Input.GetButtonDown("Jump") && !Jumping)
            {
                _rb.velocity = new Vector2(-WallJumpSide, WallJumpUp);
                wallJumping = true;
                wallSlide = false;
                StartCoroutine(WallJumpingTime());
            }
            if (yVelocity > 0 && !wallJumping) _rb.velocity = new Vector3(0, yVelocity, 0);
            if (yVelocity < 0 && !wallJumping) _rb.velocity = new Vector3(0, -WallSlide, 0);

        }
        else wallSlide = false;
    }

    void WallJumpFluid()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position - new Vector3(0.45f, 0, 0), -transform.right, out hit, 0.2f) && !hit.collider.CompareTag("Raycast Ignore"))
        {
            if (yVelocity > 0) _rb.velocity = new Vector3(0, yVelocity, 0);
            if (yVelocity < 0) _rb.velocity = new Vector3(0, -WallSlide, 0);

        }
        else if (Physics.Raycast(transform.position + new Vector3(0.45f, 0, 0), transform.right, out hit, 0.2f) && !hit.collider.CompareTag("Raycast Ignore"))
        {
            if (yVelocity > 0) _rb.velocity = new Vector3(0, yVelocity, 0);
            if (yVelocity < 0) _rb.velocity = new Vector3(0, -WallSlide, 0);
        }
    }


    private void Clip()
    {
        if (Input.GetButtonDown("Fire1") && !Fluid && !Transforming)
        {
            gameObject.layer = 8;
            Fluid = true;
            Transforming = true;
            Slime.GetComponent<MeshRenderer>().material = fluid;
            StartCoroutine(TransfoDelay());
        }
        if (Input.GetButtonDown("Fire1") && Fluid && !Transforming)
        {
            gameObject.layer = 7;
            Fluid = false;
            Transforming = true;
            Slime.GetComponent<MeshRenderer>().material = hard;
            StartCoroutine(TransfoDelay());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tramplin"))
        {
            other.gameObject.GetComponent<Tramplin>().Launch(_rb);
            wallJumping = true;
            StartCoroutine(WallJumpingTime());
        }
    }

    IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(0.15f);
        Grounded = false;
    }

    IEnumerator JumpRefresh()
    {
        yield return new WaitForSeconds(0.3f);
        Jumping = false;
    }

    IEnumerator WallJumpingTime()
    {
        yield return new WaitForSeconds(0.5f);
        wallJumping = false;
    }

    IEnumerator TransfoDelay()
    {
        yield return new WaitForSeconds(TransfoTime);
        Transforming = false;
    }
}