using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamManager : MonoBehaviour
{
    public Transform CamPos;
    public Camera Cam;
    public GameObject Respawn;
    public Transform ResPos;
    // Start is called before the first frame update
    void Start()
    {
        Cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && CamPos.position != Cam.transform.position)
        {
            Cam.transform.position = CamPos.position;
            Respawn.transform.position = ResPos.position;
        }
    }
}
