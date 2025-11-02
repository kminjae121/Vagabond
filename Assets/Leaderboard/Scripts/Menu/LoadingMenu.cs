using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingMenu : Panel
{
    [SerializeField] private TextMeshProUGUI loadingText = null;
    private Coroutine loadingCoroutine = null;

    public override void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }
        base.Initialize();
    }

    public override void Open()
    {
        base.Open();
        if (loadingCoroutine != null)
        {
            StopCoroutine(loadingCoroutine);
        }
        loadingCoroutine = StartCoroutine(AnimateLoadingText());
    }

    public override void Close()
    {
        if (loadingCoroutine != null)
        {
            StopCoroutine(loadingCoroutine);
            loadingCoroutine = null;
        }
        base.Close();
    }

    private IEnumerator AnimateLoadingText()
    {
        if (loadingText == null)
        {
            yield break;
        }

        while (true)
        {
            loadingText.text = "Loading.";
            yield return new WaitForSeconds(0.5f);

            loadingText.text = "Loading. .";
            yield return new WaitForSeconds(0.5f);

            loadingText.text = "Loading. . .";
            yield return new WaitForSeconds(0.5f);
        }
    }
}