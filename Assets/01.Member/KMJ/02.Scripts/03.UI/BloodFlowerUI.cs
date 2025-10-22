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
        _modifierFlowerValue = Mathf.Clamp(value, 0, 1000); 
    }

    private void SetFlower()
    {
        if (bloodFlowerUIs == null || bloodFlowerUIs.Count < 4)
            return;


        for (int i = 0; i < bloodFlowerUIs.Count; i++)
            bloodFlowerUIs[i].fillAmount = 0;
        

        switch (_flowerValue)
        {
            case <= 100f: 
                bloodFlowerUIs[0].fillAmount = Mathf.InverseLerp(0, 100f, _flowerValue) - 0.2f;
                break;

            case <= 200f: 
                bloodFlowerUIs[0].fillAmount = 0.8f;
                bloodFlowerUIs[1].fillAmount = Mathf.InverseLerp(100f, 200f, _flowerValue) - 0.2f;
                break;

            case <= 300f: 
                bloodFlowerUIs[0].fillAmount =  0.8f;
                bloodFlowerUIs[1].fillAmount =  0.8f;
                bloodFlowerUIs[2].fillAmount = Mathf.InverseLerp(200f, 300f, _flowerValue) - 0.2f;
                break;
            case <= 400f: 
                bloodFlowerUIs[0].fillAmount =  0.8f;
                bloodFlowerUIs[1].fillAmount =  0.8f;
                bloodFlowerUIs[2].fillAmount = 0.8f;
                bloodFlowerUIs[3].fillAmount = Mathf.InverseLerp(300f, 400f, _flowerValue) - 0.2f;
                break;
            case <= 500f: 
                bloodFlowerUIs[0].fillAmount =  0.8f;
                bloodFlowerUIs[1].fillAmount =  0.8f;
                bloodFlowerUIs[2].fillAmount = 0.8f;
                bloodFlowerUIs[3].fillAmount = 0.8f;
                bloodFlowerUIs[4].fillAmount = Mathf.InverseLerp(400f, 500f, _flowerValue) - 0.2f;
                break;
            case <= 600f: 
                bloodFlowerUIs[0].fillAmount =  0.8f;
                bloodFlowerUIs[1].fillAmount =  0.8f;
                bloodFlowerUIs[2].fillAmount = 0.8f;
                bloodFlowerUIs[3].fillAmount = 0.8f;
                bloodFlowerUIs[4].fillAmount = 0.8f;
                bloodFlowerUIs[5].fillAmount = Mathf.InverseLerp(500f, 600f, _flowerValue) - 0.2f;
                break;
            case <= 700f: 
                bloodFlowerUIs[0].fillAmount =  0.8f;
                bloodFlowerUIs[1].fillAmount =  0.8f;
                bloodFlowerUIs[2].fillAmount = 0.8f;
                bloodFlowerUIs[3].fillAmount = 0.8f;
                bloodFlowerUIs[4].fillAmount = 0.8f;
                bloodFlowerUIs[5].fillAmount = 0.8f;
                bloodFlowerUIs[6].fillAmount = Mathf.InverseLerp(600f, 700f, _flowerValue) - 0.2f;
                break;
            case <= 800f: 
                bloodFlowerUIs[0].fillAmount =  0.8f;
                bloodFlowerUIs[1].fillAmount =  0.8f;
                bloodFlowerUIs[2].fillAmount = 0.8f;
                bloodFlowerUIs[3].fillAmount = 0.8f;
                bloodFlowerUIs[4].fillAmount = 0.8f;
                bloodFlowerUIs[5].fillAmount = 0.8f;
                bloodFlowerUIs[6].fillAmount = 0.8f;
                bloodFlowerUIs[7].fillAmount = Mathf.InverseLerp(700f, 800f, _flowerValue) - 0.2f;
                break;
            case <= 900f: 
                bloodFlowerUIs[0].fillAmount =  0.8f;
                bloodFlowerUIs[1].fillAmount =  0.8f;
                bloodFlowerUIs[2].fillAmount = 0.8f;
                bloodFlowerUIs[3].fillAmount = 0.8f;
                bloodFlowerUIs[4].fillAmount = 0.8f;
                bloodFlowerUIs[5].fillAmount = 0.8f;
                bloodFlowerUIs[6].fillAmount = 0.8f;
                bloodFlowerUIs[7].fillAmount = 0.8f;
                bloodFlowerUIs[8].fillAmount = Mathf.InverseLerp(800f, 900f, _flowerValue) - 0.2f;
                break;
            default:
                bloodFlowerUIs[0].fillAmount =  0.8f;
                bloodFlowerUIs[1].fillAmount =  0.8f;
                bloodFlowerUIs[2].fillAmount = 0.8f;
                bloodFlowerUIs[3].fillAmount = 0.8f;
                bloodFlowerUIs[4].fillAmount = 0.8f;
                bloodFlowerUIs[5].fillAmount = 0.8f;
                bloodFlowerUIs[6].fillAmount = 0.8f;
                bloodFlowerUIs[7].fillAmount = 0.8f;
                bloodFlowerUIs[8].fillAmount = 0.8f;
                bloodFlowerUIs[9].fillAmount = Mathf.InverseLerp(900f, 800f, _flowerValue) - 0.2f;
                break;
        }
    }
}
