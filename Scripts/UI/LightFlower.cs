using System.Collections;
using UnityEngine;

public class LightFlower : MonoBehaviour
{
    private VignetteManager vgManager;
    private bool isActive = true;
    
    [SerializeField] private float vgIntensity;
    private AudioSource AudioSource;

    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        vgManager = VignetteManager.Instance;
    }

    public IEnumerator InteractedWith()
    {
        AudioSource.Play();
        if (vgManager.GetVignetteValue() > vgIntensity)
            vgManager.SetVignette(vgIntensity);

        yield return new WaitForSeconds(1f);
        isActive = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(isActive)
        {
            if (other.tag == "Player" || other.tag == "Block")
            {
                isActive = false;
                StartCoroutine(InteractedWith());
            }
        }
    }

}