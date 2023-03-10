using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tramplin : MonoBehaviour
{
    [SerializeField]
    private float LaunchX;
    [SerializeField]
    private float LaunchY;
    public Animator anim1;
    public Animator anim2;
    // Start is called before the first frame update

    public void Launch(Rigidbody rb)
    {
        rb.velocity = new Vector3(0, 0, 0);
        rb.velocity = new Vector3(LaunchX, LaunchY, 0);
        anim1.SetTrigger("Boing");
        anim2.SetTrigger("Boing");
        FindObjectOfType<AudioManager>().Play("Jump");
    }
}
