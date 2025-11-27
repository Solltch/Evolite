using UnityEngine;

public class Ticker : MonoBehaviour
{
    public float tickTime = 0.2f;
    private float _tickTimer;

    public delegate void TickAction();
    public static event TickAction OnTickAction;

    void Update()
    {
        _tickTimer += Time.deltaTime; // acumula tempo

        if (_tickTimer >= tickTime)
        {
            _tickTimer -= tickTime; // NÃO zera, subtrai para estabilidade
            TickEvent();
        }
    }

    private void TickEvent()
    {
        OnTickAction?.Invoke();
    }
}
