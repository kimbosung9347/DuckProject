using UnityEngine;

public class PlayerSystem : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private WareHouse wareHouse;

    private GameObject player;
    private GameObject playerObj; 

    public GameObject MakePlayer()
    { 
        player = Instantiate(playerPrefab, transform.position, transform.rotation, transform);
        playerObj = player.GetComponentInChildren<PlayerController>().gameObject;
        return GetPlayerObj();
    }   
    public void DelatePlayer()
    {
        if (player)
        {
            Destroy(player);
        }
         
        player = null;
        playerObj= null;
    }   
      
    public GameObject GetPlayerObj() { return playerObj; }
    public WareHouse GetWareHouse() { return wareHouse; }
    public Transform GetPlayerTransform() { return playerObj.GetComponent<Transform>(); }
    public PlayerInteraction GetPlayerInteraction() {  return playerObj.GetComponent<PlayerInteraction>(); }
    public PlayerStorage GetPlayerStorage() { return playerObj.GetComponent<PlayerStorage>();}
    public PlayerEquip GetPlayerEquip() { return playerObj.GetComponent<PlayerEquip>();}
    public PlayerQuickSlot GetPlayerQuickSlot() { return playerObj.GetComponent<PlayerQuickSlot>();}
    public PlayerController GetPlayerController() { return playerObj.GetComponent<PlayerController>();}
}
  