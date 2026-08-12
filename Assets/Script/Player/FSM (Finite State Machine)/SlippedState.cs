using UnityEngine;

public class SlippedState : IPlayerState
{
    private readonly PlayerController player;
    private float timer;
    private bool isRising = false;
    private Quaternion originalRotation;
    private Vector3 originalPosition;

    private Vector3 fallPosition;
    private Quaternion fallRotation;

    private Rigidbody rb;
    private bool wasKinematic;
    private bool originalUseGravity;

    private const float fallHeightOffset = 0.18f;
    private const float rotationLerpDuration = 0.8f;

    public SlippedState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        Debug.Log("[State] Entrato nello stato SLIPPED");
        timer = 0f;
        isRising = false;

        rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            wasKinematic = rb.isKinematic;
            originalUseGravity = rb.useGravity;

            rb.isKinematic = true;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        CapsuleCollider collider = player.GetComponent<CapsuleCollider>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("Blend Tree");
            animator.speed = 0.3f;
        }

        originalPosition = player.transform.position;
        originalRotation = player.transform.rotation;

        player.transform.rotation = originalRotation * Quaternion.Euler(-90f, 0f, 0f);

        player.transform.position = originalPosition + new Vector3(0f, fallHeightOffset, 0f);
    }

    public void Update()
    {
        if (isRising)
        {
            timer += Time.deltaTime;
            
            float progress = Mathf.Clamp01(timer / rotationLerpDuration);
            float t = Mathf.SmoothStep(0f, 1f, progress);

            player.transform.position = Vector3.Lerp(fallPosition, originalPosition, t);
            player.transform.rotation = Quaternion.Slerp(fallRotation, originalRotation, t);
            
        }
        else
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            {
                isRising = true;
                timer = 0f;

                fallPosition = player.transform.position;
                fallRotation = player.transform.rotation;

                Animator animator = player.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.speed = 1f;
                    animator.CrossFade("Standup", 0.4f);
                }
                else
                {
                    player.TransitionToState(player.idleState);
                }
            }
        }
    }

    public void Exit()
    {
        player.transform.position = originalPosition;
        player.transform.rotation = Quaternion.Euler(0f, player.transform.rotation.eulerAngles.y, 0f);

        Animator animator = player.GetComponent<Animator>();
        if (animator != null)
        {
            animator.speed = 1f;
        }

        CapsuleCollider collider = player.GetComponent<CapsuleCollider>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        if (rb != null)
        {
            rb.isKinematic = wasKinematic;
            rb.useGravity = originalUseGravity;
        }

        Debug.Log("[State] Uscito dallo stato SLIPPED");
    }
}
