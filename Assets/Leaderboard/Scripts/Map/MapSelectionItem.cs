using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MapSelectionItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mapNameText = null;
    [SerializeField] private Button selectButton = null;
    
    private MapData mapData = null;
    private Action<MapData> onSelectCallback = null;

    private void Start()
    {
        selectButton.onClick.AddListener(SelectMap);
    }

    public void Initialize(MapData map, Action<MapData> callback)
    {
        mapData = map;
        onSelectCallback = callback;
        if (mapNameText != null)
        {
            mapNameText.text = map.mapName;
        }
    }

    private void SelectMap()
    {
        if (onSelectCallback != null)
        {
            onSelectCallback.Invoke(mapData);
        }
    }
}