using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

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
    public Animator anim;
    public GameObject Pivot;
    public ParticleSystem transfo;
    public Transform Respawn;
    private bool Dead;
    public ParticleSystem RespawnFX;
    public ParticleSystem DeadFx;
    public ParticleSystem Indicateur1;
    public ParticleSystem Indicateur2;
    public ParticleSystem Indicateur3;
    public ParticleSystem Indicateur4;
    public ParticleSystem Indicateur5;
    //public Animator Anim;
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Dead)
        {
            xVelocity = _rb.velocity.x;
            anim.SetFloat("Xvelocity", Mathf.Abs(xVelocity));
            yVelocity = _rb.velocity.y;
            anim.SetFloat("Yvelocity", yVelocity);

            Clip();
            Move();
            Jump();
            if (!Fluid && !Grounded) WallJump();
            if (Fluid && !Grounded) WallJumpFluid();
            if (!Grounded && !wallSlide) AirRota();
        }
    }

    private void Move()
    {
        _Input = Input.GetAxis("Horizontal");
        if (Grounded)
        {
            anim.SetBool("Wall", false);
            wallSlide = false;
            Pivot.transform.localEulerAngles = new Vector3(0, 0, 0);
        }

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
        RaycastHit hit;
        if (Physics.Raycast(transform.position - new Vector3(0, 0.45f, 0), -transform.up, out hit, 0.2f) )
        {
            if (!hit.collider.CompareTag("Raycast Ignore"))
            {
                Grounded = true;
                anim.SetBool("Grounded", true);
            }
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
                anim.SetBool("Grounded", false);
            }
        }

        if (Input.GetButton("Jump") && Grounded && !Jumping && !Fluid)
        {
            _rb.velocity = new Vector2(xVelocity, jump);
            Grounded = false;
            anim.SetBool("Grounded", false);
            Jumping = true;
            StartCoroutine(JumpRefresh());
        }
        if (Input.GetButton("Jump") && Grounded && !Jumping && Fluid)
        {
            _rb.velocity = new Vector2(xVelocity, jump2);
            Grounded = false;
            anim.SetBool("Grounded", false);
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
            Pivot.transform.localEulerAngles = new Vector3(0, 0, -90);
            anim.SetBool("Wall", true);
            wallSlide = true;
            if (wallSlide && Input.GetButtonDown("Jump") && !Jumping)
            {
                _rb.velocity = new Vector2(WallJumpSide, WallJumpUp);
                wallJumping = true;
                wallSlide = false;
                anim.SetBool("Wall", false);
                StartCoroutine(WallJumpingTime());
            }
            if (yVelocity > 0 && !wallJumping) _rb.velocity = new Vector3(0, yVelocity, 0);
            if (yVelocity < 0 && !wallJumping) _rb.velocity = new Vector3(0, -WallSlide, 0);
        }
        else if (Physics.Raycast(transform.position + new Vector3(0.45f, 0, 0), transform.right, out hit, 0.2f) && !hit.collider.CompareTag("Raycast Ignore"))
        {
            Pivot.transform.localEulerAngles = new Vector3(0, 0, 90);
            anim.SetBool("Wall", true);
            wallSlide = true;
            if (wallSlide && Input.GetButtonDown("Jump") && !Jumping)
            {
                _rb.velocity = new Vector2(-WallJumpSide, WallJumpUp);
                wallJumping = true;
                wallSlide = false;
                anim.SetBool("Wall", false);
                StartCoroutine(WallJumpingTime());
            }
            if (yVelocity > 0 && !wallJumping) _rb.velocity = new Vector3(0, yVelocity, 0);
            if (yVelocity < 0 && !wallJumping) _rb.velocity = new Vector3(0, -WallSlide, 0);

        }
        if (!Physics.Raycast(transform.position - new Vector3(0.45f, 0, 0), -transform.right, 0.2f) && !Physics.Raycast(transform.position + new Vector3(0.45f, 0, 0), transform.right, 0.2f))
        {
            wallSlide = false;
            anim.SetBool("Wall", false);
        }
    }

    void WallJumpFluid()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position - new Vector3(0.45f, 0, 0), -transform.right, out hit, 0.2f) && !hit.collider.CompareTag("Raycast Ignore"))
        {
            wallSlide = true;
            Pivot.transform.localEulerAngles = new Vector3(0, 0, -90);
            anim.SetBool("Wall", true);
            if (yVelocity > 0) _rb.velocity = new Vector3(0, yVelocity, 0);
            if (yVelocity < 0) _rb.velocity = new Vector3(0, -WallSlide, 0);

        }
        else if (Physics.Raycast(transform.position + new Vector3(0.45f, 0, 0), transform.right, out hit, 0.2f) && !hit.collider.CompareTag("Raycast Ignore"))
        {
            wallSlide = true;
            Pivot.transform.localEulerAngles = new Vector3(0, 0, 90);
            anim.SetBool("Wall", true);
            if (yVelocity > 0) _rb.velocity = new Vector3(0, yVelocity, 0);
            if (yVelocity < 0) _rb.velocity = new Vector3(0, -WallSlide, 0);
        }
        if (!Physics.Raycast(transform.position - new Vector3(0.45f, 0, 0), -transform.right, 0.2f) && !Physics.Raycast(transform.position + new Vector3(0.45f, 0, 0), transform.right, 0.2f))
        {
            wallSlide = false;
            anim.SetBool("Wall", false);
        }

    }


    private void Clip()
    {
        if (Input.GetButtonDown("Fire1") && !Fluid && !Transforming)
        {
            gameObject.layer = 8;
            Fluid = true;
            Transforming = true;
            Slime.GetComponent<SkinnedMeshRenderer>().material = fluid;
            StartCoroutine(TransfoDelay());
            transfo.Play();
        }
        if (Input.GetButtonDown("Fire1") && Fluid && !Transforming)
        {
            gameObject.layer = 7;
            Fluid = false;
            Transforming = true;
            Slime.GetComponent<SkinnedMeshRenderer>().material = hard;
            StartCoroutine(TransfoDelay());
            transfo.Play();
        }
    }

    void AirRota()
    {
        var angle1 = Mathf.Atan2(_rb.velocity.y, _rb.velocity.x) * Mathf.Rad2Deg;
        Pivot.transform.localEulerAngles = new Vector3(0, 0, angle1 - 90);
    }

    void Death()
    {
        Dead = true;
        StartCoroutine(DeathDelay());
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tramplin"))
        {
            other.gameObject.GetComponent<Tramplin>().Launch(_rb);
            wallJumping = true;
            StartCoroutine(WallJumpingTime());
        }
        else if (other.gameObject.CompareTag("Death"))
        {
            if (!Dead) Death();

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Raycast Ignore"))
        {
            if (!Dead) Death();
        }
    }

    IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(0.15f);
        Grounded = false;
        anim.SetBool("Grounded", false);
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
        Indicateur1.Play();
        Indicateur2.Play();
        Indicateur3.Play();
        Indicateur4.Play();
        Indicateur5.Play();
    }
    IEnumerator DeathDelay()
    {
        DeadFx.Play();
        Mesh.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        RespawnFX.Play();
        yield return new WaitForSeconds(0.5f);
        Mesh.SetActive(true);
        Dead = false;
        transform.position = Respawn.position;  
    }
}