using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menue : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump")) UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        if (Input.GetButtonDown("Fire1")) Application.Quit();
    }
}
