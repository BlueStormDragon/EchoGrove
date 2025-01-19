using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationForTrailer : MonoBehaviour
{
    [SerializeField] private string aClip;
    [SerializeField] private Animator anim;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            anim.Play(aClip.ToString());
        }
    }
}
