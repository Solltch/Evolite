using UnityEngine;

public class PlayerSetPart : MonoBehaviour
{
    public Player_General plr;
    public int iD;
    public PlayerPartType partType;
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
    public void SetPart()
    {
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
            default:
                break;
        }
    }
}
