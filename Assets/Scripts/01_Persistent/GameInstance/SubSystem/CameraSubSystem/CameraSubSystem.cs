using NUnit.Framework;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
public class CameraSubSystem : GameInstanceSubSystem
{
    private CameraSystem cachedCameraSystem;
      
    public override void Init()
    {
        CacheCameras();
        cachedCameraSystem.DisableMainCamera();
    }
    public override void LevelStart(ELevelType _type)
    {
        switch (_type)
        {
            case ELevelType.Persistent:
            {
                cachedCameraSystem.DisableMainCamera();
            } 
            break; 
                 
            case ELevelType.Loading:
            {
                cachedCameraSystem.DisableMainCamera();
            }
            break;  

            case ELevelType.Mainmenu:
            {
                cachedCameraSystem.DisableMainCamera();
            }
            break;
                 
            case ELevelType.Home:
            {
                cachedCameraSystem.ActiveMainCamera();
            } 
            break;
                 
            case ELevelType.Farm:
            {
                cachedCameraSystem.ActiveMainCamera();
            }
            break;

            case ELevelType.Test:
            { 
                cachedCameraSystem.ActiveMainCamera();
            }
            break;
        } 
    } 
    public override void LevelEnd(ELevelType _type)
    {
    }

    public CameraSystem GetCameraSystem() { return cachedCameraSystem; }
    
    private void CacheCameras()
    {
        if (cachedCameraSystem)
            return;
         
        cachedCameraSystem = Object.FindAnyObjectByType<CameraSystem>();
    }
} 
