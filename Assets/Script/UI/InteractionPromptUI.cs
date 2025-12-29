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

    private Transform targetTransform; // L'oggetto che stiamo guardando (es. il Telefono)
    private Camera mainCam;

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
        // 1. Controlli di sicurezza base
        if (promptPanel == null) return;
        if (targetTransform == null || !targetTransform.gameObject.activeInHierarchy)
        {
            HidePrompt();
            return;
        }

        // 2. FIX CAMERA PERSA: Se la camera è nulla (cambio scena), ritroviamola!
        if (mainCam == null) mainCam = Camera.main;

        // Se ancora non c'è una camera (es. caricamento in corso), esci
        if (mainCam == null) return;

        // 3. Posizionamento
        if (promptPanel.activeSelf)
        {
            Vector3 screenPos = mainCam.WorldToScreenPoint(targetTransform.position + offset);
            panelRect.position = screenPos;
        }
    }

    // Chiamata dal PlayerInteraction
    public void ShowPrompt(InteractableObject interactableObj)
    {
        if (interactableObj == null) return;

        targetTransform = interactableObj.transform; // Agganciamo il target
        objectNameText.text = interactableObj.interactableData.displayName;

        promptPanel.SetActive(true);

        // Aggiorna icona tasto iniziale
        if (InputManager.Instance != null)
            UpdateButtonIcon(InputManager.Instance.currentInputType);
    }

    public void HidePrompt()
    {
        targetTransform = null; // Sganciamo il target
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