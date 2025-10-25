using UnityEngine;
using System.Collections;

public class MaskController : MonoBehaviour
{
    public Material targetMaterial;
    public float defaultValue = 2.0f;
    public float targetValue = 0.56f;
    public float downDuration = 0.2f;
    public float upDuration = 0.1f;

    private const string MASK_PROPERTY_NAME = "_Mask_Size";
    private int _maskSizeID;
    private bool _isAnimating = false;
    private bool _isMaskActive = false; // false = defaultValue 상태, true = targetValue 상태

    void Start()
    {
        _maskSizeID = Shader.PropertyToID(MASK_PROPERTY_NAME);

        if (targetMaterial == null)
        {
            Debug.LogError("Target Material (Asset)이 할당되지 않았습니다! 인스펙터에서 지정해주세요.", this);
            this.enabled = false;
            return;
        }

        targetMaterial.SetFloat(_maskSizeID, defaultValue);
        _isMaskActive = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleMaskAnimation();
        }
    }

    public void ToggleMaskAnimation()
    {
        if (_isAnimating || targetMaterial == null)
        {
            return;
        }

        if (_isMaskActive)
        {
            StartCoroutine(AnimateMaskUp());
        }
        else
        {
            StartCoroutine(AnimateMaskDown());
        }
    }

    private IEnumerator AnimateMaskDown()
    {
        _isAnimating = true;
        float elapsedTime = 0f;
        float startValue = targetMaterial.GetFloat(_maskSizeID);

        while (elapsedTime < downDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / downDuration;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            float currentValue = Mathf.Lerp(startValue, targetValue, smoothT);
            targetMaterial.SetFloat(_maskSizeID, currentValue);
            yield return null;
        }
        targetMaterial.SetFloat(_maskSizeID, targetValue);
        
        _isMaskActive = true;
        _isAnimating = false;
    }

    private IEnumerator AnimateMaskUp()
    {
        _isAnimating = true;
        float elapsedTime = 0f;
        float startValue = targetMaterial.GetFloat(_maskSizeID);

        while (elapsedTime < upDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / upDuration;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            float currentValue = Mathf.Lerp(startValue, defaultValue, smoothT);
            targetMaterial.SetFloat(_maskSizeID, currentValue);
            yield return null;
        }
        targetMaterial.SetFloat(_maskSizeID, defaultValue);
        
        _isMaskActive = false;
        _isAnimating = false;
    }

    void OnDestroy()
    {
        ResetMaterialValue();
    }

    void OnApplicationQuit()
    {
        ResetMaterialValue();
    }

    private void ResetMaterialValue()
    {
        if (targetMaterial != null)
        {
            targetMaterial.SetFloat(_maskSizeID, defaultValue);
        }
    }
}