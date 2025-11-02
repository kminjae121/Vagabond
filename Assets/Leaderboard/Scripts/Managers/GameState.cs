using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public enum State
    {
        Menu,
        InGame,
        Paused
    }

    private State currentState = State.Menu;
    private static GameState instance = null;

    public static GameState Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameState>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameState");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<GameState>();
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

    public void SetState(State newState)
    {
        currentState = newState;
        Debug.Log("게임 상태 변경: " + newState);
    }

    public State GetState()
    {
        return currentState;
    }

    public bool IsInGame()
    {
        return currentState == State.InGame;
    }
}