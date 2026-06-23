using UnityEngine;

public class SlipDamageReaction : InteractionReaction
{
    [Header("Parametri")]
    [SerializeField] private float damageAmount = 15f;
    [SerializeField] private float rageAmount = 10f;
    [SerializeField] private bool giveRage = true;

    private bool hasActivated = false;

    protected override void PerformReaction(GameObject interactor)
    {
        if (hasActivated) return;

        hasActivated = true;

        GameObject player = GameObject.FindWithTag("Player");
        PlayerController playerController = player != null ? player.GetComponent<PlayerController>() : null;
        if (playerController != null)
        {
            playerController.TakeDamage(damageAmount);

            playerController.SlipAndFall();

            Debug.Log($"[SlipDamage] Scivolata! Danno: {damageAmount}, Rabbia: {(giveRage ? rageAmount : 0)}");
        }
    }
}