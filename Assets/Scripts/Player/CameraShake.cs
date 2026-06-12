using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        Instance = this;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float force = 0.5f)
    {
        if (impulseSource == null) return;
        impulseSource.GenerateImpulse(force);
    }

    public void Shake(float duration, float amplitude, float frequency)
    {
        Shake(amplitude);
    }
}