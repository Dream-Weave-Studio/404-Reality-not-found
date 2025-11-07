using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager Instance { get; private set; }

    // Enum stato gioco
    public enum GameState { MainMenu, Gameplay, Paused, Cutscene }
    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        // Integrazione Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Metodo stato gioco
    public void SetGameState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log("Game State in: " + CurrentState);
    }

    // Metodo Caricare scene
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Coroutine per la regia della Scena 1
    public IEnumerator StartScene1Sequence()
    {
        SetGameState(GameState.Cutscene);

        // 1. Mostra la lettera dell’Hacker tramite DialogSystem
        yield return DialogSystem.Instance.ShowLetter("Salve Ryo... il gioco ha inizio!");

        // 2. Aspetta che il giocatore chiuda la lettera
        yield return new WaitUntil(() => DialogSystem.Instance.IsLetterClosed);

        // 3. Avvia dissolvenza (qui ipotizziamo un FadeManager)
        if (FadeManager.Instance != null)
        {
            yield return FadeManager.Instance.FadeOut();
            yield return FadeManager.Instance.FadeIn();
        }

        // 4. Imposta lo stato su Gameplay
        SetGameState(GameState.Gameplay);
    }

    private void Start()
    {
        // Avvio automatico solo se sei nella Scena 1
        if (SceneManager.GetActiveScene().name == "Appartamento")
        {
            StartCoroutine(StartScene1Sequence());
        }
    }
}

