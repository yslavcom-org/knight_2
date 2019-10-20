using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image foregroundImage;
    [SerializeField]
    private float updateSpeedSeconds = 0.5f;
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private float positionOffset = 2f;

    private Health health;

    #region built-in methods
    private void LateUpdate()
    {
        if (null == camera) return;

        //transform.position = camera.WorldToScreenPoint(health.transform.position + Vector3.up * positionOffset);
        transform.position = health.transform.position + Vector3.up * positionOffset;
        transform.LookAt(camera.transform);
    }

    private void OnDestroy()
    {
        health.OnHealthPctChanged -= HandleHealthChanged;
    }
    #endregion

    #region custom methods

    public void SetHealth(Health health)
    {
        this.health = health;
        health.OnHealthPctChanged += HandleHealthChanged;
    }

    public void SetPositionOffset(float positionOffset)
    {
        this.positionOffset = positionOffset;
    }

    private void HandleHealthChanged(float pct)
    {
        StartCoroutine(ChangeToPct(pct));
    }

    private IEnumerator ChangeToPct(float pct)
    {
        float preChangePct = foregroundImage.fillAmount;
        float elapsed = 0f;

        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            foregroundImage.fillAmount = Mathf.Lerp(preChangePct, pct, elapsed / updateSpeedSeconds);
            yield return null;
        }

        foregroundImage.fillAmount = pct; // do this to set the fixed amount if Lerp went a bit inaccurate
    }

    public void SetTrackingCamera(Camera cam)
    {
        camera = cam;
    }


    #endregion
}
