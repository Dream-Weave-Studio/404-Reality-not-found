using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class RageSystem : MonoBehaviour
{
    public float maxRage = 100f;   //valore massimo rabbia

    [SerializeField] private float currentRage = 0f;  //valore corrente della rabbia serializzato per vedere se lo script funziona

    public LayerMask rageTriggerLayers;   //layer che permettono di far salire la rabbia

    public LayerMask rageTriggerLayerMinus; //layer che permettono di far scendere la rabbia

    public float aumentoPerTrigger = 10f;  //valore che viene sommato a ogni entrata nel layer specifico

    public float diminuzionePerTrigger = 10f; //valore che viene sotratto a ogni entrata nel layer specifico

    private HashSet<int> usedLayers = new HashSet<int>(); //per impostare i layer con cui si è già interagito per evitare bug nel gioco (potrebbe essere modificato in un altro modo per evitare di creare mille layer)
    public void IncrementoRage(float amount)     //metodo che incremnta la rabbia
    {
        currentRage += amount;
        currentRage = Mathf.Clamp(currentRage, 0f, maxRage);
        Debug.Log($"Rabbia attuale: {currentRage}");   //questo andrà sostituito poi con la visualizzazione trammite barra HUD
    }

    public void VerificaIncrementoRage(GameObject other) //metodo che verifica il layer degli oggetti e se è già stato interagito non lo fa più usare
    {
        int otherLayer = other.layer;

        if (((1 << otherLayer) & rageTriggerLayers.value) != 0 && !usedLayers.Contains(otherLayer))
        {
            IncrementoRage(aumentoPerTrigger);
            usedLayers.Add(otherLayer);
        }
        else if (((1 << otherLayer) & rageTriggerLayerMinus.value) != 0 && !usedLayers.Contains(otherLayer))
        {
            IncrementoRage(-diminuzionePerTrigger);
            usedLayers.Add(otherLayer);
        }
    }


    private void OnTriggerEnter(Collider other) //per l'entrata nel trigger e richiama il metodo per la validazione
    {
        VerificaIncrementoRage(other.gameObject);
    }
}