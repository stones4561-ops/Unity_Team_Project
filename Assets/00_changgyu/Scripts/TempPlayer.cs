using UnityEngine;
using System.Collections.Generic;

public class TempPlayer : MonoBehaviour
{
  
    public PlayerInventory inventory;

    
    


    private void Update()
    {

        if(Input.GetKeyDown(KeyCode.I))
        {
            TurnInventory();
        }
    }

    private void TurnInventory()
    {
        if(inventory.gameObject.activeSelf)
            inventory.gameObject.SetActive(false);
        else
            inventory.gameObject.SetActive(true);
    }
    



}
