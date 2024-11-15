using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro; // using text mesh for the clock display

using UnityEngine.Rendering; // used to access the volume component

public class DayNightCycle : MonoBehaviour
{
    public TextMeshProUGUI timeDisplay; // Display Time
    public TextMeshProUGUI dayDisplay; // Display Day
    public UnityEngine.Rendering.Universal.Light2D gLight; // this is the post processing volume
    public Color dayColor;
    public Color nightColor;

    public float tickStart; // Increasing the tick, increases second rate
    private float tick;
    public float seconds; 
    public float mins;
    public int hours;
    public int days = 1;

    public bool activateLights; // checks if lights are on
    public GameObject[] lights; // all the lights we want on when its dark
    public SpriteRenderer[] stars; // star sprites 

    public GameObject Boss1;
    bool hasSpawned = false;

    // Start is called before the first frame update
    void Start()
    {   
        if(hours < 22){
            Boss1.SetActive(false);
        }
        gLight = gameObject.GetComponent<UnityEngine.Rendering.Universal.Light2D>();
    }

    // Update is called once per frame
    void FixedUpdate() // we used fixed update, since update is frame dependant. 
    {
        CalcTime();
    }

    public void CalcTime() // Used to calculate sec, min and hours
    {
        if(hours < 21 && hours >= 7){
            tick = tickStart * 12f;
        }
        else if(hours == 21 || hours == 6){
            tick = tickStart * 6f;
        }
        else{
            tick = tickStart;
        }
        seconds += Time.fixedDeltaTime * tick; // multiply time between fixed update by tick
        if((hours >= 21 && hours < 22) || (hours >= 6 && hours < 7)){
            mins += Time.fixedDeltaTime * tick / 60;
        }
        else if(seconds >= 60){
            seconds = 0;
            mins += 1;
            mins = Mathf.Abs(mins);
        }


        if (mins >= 60) //60 min = 1 hr
        {
            mins = 0;
            hours += 1;
        }

        if (hours >= 24) //24 hr = 1 day
        {
            hours = 0;
            days += 1;
        }
        ControlPPV(); // changes post processing volume after calculation
        if(!hasSpawned){
            SpawnEnemies();
        }
    }

    public void ControlPPV() // used to adjust the post processing slider.
    {
        //ppv.weight = 0;
        if(hours>=21 && hours<22) // dusk at 21:00 / 9pm    -   until 22:00 / 10pm
        {
            if(gLight.color != nightColor){
                gLight.color =  Color.Lerp(dayColor, nightColor, (float)seconds / 3600); // since dusk is 1 hr, we just divide the mins by 60 which will slowly increase from 0 - 1 
                for (int i = 0; i < stars.Length; i++)
                {
                    stars[i].color = new Color(stars[i].color.r, stars[i].color.g, stars[i].color.b, (float)mins / 60); // change the alpha value of the stars so they become visible
                }

                if (activateLights == false) // if lights havent been turned on
                {
                    if (seconds > 2700) // wait until pretty dark
                    {
                        for (int i = 0; i < lights.Length; i++)
                        {
                            lights[i].SetActive(true); // turn them all on
                        }
                        activateLights = true;
                    }
                }
            }
        }
     

        if(hours>=6 && hours<7) // Dawn at 6:00 / 6am    -   until 7:00 / 7am
        {
            gLight.color =  Color.Lerp(nightColor, dayColor, (float)seconds / 3600); // we minus 1 because we want it to go from 1 - 0
            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].color = new Color(stars[i].color.r, stars[i].color.g, stars[i].color.b, 1 -(float)mins / 60); // make stars invisible
            }
            if (activateLights == true) // if lights are on
            {
                if (mins > 45) // wait until pretty bright
                {
                    for (int i = 0; i < lights.Length; i++)
                    {
                        lights[i].SetActive(false); // shut them off
                    }
                    activateLights = false;
                }
            }
        }
    }
    public void SpawnEnemies(){
        if(hours >= 22){
            Boss1.SetActive(true);
            hasSpawned = true;
        }
    }
}