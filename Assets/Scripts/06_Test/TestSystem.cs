using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TestSystem : MonoBehaviour
{
    [SerializeField] public Transform respawnTP;
    [SerializeField] public List<EItemID> listItemId;

    private void Start()
    {
        StartCoroutine(StartNextFrame());
    }

    private IEnumerator StartNextFrame()
    {
        yield return null; // 다음 프레임

        var instance = GameInstance.Instance;
         
        var disableCanvas = instance
            .UI_GetPersistentUIGroup()
            .GetDisableCanvas();
        disableCanvas.DisableInstant();

        var respawn = instance.PLAYER_GetPlayerRespawn();
        respawn.SpawnInFarm(respawnTP);

        PlayerStorage storage = instance.PLAYER_GetPlayerStorage();
        storage.MakeStorageByCapacity();
         
        foreach (var itemId in listItemId)
        {
            int emptyIndex = storage.GetEmptyIndex();
            if (emptyIndex < 0)
                break;

            var item = instance.SPAWN_MakeItem(itemId);
            storage.InsertItemNotRenewUI(emptyIndex, item);
        }
    }
} 
 