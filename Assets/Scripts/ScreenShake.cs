using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public float shakeDuration = 0.3f; 

    public float shakeAmount = 0.7f; 

    private Vector3 originalCamPos;
    private float currentShakeTime;

    void Start()
    {
        originalCamPos = transform.position;
    }
    public void TriggerShake()
    {
        currentShakeTime = shakeDuration;
    }

    void Update()
    {
        if (currentShakeTime > 0)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeAmount;
            transform.position = originalCamPos + randomOffset;

            currentShakeTime -= Time.deltaTime;
        }
        else
        {
            // Reset camera position to original after shake duration
            transform.position = originalCamPos;
        }
    }
}