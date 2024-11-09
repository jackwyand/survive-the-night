using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTextFade : MonoBehaviour
{
    public Animator transitionAnim;

    public void FadeIn(){
        transitionAnim.SetTrigger("FadeIn");
    }
}
