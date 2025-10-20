using System;
using System.Collections.Generic;
using System.Linq;
using _01.Member.KMJ._02.Scripts._02.System._01.BloodFlower;
using Code.Core.Debugs;
using UnityEngine;
using UnityEngine.UI;

public class BloodFlowerUI : MonoBehaviour
{
    [SerializeField] private float increasingValue = 1;
    public List<Image> bloodFlowerUIs = new List<Image>();
    
    public float _flowerValue = 0;

    private float _modifierflowerValue = 1;


    private void Awake()
    {
        GetComponentsInChildren<Image>().ToList().ForEach(UI => bloodFlowerUIs.Add(UI));
    }

    private void Update()
    {
        LerpUIValue();
        SetFlower();
    }

    private void LerpUIValue()
    {
        _flowerValue = Mathf.Lerp(_flowerValue, _modifierflowerValue, Time.deltaTime * increasingValue);
    }

    public void SetUIValue(float value)
    {
        _modifierflowerValue = value;
    }

    public void SetFlower()
    {
        switch (_flowerValue)
        {
            case <= 0:
                bloodFlowerUIs[0].fillAmount = Mathf.Clamp01(_flowerValue / 3f);
                break;

            case <= 3:
                bloodFlowerUIs[0].fillAmount = 1f;
                bloodFlowerUIs[1].fillAmount = Mathf.Clamp01((_flowerValue - 3f * 0f) / 3f);
                break;

            case <= 6:
                bloodFlowerUIs[0].fillAmount = 1f;
                bloodFlowerUIs[1].fillAmount = 1f;
                bloodFlowerUIs[2].fillAmount = Mathf.Clamp01((_flowerValue - 3f * 2f / 2f) / 3f);
                bloodFlowerUIs[2].fillAmount = Mathf.Clamp01((_flowerValue - 3f) / 3f);
                break;

            case <= 9:
                bloodFlowerUIs[0].fillAmount = 1f;
                bloodFlowerUIs[1].fillAmount = 1f;
                bloodFlowerUIs[2].fillAmount = 1f;
                bloodFlowerUIs[3].fillAmount = Mathf.Clamp01((_flowerValue - 6f) / 3f);
                break;

            default: 
                foreach (var img in bloodFlowerUIs)
                    img.fillAmount = 1f;
                break;   
        }
    }
}
