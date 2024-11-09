using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Crossfade : MonoBehaviour
{

    public Animator transition;

    public float transitionTime = 1f;


    private void OnEnable(){
        PlayerController.OnPlayerDeath += playerHasDied;
    }
    private void OnDisable(){
        PlayerController.OnPlayerDeath -= playerHasDied;
    }
    public void LoadNextLevel(){
        StartCoroutine(LoadLevel());
    }

    public void playerHasDied(){
        StartCoroutine(playerDeath());
    }

    IEnumerator LoadLevel(){

        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        if(SceneManager.GetActiveScene().name == "MainMenu"){
            SceneManager.LoadScene("Forest");
        }
        else if(SceneManager.GetActiveScene().name == "Forest"){
            SceneManager.LoadScene("MainMenu");
        }
    }

    IEnumerator playerDeath(){

        transition.SetTrigger("pDeath");

        yield return new WaitForSeconds(transitionTime * 2f);

        SceneManager.LoadScene("MainMenu");
    }
}
