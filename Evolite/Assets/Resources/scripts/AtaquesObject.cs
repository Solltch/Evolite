using UnityEngine;

public class AtaquesObject : MonoBehaviour
{
    [Header("Valores")]
    public string nome;
    public float damage;
    public float atkSpeed;
    public float range;
    public AnimationClip animations;

    [Header("Efeitos Opcionais")]
    public GameObject projectilePrefab;
    public AudioClip soundEffect;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
