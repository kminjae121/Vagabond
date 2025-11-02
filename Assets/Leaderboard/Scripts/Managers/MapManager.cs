using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private MapData[] maps = null;
    private MapData currentSelectedMap = null;
    private static MapManager instance = null;

    public static MapManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<MapManager>();
                if (instance == null)
                {
                    instance = new GameObject("MapManager").AddComponent<MapManager>();
                }
            }
            return instance;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public MapData[] GetAllMaps()
    {
        return maps;
    }

    public MapData GetMapById(string mapId)
    {
        foreach (MapData map in maps)
        {
            if (map.mapId == mapId)
            {
                return map;
            }
        }
        return null;
    }

    public void SelectMap(MapData map)
    {
        currentSelectedMap = map;
    }

    public MapData GetSelectedMap()
    {
        return currentSelectedMap;
    }

    public void PlayMap(MapData map)
    {
        currentSelectedMap = map;
        GameState.Instance.SetState(GameState.State.InGame);
        UnityEngine.SceneManagement.SceneManager.LoadScene(map.sceneName);
    }
}