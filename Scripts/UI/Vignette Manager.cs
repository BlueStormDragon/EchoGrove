using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VignetteManager : MonoBehaviour
{
    [SerializeField] private GameObject volume;
    private Volume v;
    private Vignette vg;
    [SerializeField] private float vgMinSpeed = 0.0015f, vgMaxSpeed = 0.01f;

    private bool isRunning = false;

    public static VignetteManager Instance;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        v = volume.GetComponent<Volume>();
        v.profile.TryGet(out vg);

        StartCoroutine(Vignette());
        
        StartCoroutine(SetVignetteValue(0));
    } 

    private IEnumerator Vignette()
    {
        while(true)
        {
            if(vg.intensity.value != 0.8)
                vg.intensity.value += vgMinSpeed;
                
            yield return null;
        }
    }

    public void SetVignette(float value)
    {
        if (!isRunning)
        {
            isRunning = true;
            
            StartCoroutine(SetVignetteValue(value));
        }
        else
        {
            StopCoroutine(SetVignetteValue(value));

            StartCoroutine(SetVignetteValue(value));
        }
    }
    private IEnumerator SetVignetteValue(float value)
    {
        while (vg.intensity.value >= value)
        {
            vg.intensity.value -= vgMaxSpeed;
            yield return null;
        }
    }
    public float GetVignetteValue()
    {
        return vg.intensity.value;
    }
}
