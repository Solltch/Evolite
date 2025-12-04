using UnityEngine;

public class Creature_Race : MonoBehaviour
{
    public int bodyAcessoriesIndex;
    public int headIndex;
    public int headAcessoriesIndex;
    public int FaceIndex;
    public int eyeIndex;
    public int pupilIndex;

    public float headSize;
    public float headAcessSize;
    public float eyeSize;
    public float pawSize;

    public Material PlayerSkin1;
    public Material PlayerSkin2;
    public Material PlayerSkin3;
    public Material PlayerSkin4;
    public Material PlayerEye;
    public Material PlayerPupil;

    public Material creatureSkin1;
    public Material creatureSkin2;
    public Material creatureSkin3;
    public Material creatureSkin4;
    public Material creatureEye;
    public Material creaturePupil;

    void Awake()
    {
        // Estes valores são gerados de forma 100% aleatória em tempo de execução
        bodyAcessoriesIndex = UnityEngine.Random.Range(-1, 4);
        headIndex = UnityEngine.Random.Range(0, 4);

        // Se headIndex NÃO FOR 0 headAcessoriesIndex vira -1
        if (headIndex != 0)
            headAcessoriesIndex = -1;
        else
            headAcessoriesIndex = UnityEngine.Random.Range(-1, 2);
        FaceIndex = UnityEngine.Random.Range(-1, 9);
        eyeIndex = UnityEngine.Random.Range(-1, 19);
        pupilIndex = UnityEngine.Random.Range(-1, 12);

        headSize = UnityEngine.Random.Range(0.6f, 1.4f);
        headAcessSize = UnityEngine.Random.Range(0.6f, 1.4f);
        eyeSize = UnityEngine.Random.Range(0.6f, 1.4f);
        pawSize = UnityEngine.Random.Range(0.7f, 1.3f);

        creatureSkin1 = new Material(PlayerSkin1);
        creatureSkin2 = new Material(PlayerSkin2);
        creatureSkin3 = new Material(PlayerSkin3);
        creatureSkin4 = new Material(PlayerSkin4);
        creatureEye = new Material(PlayerEye);
        creaturePupil = new Material(PlayerPupil);

        creatureSkin1.color = Random.ColorHSV(0f, 1f, 0.4f, 1f, 0.6f, 1f);
        creatureSkin2.color = Random.ColorHSV(0f, 1f, 0.4f, 1f, 0.6f, 1f);
        creatureSkin3.color = Random.ColorHSV(0f, 1f, 0.4f, 1f, 0.6f, 1f);
        creatureSkin4.color = Random.ColorHSV(0f, 1f, 0.4f, 1f, 0.6f, 1f);

        creatureEye.color = Random.ColorHSV(0f, 1f, 0.3f, 1f, 0.3f, 1f);
        creaturePupil.color = Random.ColorHSV(0f, 1f, 0.2f, 1f, 0.1f, 0.3f);
    }
}
