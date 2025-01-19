using UnityEngine;

public class FallingTreePlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject trigger;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !audioSource.isPlaying)
        {
            audioSource.Play();
            Destroy(trigger);
        }
        
    }
}
