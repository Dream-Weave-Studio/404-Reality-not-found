using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Definiamo gli stati del gioco possibili nella Scena 1
    public enum GameState
    {
        Initialization, // Caricamento scena
        IntroSequence,  // Schermo nero + Lettera Hacker [cite: 104]
        Gameplay,       // Ryo si muove e interagisce
        Cutscene,       // Eventi bloccanti (es. Telefonata Capo [cite: 198])
        Paused          // Menu di pausa
    }

    [Header("Debug Info")]
    public GameState currentState;
    public GameState startState;

    // Eventi a cui altri script possono iscriversi (es. la UI o il PlayerInput)
    public static event Action<GameState> OnStateChanged;

    private void Awake()
    {
        // Implementazione Singleton: mi assicuro ce ne sia solo uno
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sopravvive al cambio scene
        }
        else
        {
            Destroy(gameObject);
        }

        // Appena parte il gioco, avvia la sequenza intro
        ChangeState(GameState.IntroSequence);
    }

    private void Start()
    {
        
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log($"Game State cambiato in: {newState}");

        // Notifica tutti i sistemi (UI, Input, Audio) che lo stato è cambiato
        OnStateChanged?.Invoke(newState);

        switch (currentState)
        {
            case GameState.IntroSequence:
            // Non serve fare nulla qui, l'IntroController si attiverà da solo
            // leggendo lo stato nel suo Start()
            break;
            case GameState.Gameplay:
                // Sblocca input giocatore
                Time.timeScale = 1;
                break;
            case GameState.Paused:
                // Blocca il tempo
                Time.timeScale = 0;
                break;
        }
    }

    public void EndIntro()
    {
        // 1. Cambiamo stato per sbloccare i movimenti del player
        ChangeState(GameState.Gameplay);

        // 2. Impostiamo il primo obiettivo
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.SetObjective("TROVA IL TELEFONO");
        }
        else
        {
            Debug.LogWarning("ObjectiveManager mancante nella scena!");
        }
    }
}