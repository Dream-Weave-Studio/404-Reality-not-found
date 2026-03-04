using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    public static InteractionPromptUI Instance;

    [Header("Riferimenti UI")]
    public GameObject promptPanel;    // Il pannello intero
    public TMP_Text buttonText;       // [E] o (X)
    public TMP_Text objectNameText;   // "Telefono"
    public RectTransform panelRect;   // Il RectTransform del pannello per muoverlo

    [Header("Settings")]
    public Vector3 offset = new Vector3(0, 2f, 0); // Quanto in alto sopra l'oggetto deve stare (es. 2 metri)

    [Header("References")]
    public Canvas rootCanvas;

    [Header("Target")]
    public Transform player;

    private Transform targetTransform; // L'oggetto che stiamo guardando (es. il Telefono)
    private Camera mainCam;
    private bool isStaticMode = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // NON usare DontDestroyOnLoad per la UI se possibile, 
            // ma se lo usi, devi aggiornare i riferimenti così:
        }
        else
        {
            // Se esiste già un manager UI, aggiornagli i riferimenti
            Instance.promptPanel = this.promptPanel;
            Instance.buttonText = this.buttonText;
            Instance.objectNameText = this.objectNameText;
            Instance.panelRect = this.panelRect;
            Instance.player = this.player;
            Instance.rootCanvas = this.rootCanvas;

            // Spegni il prompt per sicurezza
            Instance.HidePrompt();

            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Iscrizione evento Input (come prima)
        if (InputManager.Instance != null)
            InputManager.Instance.OnInputChanged += UpdateButtonIcon;
    }

    private void LateUpdate()
    {
        if (promptPanel == null || isStaticMode) return;

        if (targetTransform == null || !targetTransform.gameObject.activeInHierarchy)
        {
            HidePrompt();
            return;
        }

        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return;

        if (promptPanel.activeSelf)
        {
            // 1. Converti posizione world in pixel screen
            Vector3 screenPos = mainCam.WorldToScreenPoint(targetTransform.position + offset);

            // 2. Converti pixel screen in coordinate locali del Canvas
            // null è OBBLIGATORIO per Screen Space Overlay
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rootCanvas.GetComponent<RectTransform>(),
                screenPos,
                null,
                out Vector2 localPoint
            );

            panelRect.anchoredPosition = localPoint;
        }
    }

    // Chiamata dal PlayerInteraction
    public void ShowPrompt(InteractableObject interactableObj)
    {
        if (interactableObj == null) return;

        isStaticMode = false;

        targetTransform = player;

        objectNameText.text = interactableObj.interactableData.displayName;
        promptPanel.SetActive(true);

        if (InputManager.Instance != null)
            UpdateButtonIcon(InputManager.Instance.currentInputType);
    }

    public void ShowPromptStatic(string message)
    {
        isStaticMode = true;
        targetTransform = null;

        objectNameText.text = message;  // "Premi E per zittire Lexa" / "Premi X per zittire Lexa"
        panelRect.anchoredPosition = Vector2.zero;
        promptPanel.SetActive(true);

        // Aggiorna subito l'icona con il dispositivo corrente
        if (InputManager.Instance != null)
            UpdateButtonIcon(InputManager.Instance.currentInputType);
    }

    public void HidePrompt()
    {
        isStaticMode = false; // Reset anche della modalità statica
        targetTransform = null; // Sganciamo il target
        objectNameText.text = "";
        promptPanel.SetActive(false);
    }

    private void UpdateButtonIcon(InputManager.InputType type)
    {
        // ... (Stesso codice switch case di prima per le icone) ...
        switch (type)
        {
            case InputManager.InputType.Keyboard:
                buttonText.text = "[E]";
                buttonText.color = Color.yellow;
                break;
            default:
                buttonText.text = "(X)";
                buttonText.color = Color.cyan;
                break;
        }
    }
}