using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LightScript : MonoBehaviour
{
    public UnityEngine.Rendering.Universal.Light2D myLight;

    public float intensityNum;
    public float radiusNum;
    public float intensityCooldownStart;
    private float intensityCooldown;
    public bool isFire;
    private DayNightCycle dayNightScript;
    public GameObject global_light;
    public Color startingColor;

    void Start(){

        myLight = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
        myLight.color = startingColor;
        dayNightScript = global_light.GetComponent<DayNightCycle>();
        intensityCooldown = intensityCooldownStart;
    }

    public void Update(){
        if(intensityCooldown <= 0){
            if(dayNightScript.hours >= 21 && dayNightScript.hours < 22){
                myLight.intensity = Mathf.Lerp(0, intensityNum, dayNightScript.seconds / 3600);
                myLight.color = Color.Lerp(startingColor, startingColor - dayNightScript.nightColor, dayNightScript.seconds / 3600);
            }
            if(isFire){
                myLight.pointLightOuterRadius = Random.Range(radiusNum -1f, radiusNum);
            }
            intensityCooldown = intensityCooldownStart;
        }
        else{
            intensityCooldown -= Time.deltaTime;
        }
    }    
}
