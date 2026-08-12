using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    [Header("Riferimenti UI In-Game")]
    public GameObject dialogPanel;     
    public Image portraitImage;       
    public TMP_Text dialogText;        

    [Header("Settings")]
    public float typingSpeed = 0.03f;  
    public float displayDuration = 2f;
    public float readingSpeedPerWord = 0.4f; 

    private Coroutine hideCoroutine;
    private Coroutine currentRoutine;

    public bool IsDialogActive { get; private set; } = false;
    public string CurrentFullText { get; private set; } = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {

            Instance.dialogPanel = this.dialogPanel;
            Instance.portraitImage = this.portraitImage;
            Instance.dialogText = this.dialogText;

            if (Instance.dialogPanel != null) Instance.dialogPanel.SetActive(false);

            Destroy(gameObject);
        }
    }

    public void ShowDialog(string text, Sprite portrait)
    {
        CurrentFullText = text;

        dialogPanel.SetActive(true);
        dialogText.text = text;

        if (portrait != null)
        {
            portraitImage.sprite = portrait;
            portraitImage.gameObject.SetActive(true);
        }
        else
        {
            portraitImage.gameObject.SetActive(false); 
        }

        if (currentRoutine != null) StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(TypeTextRoutine(text));
    }

    private float CalculateReadTime(string text)
    {
        int wordCount = text.Split(' ').Length;
        float readTime = wordCount * readingSpeedPerWord;
        return Mathf.Max(readTime, displayDuration);
    }

    private IEnumerator TypeTextRoutine(string textToType)
    {
        IsDialogActive = true;
        dialogText.text = ""; 

        foreach (char letter in textToType.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(CalculateReadTime(textToType));

        CloseDialog();
    }

    public void ShowDialogPersistent(string text, Sprite portrait)
    {
        CurrentFullText = text;
        if (currentRoutine != null) StopCoroutine(currentRoutine);

        dialogPanel.SetActive(true);
        dialogText.text = "";

        if (portrait != null)
        {
            portraitImage.sprite = portrait;
            portraitImage.gameObject.SetActive(true);
        }
        else
        {
            portraitImage.gameObject.SetActive(false);
        }

        currentRoutine = StartCoroutine(TypeTextPersistentRoutine(text));
    }

    private IEnumerator TypeTextPersistentRoutine(string textToType)
    {
        IsDialogActive = true;
        dialogText.text = "";
        foreach (char letter in textToType.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void CloseDialog()
    {
        IsDialogActive = false;
        CurrentFullText = "";
        dialogPanel.SetActive(false);
        dialogText.text = "";
    }
}