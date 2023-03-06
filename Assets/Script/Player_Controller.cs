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
    public bool Grounded;
    public bool Jumping;
    private float yVelocity;
    private float xVelocity;
    private float movementX;
    public GameObject Mesh;
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

        Move();
        Jump();
    }

    private void Move()
    {
        _Input = Input.GetAxis("Horizontal");

        if (Grounded)
        {
            float targetSpeedX = _Input * Speed;
            float speedDifX = targetSpeedX - _rb.velocity.x;
            float accelRateX = (Mathf.Abs(targetSpeedX) < 0.01f) ? Acceleration : Deceleration;
            movementX = Mathf.Pow(Mathf.Abs(speedDifX) * accelRateX, velPower) * Mathf.Sign(speedDifX);
        }
        if (!Grounded)
        {
            float targetSpeedX = _Input * AirSpeed;
            float speedDifX = targetSpeedX - _rb.velocity.x;
            float accelRateX = (Mathf.Abs(targetSpeedX) < 0.01f) ? Acceleration : Deceleration;
            movementX = Mathf.Pow(Mathf.Abs(speedDifX) * accelRateX, velPower) * Mathf.Sign(speedDifX);
        }
        _rb.AddForce(movementX * transform.right * Time.deltaTime);

        if (xVelocity > 0) Mesh.transform.localEulerAngles = new Vector3(-90, 180, 0);
        if (xVelocity < 0) Mesh.transform.localEulerAngles = new Vector3(-90, 0, 0);
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

        if (Input.GetButton("Jump") && Grounded && !Jumping)
        {
            _rb.velocity = new Vector2(xVelocity, jump);
            Grounded = false;
            Jumping = true;
            StartCoroutine(JumpRefresh());
        }

        yVelocity = _rb.velocity.y;
        if (yVelocity < 0) Physics.gravity = new Vector3(0, -20, 0);
        if (yVelocity >= 0 || !Grounded) Physics.gravity = new Vector3(0, -11f, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position - new Vector3(0, 0.45f, 0), transform.up * -0.3f);
    }

    IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(0.15f);
        Grounded = false;
    }

    IEnumerator JumpRefresh()
    {
        yield return new WaitForSeconds(0.5f);
        Jumping = false;
    }
}