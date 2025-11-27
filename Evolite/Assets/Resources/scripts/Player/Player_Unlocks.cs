using System.Collections.Generic;
using UnityEngine;

public class Player_Unlocks : MonoBehaviour
{
    public static Player_Unlocks instance;

    public class UnlockPair
    {
        public PlayerSetPart.PlayerPartType type;
        public int id;
    }

    public List<UnlockPair> unlockedParts = new List<UnlockPair>();

    public bool HasPart(PlayerSetPart.PlayerPartType type, int id)
    {
        return unlockedParts.Exists(p => p.type == type && p.id == id);
    }

    public void UnlockPart(PlayerSetPart.PlayerPartType type, int id)
    {
        if (!HasPart(type, id))
            unlockedParts.Add(new UnlockPair() { type = type, id = id });
    }
}
