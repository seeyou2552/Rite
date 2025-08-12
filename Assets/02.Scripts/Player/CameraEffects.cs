using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class CameraEffects : MonoBehaviour
{
    [Header("ī�޶� ����")]
    public Transform camTransform;
    public float shakeIntensity = 0.2f;
    public float shakeDuration = 0.5f;
    private float shakeTimer;
    private Vector3 originalPos;

    [Header("����Ʈ ���μ���")]
    public Volume volume;
    private FilmGrain filmGrain;
    private ColorAdjustments colorAdjustments;
    private float grainTimer;
    private float effectSpeed = 0.5f;
    bool isSuiciding = false;

    void Start()
    {
        if (camTransform == null)
            camTransform = Camera.main.transform;

        originalPos = camTransform.localPosition;

        if (volume != null)
        {
            volume.profile.TryGet(out filmGrain);
            volume.profile.TryGet(out colorAdjustments);

            if (filmGrain != null)
            {
                filmGrain.active = false;
            }

            if (colorAdjustments != null)
            {
                colorAdjustments.active = false;
            }
        }

    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeIntensity;

            if (shakeTimer <= 0f)
                camTransform.localPosition = originalPos;
        }

        if (isSuiciding)
        {
            SuicideEffect();
        }
    }

    public void ShakeCamera(float duration)
    {
        shakeTimer = duration > 0 ? duration : shakeDuration;
    }

    public void ToggleFilmGrain(bool on)
    {
        if (filmGrain == null) return;

        filmGrain.active = on;
    }

    public void PlayChaseBurstEffect()
    {
        SoundManager.Instance.PlaySFX("DetectedSound");
        StartCoroutine(DoChaseBurstEffect());
    }

    private IEnumerator DoChaseBurstEffect()
    {
        ShakeCamera(0.5f);
        ToggleFilmGrain(true); // ���� ������
        colorAdjustments.active = true;

        yield return new WaitForSeconds(0.5f);

        ToggleFilmGrain(false);
    }

    public void EndChaseEffect()
    {
        // �߰� ����: ColorAdjustments ���ֱ�
            colorAdjustments.active = false;
    }

    public void StartSuicideEffect()
    {
        isSuiciding = true;
        volume.weight = 0;
        colorAdjustments.active = true;
        filmGrain.active = true;
        SoundManager.Instance.PlaySFX("BloodSquish");
    }

    public void EndSuicideEffect()
    {
        isSuiciding = false;
        volume.weight = 0.65f;
        colorAdjustments.active = false;
        filmGrain.active = false;
    }

    private void SuicideEffect()
    {
        volume.weight = Mathf.MoveTowards(volume.weight, 0.65f, effectSpeed *Time.deltaTime);

        if (volume.weight >= 0.65f)
        {
            isSuiciding = false; 
        }
    }
}

