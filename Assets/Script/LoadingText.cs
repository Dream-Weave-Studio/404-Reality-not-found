using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingText : MonoBehaviour
{
    [SerializeField] private TMP_Text loadingText;

    private Coroutine loadingRoutine;

    private void Start()
    {
        loadingRoutine = StartCoroutine(LoadingRoutine());
    }

    public void StartLoading()
    {
        // Evita di avviare più coroutine contemporaneamente
        if (loadingRoutine != null)
            return;

        loadingRoutine = StartCoroutine(LoadingRoutine());
    }

    public void StopLoading()
    {
        Debug.Log("STOP LOADING");

        if (loadingRoutine != null)
        {
            StopCoroutine(loadingRoutine);
            loadingRoutine = null;
        }

        loadingText.text = "";
    }

    private IEnumerator LoadingRoutine()
    {
        int dots = 0;

        while (true)
        {
            loadingText.text = "Attesa" + new string('.', dots);

            dots = (dots + 1) % 4;

            yield return new WaitForSeconds(0.4f);
        }
    }
}