using Code.Core;
using Code.Core.Debugs;
using UnityEngine;

public class SceneChangeManager : MonoSingleton<SceneChangeManager>
{
    private string _currentSceneName;
    private string _nextSceneName;
    
    public void SetNextSceneName(string sceneName)
    {
        _nextSceneName = sceneName;
    }

    public void GoLoadingScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("LoadingScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void MoveToNextScene()
    {
        if (_nextSceneName == _currentSceneName)
        {
            UnityLogger.LogWarning("CurrentScene and NextScene is the same");
        }
        
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_nextSceneName);
        _currentSceneName = _nextSceneName;
    }
}
