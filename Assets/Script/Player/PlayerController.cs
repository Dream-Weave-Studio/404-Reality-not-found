using System.Collections;
using UnityEngine;

public class PlayerController : GameEntity
{
    [SerializeField] private MovementComponent movementComponent;
    [SerializeField] private LoadingText loadingText;

    #region Variabili e componenti

    private bool canStandUp = false;
    public Sprite ryoAngryFace; // Per il dialogo di risposta

    private StateMachineController stateMachine;
    public IdleState idleState { get; private set; }
    public WalkingState walkingState { get; private set; }
    public RunningState runningState { get; private set; }
    public SittingState sittingState { get; private set; }
    public SlippedState slippedState { get; private set; }

    private int blendHash;

    #endregion

    #region Unity Methods (Start, Update)

    protected override void Start()
    {
        base.Start();

        if (InputManager.Instance == null)
        {
            Debug.LogError("InputManager.Instance è null!");
            return;
        }

        if (movementComponent == null)
        {
            Debug.LogError("MovementComponent non è assegnato nell'Inspector!");
            return;
        }

        idleState = new IdleState(this);
        walkingState = new WalkingState(this);
        runningState = new RunningState(this);
        sittingState = new SittingState(this);
        slippedState = new SlippedState(this);

        stateMachine = new StateMachineController();

        // Scegli lo stato iniziale in base alla fase di gioco:
        // - Se l'intro non è ancora finita, il player parte seduto
        // - Altrimenti (load di salvataggio) parte in piedi
        bool startSitting = GameManager.Instance != null && !GameManager.Instance.introFinished;

        // Comunica all'Animator quale stato scegliere
        if (animator != null)
            animator.SetBool("SkipIntro", !startSitting);

        stateMachine.Initialize(startSitting ? (IPlayerState)sittingState : idleState);

        blendHash = Animator.StringToHash("Blend");
    }

    void Update()
    {
        stateMachine.UpdateState();

        if (animator != null && movementComponent != null)
        {
            float normalized = 0f;
            if (movementComponent.MaxSpeed > 0f)
                normalized = Mathf.Clamp01(movementComponent.CurrentSpeed / movementComponent.MaxSpeed);

            animator.SetFloat(blendHash, normalized);
        }
    }



    #endregion

    #region Gestione Animazione StandUp

    /// <summary>
    /// Chiamato da SittingState quando il player preme WASD.
    /// Attiva il trigger sull'Animator e mostra il dialogo.
    /// </summary>
    public void TriggerStandUp()
    {
        DialogManager.Instance.ShowDialog(
            "FOTTITI! ASSISTENTE DI MERDA! Ma dove ho lasciato il telefono ieri?",
            ryoAngryFace
        );

        if (animator != null)
            animator.SetTrigger("StandUp");
    }

    /// <summary>
    /// Chiamato dall'Animation Event sul frame finale della clip "StandUp".
    /// Solo qui il movimento viene sbloccato.
    /// </summary>
    public void OnStandUpAnimationFinished()
    {
        animator.SetBool("SkipIntro", false);

        if (stateMachine.GetCurrentStateName() == "SlippedState")
        {
            TransitionToState(idleState);
            return;
        }

        TransitionToState(idleState); // ← Ora il player può muoversi
        loadingText.StopLoading();
        GameManager.Instance.EndIntro();
        QuestManager.Instance.StartQuest(QuestManager.Instance.startingQuest);
    }

    public void SlipAndFall()
    {
        TransitionToState(slippedState);
    }

    public void SetBlendTree()
    {
        animator.SetTrigger("SetBlendTree");
    }

    #endregion

    #region Gestione Stati

    public void TransitionToState(IPlayerState newState)
    {
        stateMachine.ChangeState(newState);
    }

    #endregion

    #region Proxy MovementComponent

    public bool HasMovementInput() => movementComponent.HasMovementInput();
    public bool IsRunningInput() => movementComponent.IsRunningInput();
    public void HandleIsometricMovement() => movementComponent.HandleIsometricMovement();

    #endregion
}