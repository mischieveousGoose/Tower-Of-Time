using UnityEngine;
using Unity.Cinemachine;
using System.Collections;


public class CameraController : MonoBehaviour
{
    CinemachineCamera virtualCamera;
    bool isFacingRight;

    public GameObject leftTarget;
    public GameObject rightTarget;
    CinemachineBasicMultiChannelPerlin noise;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineCamera>();
        noise = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;
        noise.AmplitudeGain = 0f;
    }

    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            isFacingRight = Input.GetAxisRaw("Horizontal") > 0;
        }

        if (isFacingRight)
        {
            virtualCamera.Follow = rightTarget.transform;
        }
        else
        {
            virtualCamera.Follow = leftTarget.transform;
        }
    }

    public void ScreenShake(float intensity, float duration)
    {
        noise.AmplitudeGain = intensity;
        StartCoroutine(ResetNoiseAfterDuration(noise, duration));

    }

    IEnumerator ResetNoiseAfterDuration(CinemachineBasicMultiChannelPerlin noise, float duration)
    {
        yield return new WaitForSeconds(duration);
        noise.AmplitudeGain = 0f;
    }
    
}
