using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    [Header("References")]
    public PlayerManager playerManager;
    public Image fillImage;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (playerManager != null)
        {
            playerManager.life.OnValueChanged += OnLifeChanged;
            UpdateBar(playerManager.life.Value);
        }
    }

    public void UpdateBarColor(bool isMain)
    {
        if (fillImage != null)
        {
            if (isMain)
            {
                fillImage.color = Color.green;
            }
            else
            {
                fillImage.color = Color.red;
            }
        }
    }

    void OnDestroy()
    {
        if (playerManager != null)
            playerManager.life.OnValueChanged -= OnLifeChanged;
    }

    void OnLifeChanged(int previousValue, int newValue)
    {
        UpdateBar(newValue);
    }

    void UpdateBar(int lifeValue)
    {
        if (fillImage != null)
        {

            fillImage.fillAmount = Mathf.Clamp01(((float)lifeValue )/ playerManager.networkMaxLife.Value);
        }
    }

    void LateUpdate()
    {
        if (mainCamera != null)
            transform.forward = mainCamera.transform.forward;
    }
}