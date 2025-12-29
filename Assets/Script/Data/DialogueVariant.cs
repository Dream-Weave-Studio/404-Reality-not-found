using UnityEngine;

[System.Serializable]
public struct DialogueVariant
{
    [Tooltip("ID della memoria richiesta per sbloccare questo testo (es. 'Knows_Secret_Code')")]
    public string requiredMemoryID;

    [TextArea(3, 10)]
    public string[] alternateLines; // Le frasi alternative da mostrare
}