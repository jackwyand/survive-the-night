using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    public Crossfade transition;

    public void PlayGame(){

        transition.LoadNextLevel();
    }
}
