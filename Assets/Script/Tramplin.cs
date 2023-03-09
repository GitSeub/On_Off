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
        rb.velocity = new Vector3(LaunchX, LaunchY, 0);
        
    }
}
