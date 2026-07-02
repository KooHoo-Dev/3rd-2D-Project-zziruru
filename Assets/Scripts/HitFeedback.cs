using UnityEngine;
using System.Collections;

public class HitFeedback : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hitColor = Color.white;
    [SerializeField] private float flashDuration = 0.08f;
    [SerializeField] private float deathFadeDuration = 0.25f;

    private Color originalColor;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        // if (spriteRenderer == null)
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        originalColor = spriteRenderer.color;
    }

    public void PlayHitFlash()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }

    public IEnumerator PlayDeathFade()
    {
        float timer = 0f;
        Color startColor = spriteRenderer.color;

        while (timer < deathFadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / deathFadeDuration;
            spriteRenderer.color = new Color(
                startColor.r,
                startColor.g,
                startColor.b,
                Mathf.Lerp(startColor.a, 0f, t)
            );
            Vector3 startScale = transform.localScale;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            yield return null;
        }
    }
}
