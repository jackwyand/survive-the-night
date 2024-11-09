using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour

{
    public static CameraShake Instance {get; private set;}
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private float shakeTimer;
    private float startingMagnitude;
    private float shakeTimerTotal;

    private void Awake(){

        Instance = this;
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    public void Shake(float duration, float magnitude){
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = magnitude;

        shakeTimer = duration;
        shakeTimerTotal = duration;
        startingMagnitude = magnitude;
    }

    private void Update(){
        if(shakeTimer > 0){
            shakeTimer -= Time.deltaTime;

            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = 
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingMagnitude, 0f, (1- (shakeTimer / shakeTimerTotal)));
        }
    }

}