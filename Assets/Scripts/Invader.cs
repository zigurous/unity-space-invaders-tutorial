using UnityEngine;

/// <summary>
/// Handles animating the invader sprite and detecting when the invader dies.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class Invader : MonoBehaviour
{
    /// <summary>
    /// The sprite renderer component of the invader.
    /// </summary>
    public SpriteRenderer spriteRenderer { get; private set; }

    /// <summary>
    /// The sprites that are animated on the invader.
    /// </summary>
    [Tooltip("The sprites that are animated on the invader.")]
    public Sprite[] animationSprites = new Sprite[0];

    /// <summary>
    /// The amount of seconds between switching sprites to simulate animation.
    /// </summary>
    [Tooltip("The amount of seconds between switching sprites to simulate animation.")]
    public float animationTime = 1.0f;

    /// <summary>
    /// The index of the current sprite being rendered.
    /// </summary>
    public int animationFrame { get; private set; }

    /// <summary>
    /// The amount of points the invader is worth when killed.
    /// </summary>
    [Tooltip("The amount of points the invader is worth when killed.")]
    public int score = 10;

    /// <summary>
    /// The callback invoked when the invader is killed.
    /// </summary>
    public System.Action<Invader> killed;

    private void Awake()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.spriteRenderer.sprite = this.animationSprites[0];
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), this.animationTime, this.animationTime);
    }

    private void AnimateSprite()
    {
        this.animationFrame++;

        // Loop back to the start if the animation frame exceeds the length
        if (this.animationFrame >= this.animationSprites.Length) {
            this.animationFrame = 0;
        }

        this.spriteRenderer.sprite = this.animationSprites[this.animationFrame];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser")) {
            this.killed?.Invoke(this);
        }
    }

}
