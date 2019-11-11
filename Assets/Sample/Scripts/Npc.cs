using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Npc : MonoBehaviour
{
    [SerializeField] string npcName = "Gaston";
    public string NpcName { get { return npcName; } }

    [SerializeField] int level = 1;
    public int Level { get { return level; } }

    [SerializeField] Race race = Race.Human;
    public Race Race { get { return race; } }

    [SerializeField] int money = 500;
    public int Money { get { return money; } }

    [SerializeField] Reputation reputation = Reputation.Neutral;
    public Reputation Reputation { get { return reputation; } }

    [SerializeField] string dialogName;
    DialogBoard dialog = null;
    NpcState state;

    private void Start()
    {
        dialog = DialogManager.Singleton.GetDialog(dialogName);

        if (dialog != null)
        {
            state = new NpcState();
            state.npc = this;
            dialog.state = state;
        }
    }

    public void Talk(Player player)
    {
        state.player = player;
        UIDialog.singleton.ShowWindow(this, dialog);
    }

    public void OnMouseDown()
    {
        Talk(Player.singleton);
    }
}
