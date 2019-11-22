using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PieGame
{
    public class InventoryManager : MonoBehaviour
    {
        public Weapon curWeapon;


        public void Init()
        {

        }


    }

    [System.Serializable]
    public class Weapon
    {
        public List<Action> actions;
        public List<Action> th_actions;
        //Can use both one and tho hands
        public bool switchable;
    }
}