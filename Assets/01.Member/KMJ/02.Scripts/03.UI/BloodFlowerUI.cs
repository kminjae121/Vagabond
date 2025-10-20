  using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BloodFlowerUI : MonoBehaviour
{
    [Header("Lerp 속도 조절")]
    [SerializeField] private float increasingValue = 5f;

    [Header("UI 꽃잎 리스트 (자동 할당됨)")]
    public List<Image> bloodFlowerUIs = new List<Image>();

    [SerializeField] private List<float> _flowerCountList;
    
    private float _flowerValue = 1;         
    private float _modifierFlowerValue = 1;
    
    private void Awake()
    {
        bloodFlowerUIs = GetComponentsInChildren<Image>().ToList();
    }

    private void Update()
    {
        LerpUIValue();
        SetFlower(); 
    }

    private void LerpUIValue()
    {
        _flowerValue = Mathf.Lerp(_flowerValue, _modifierFlowerValue, Time.deltaTime * increasingValue);
    }

    public void SetUIValue(float value)
    {
        _modifierFlowerValue = Mathf.Clamp(value, 0, 10); 
    }

    private void SetFlower()
    {
        if (bloodFlowerUIs == null || bloodFlowerUIs.Count < 4)
            return;


        for (int i = 0; i < bloodFlowerUIs.Count; i++)
            bloodFlowerUIs[i].fillAmount = 0;
        

        switch (_flowerValue)
        {
            case <= 3f: 
                bloodFlowerUIs[0].fillAmount = Mathf.InverseLerp(0, 3f, _flowerValue);
                break;

            case <= 6f: 
                bloodFlowerUIs[0].fillAmount = 1f;
                bloodFlowerUIs[1].fillAmount = Mathf.InverseLerp(3f, 6f, _flowerValue);
                break;

            case <= 9f: 
                bloodFlowerUIs[0].fillAmount = 1f;
                bloodFlowerUIs[1].fillAmount = 1f;
                bloodFlowerUIs[2].fillAmount = Mathf.InverseLerp(6f, 9f, _flowerValue);
                break;

            default:
                bloodFlowerUIs[0].fillAmount = 1f;
                bloodFlowerUIs[1].fillAmount = 1f;
                bloodFlowerUIs[2].fillAmount = 1f;
                bloodFlowerUIs[3].fillAmount = Mathf.InverseLerp(9f, 10f, _flowerValue);
                break;
        }
    }
}
