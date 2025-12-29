using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Telefono: MonoBehaviour

{
    public GameObject telefonoCellulare;
    public bool contactzone = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (contactzone && Input.GetKeyDown(KeyCode.F))

            Debug.Log("Premi F");
        
    }
   
    public void OnTriggerStay(Collider other)
    {
        telefonoCellulare = other.gameObject;
        
        Debug.Log($"rimango in {telefonoCellulare}");
    }

    public void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("Telefono Cellulare"))
            contactzone = true;
        Debug.Log("Premi F per interagire");
            
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Telefono Cellulare"))
            contactzone = false;
        Debug.Log("...");




    }
        
   
}
