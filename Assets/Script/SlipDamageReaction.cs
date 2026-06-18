using UnityEngine;

/// <summary>
/// Reazione che applica danno e rabbia UNA SOLA VOLTA.
/// Da usare insieme a InteractableObject o come trigger.
/// </summary>
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
        GameEntity entity = player != null ? player.GetComponent<GameEntity>() : null;
        if (entity != null)
        {
            // Applica effetti
            entity.TakeDamage(damageAmount);
            // if (giveRage) entity.ModifyRage(rageAmount);

            Debug.Log($"[SlipDamage] Scivolata! Danno: {damageAmount}, Rabbia: {(giveRage ? rageAmount : 0)}");
        }
    }
}