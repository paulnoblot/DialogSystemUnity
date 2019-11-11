using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player singleton;

    [SerializeField] string playerName = "Gaston";
    public string PlayerName { get { return playerName; } }

    [SerializeField] int level = 1;
    public int Level { get { return level; } }

    [SerializeField] Race race = Race.Human;
    public Race Race { get { return race; } }

    [SerializeField] int money = 500;
    public int Money { get { return money; } }

    private void Start()
    {
        if(singleton == null)
            singleton = this;
    }
}
