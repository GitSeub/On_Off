using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class end : MonoBehaviour
{
    public Animation anim;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            anim.Play();
            StartCoroutine(DelayMenu());
        }
    }

    IEnumerator DelayMenu()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(0);
    }
}
