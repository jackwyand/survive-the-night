using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxChange : MonoBehaviour
{
    Renderer rend;

    Texture2D MainTexture;

    private DayNightCycle dayNightCycle;
    public GameObject global_light;

    private float defaultBlend = 1;

    public bool isBackground;

    public float StartBlendValue;

    void Start(){
        dayNightCycle = global_light.GetComponent<DayNightCycle>();
        rend = GetComponent<Renderer>();
        MainTexture = Helpers.ConvertSpriteToTexture(this.GetComponent<SpriteRenderer>().sprite);
        rend.material.SetTexture("MainTex", MainTexture);
        if(!isBackground){
            rend.material.SetFloat("StarSize", 99999999);
        }
        Blend(StartBlendValue);
    }

    void Update(){

        if(dayNightCycle.hours >= 21 && dayNightCycle.hours < 22){
            Blend(Mathf.Lerp(defaultBlend, 0, dayNightCycle.seconds / 3600));
        }
        else if(dayNightCycle.hours > 7 && dayNightCycle.hours < 21){
            Blend(defaultBlend);
        }
    }

    void Blend(float blend){
        rend.material.SetFloat("BlendValue", blend);
    }
}
