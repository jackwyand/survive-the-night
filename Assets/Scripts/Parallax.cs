using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length, startpos, ypos;
    public GameObject cam;
    public float parallaxEffect;

    public bool verticalParallax;

    void Start(){

        startpos = transform.position.x;
        ypos = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update(){

        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);
        float ydist = (cam.transform.position.y * parallaxEffect);

        if(verticalParallax){

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);
        }
        else{
            transform.position = new Vector3(startpos + dist, ypos + ydist, transform.position.z);
        }
    }
}