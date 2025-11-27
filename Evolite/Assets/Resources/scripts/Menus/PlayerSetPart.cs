using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetPart : MonoBehaviour
{
    public Player_General plr;
    public int iD;
    public PlayerPartType partType;
    private Button btn;

    public enum PlayerPartType
    {
        Head,
        Eye,
        Pupil,
        BodyAccessory,
        HeadAccessory,
        FaceAccessory,
        Tail
    }

    private void Awake()
    {
        btn = GetComponent<Button>();
    }

    private void Start()
    {
        //UpdateButtonLock();
    }

    public void SetPart()
    {
        //if (!IsUnlocked())
            //return;

        switch (partType)
        {
            case PlayerPartType.Head:
                plr.headIndex = iD;
                break;
            case PlayerPartType.Eye:
                plr.eyeIndex = iD;
                break;
            case PlayerPartType.Pupil:
                plr.pupilIndex = iD;
                break;
            case PlayerPartType.BodyAccessory:
                plr.bodyAcessoriesIndex = iD;
                break;
            case PlayerPartType.HeadAccessory:
                plr.headAcessoriesIndex = iD;
                break;
            case PlayerPartType.FaceAccessory:
                plr.FaceIndex = iD;
                break;
            case PlayerPartType.Tail:
                break;
        }
    }

    //bloqueio das parts

    /*private bool IsUnlocked()
    {
       return Player_Unlocks.instance.HasPart(partType, iD);
    }*/

    // bloqueia o botão caso a parte não esteja desbloqueada
    /*public void UpdateButtonLock()
    {
        if (btn == null) btn = GetComponent<Button>();
        btn.interactable = IsUnlocked();
    }*/

    /*public static void UpdateAllButtons()
    {
        PlayerSetPart[] parts = FindObjectsByType<PlayerSetPart>(FindObjectsSortMode.None);

        foreach (var p in parts)
            p.UpdateButtonLock();
    }*/

    public class UnlockPair
    {
        public PlayerSetPart.PlayerPartType type;
        public int id;
    }

    public List<UnlockPair> unlockedParts = new List<UnlockPair>();

    /*public bool HasPart(PlayerSetPart.PlayerPartType type, int id)
    {
        return unlockedParts.Exists(p => p.type == type && p.id == id);
    }*/

    /*public void UnlockPart(PlayerSetPart.PlayerPartType type, int id)
    {
        if (!HasPart(type, id))
            unlockedParts.Add(new UnlockPair() { type = type, id = id });
    }*/
}
