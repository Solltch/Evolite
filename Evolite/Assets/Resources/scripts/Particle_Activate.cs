using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using MovementState = Creature_General.MovementState;

public class Particle_Activate : MonoBehaviour
{
    public Player_Movement statsP;
    public Creature_General statsC;
    public bool isPlayer;
    public bool isRunning;
    public bool isGrounded;
    public bool onSlope;
    public ParticleSystem particles;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(isPlayer)
        {
            statsP = GameObject.Find("Player Collider").GetComponent<Player_Movement>();
        }
        else
            statsC = transform.GetComponentInParent<Creature_General>();

        particles = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    private void OnEnable()
    {
        Ticker.OnTickAction += Tick;
    }

    private void OnDisable()
    {
        Ticker.OnTickAction -= Tick;
    }

    //roda a cada 0.2s
    private void Tick()
    {
        if (isPlayer)
        {
            isRunning = statsP.isRunning;
            isGrounded = statsP.isGrounded;
            onSlope = statsP.onSlope;

            if (isRunning && isGrounded || onSlope && statsP.currentAngle > statsP.maxSlopeAngle)
            {
                if (!particles.isPlaying)
                    particles.Play();
            }
            else
            {
                if (particles.isPlaying)
                    particles.Stop();
            }
        }
        else
        {
            isRunning = statsC.moveState == MovementState.running;

            if (isRunning)
            {
                if (!particles.isPlaying)
                    particles.Play();
            }
            else
            {
                if (particles.isPlaying)
                    particles.Stop();
            }
        }

    }
}
