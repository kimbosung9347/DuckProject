using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UIElements;

public class AiDetected : DuckDetected
{ 
    private float detectedTime = 0f;
    private float maxDetectedTime = 0.15f;
    private bool isDetected = false;
    
    private void Update()
    {
        detectedTime += Time.deltaTime;
        if (detectedTime > maxDetectedTime)
        {
            detectedTime = 0f;
            DisableAiRenderer();
        } 
    } 

    public override void Spawn()
    {
        base.Spawn(); 
        isDetected = true;
        DisableAiRenderer();
    } 


    public void ActiveAiRenderer()
    {
        detectedTime = 0f;

        if (isDetected == true)
            return; 
         
        isDetected = true;
        ActiveDuckRenderer();
    }

    public void DisableAiRenderer()
    {
        if (isDetected == false)
            return;

        isDetected = false;
        // 자신의 모든 랜더러를 꺼주기

        DisableDuckRenderer();
    }
}
