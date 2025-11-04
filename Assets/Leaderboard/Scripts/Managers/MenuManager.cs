using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;

public class MenuManager : MonoBehaviour
{
    
    private bool initialized = false;
    private bool eventsInitialized = false;
    
    private static MenuManager singleton = null;

    public static MenuManager Singleton
    {
        get
        {
            if (singleton == null)
            {
                singleton = FindFirstObjectByType<MenuManager>();
                singleton.Initialize();
            }
            return singleton; 
        }
    }

    private void Awake()
    {
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }
        
        DontDestroyOnLoad(gameObject);
    }

    private void Initialize()
    {
        if (initialized) { return; }
        initialized = true;
    }
    
    private void OnDestroy()
    {
        if (singleton == this)
        {
            singleton = null;
        }
    }

    private void Start()
    {
        Application.runInBackground = true;
        
        // Canvas 즉시 활성화
        Canvas[] allCanvas = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log("Canvas 찾음: " + allCanvas.Length + "개");
        
        foreach (Canvas canvas in allCanvas)
        {
            canvas.gameObject.SetActive(true);
            Debug.Log("Canvas 활성화: " + canvas.gameObject.name);
        }
        
        // 코루틴으로 지연 실행
        StartCoroutine(InitializeServices());
    }

    private IEnumerator InitializeServices()
    {
        yield return null;
        yield return null;
        
        // PanelManager 초기화 확인
        Debug.Log("PanelManager Singleton 접근");
        var pm = PanelManager.Singleton;
        Debug.Log("PanelManager 준비 완료");
        
        yield return null;
        
        PanelManager.CloseAll();
        PanelManager.Open("loading");
        
        yield return StartCoroutine(StartClientServiceCoroutine());
    }

    // 외부에서 호출할 수 있는 public 메서드
    public void StartClientService()
    {
        StartCoroutine(StartClientServiceCoroutine());
    }

    private IEnumerator StartClientServiceCoroutine()
    {
        // try/catch 밖에서 yield return 처리
        bool initialized = false;
        Exception initException = null;

        // Unity Services 초기화 (try/catch 제거)
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            var options = new InitializationOptions();
            options.SetProfile("default_profile");
            
            var initTask = UnityServices.InitializeAsync();
            while (!initTask.IsCompleted)
            {
                yield return null;
            }
            
            if (initTask.IsFaulted)
            {
                initException = initTask.Exception;
                initialized = false;
            }
            else
            {
                initialized = true;
            }
        }
        else
        {
            initialized = true;
        }

        yield return null;

        // 결과 처리
        if (initialized)
        {
            if (!eventsInitialized)
            {
                SetupEvents();
            }

            PanelManager.CloseAll();
            PanelManager.Open("start");
            Debug.Log("메뉴 표시됨");
        }
        else
        {
            Debug.LogError("초기화 실패: " + (initException != null ? initException.Message : "Unknown error"));
            ShowError(ErrorMenu.Action.StartService, "Failed to connect to the network.", "Retry");
        }
    }
    
    public async void SignInWithUsernameAndPasswordAsync(string username, string password)
    {
        PanelManager.Open("loading");
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
        }
        catch (AuthenticationException exception)
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Username or password is wrong.", "OK");
        }
        catch (RequestFailedException exception)
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to connect to the network.", "OK");
        }
    }
    
    public async void SignUpWithUsernameAndPasswordAsync(string username, string password)
    {
        PanelManager.Open("loading");
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
        }
        catch (AuthenticationException exception)
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to sign you up.", "OK");
        }
        catch (RequestFailedException exception)
        {
            ShowError(ErrorMenu.Action.OpenAuthMenu, "Failed to connect to the network.", "OK");
        }
    }
    
    public void SignOut()
    {
        AuthenticationService.Instance.SignOut();
        PanelManager.CloseAll();
        PanelManager.Open("start");
    }
    
    private void SetupEvents()
    {
        eventsInitialized = true;
        AuthenticationService.Instance.SignedIn += () =>
        {
            SignInConfirmAsync();
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            PanelManager.CloseAll();
            PanelManager.Open("start");
        };
        
        AuthenticationService.Instance.Expired += () =>
        {
            SignInWithUsernameAndPasswordAsync("", "");
        };
    }
    
    private void ShowError(ErrorMenu.Action action = ErrorMenu.Action.None, string error = "", string button = "")
    {
        PanelManager.Close("loading");
        ErrorMenu panel = (ErrorMenu)PanelManager.GetSingleton("error");
        if (panel != null)
        {
            panel.Open(action, error, button);
        }
    }
    
    private async void SignInConfirmAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(AuthenticationService.Instance.PlayerName))
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync("Player");
            }
            PanelManager.CloseAll();
            GameState.Instance.SetState(GameState.State.Menu);
            PanelManager.Open("main");
        }
        catch
        {
            
        }
    }
    
}