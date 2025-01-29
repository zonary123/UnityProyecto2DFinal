using System;
using System.Collections.Generic;
using Code.Scripts;
using UnityEngine;

public class Inventory{
    [Header("Inventory")]
    [SerializeField] public int gold{ set; get; } = 0;
    [SerializeField] public List<Item> items {get; set; }= new();
    
    public void Add(Inventory inventory){
        inventory.gold += gold;
    }

    public bool HasKey(String itemId, int quantity){
        for (var i = 0; i < items.Count; i++){
            if (items[i].id == itemId){
                int num = items[i].quantity;
                if (num >= quantity){
                    items[i].quantity -= quantity;
                    return true;
                }
                return false;
            }
        }

        return false;
    }
}
