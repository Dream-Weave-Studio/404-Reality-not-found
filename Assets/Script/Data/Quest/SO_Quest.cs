using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SubObjectiveData
{
    public string id;
    public string displayName;

    [Tooltip("Evento che sblocca questa missione. Es: saw_takeaway, opened_frigo, found_keys...")]
    public string unlockedByEventID;
}

[CreateAssetMenu(fileName = "New Quest", menuName = "404 Reality/Quest")]
public class SO_Quest : ScriptableObject
{
    [Header("Missione Principale")]
    public string questID;
    public string mainDisplayName;

    [Header("Sotto-obiettivi (tutti event-driven)")]
    [Tooltip("L'ordine in lista non importa: ognuno si attiva solo quando arriva il suo evento.")]
    public List<SubObjectiveData> subObjectives;
}