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
    private SpriteRenderer _spriteRenderer;

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
    private int _animationFrame;

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
        // Get a reference to the sprite renderer so we can animate the sprite
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = this.animationSprites[0];
    }

    private void Start()
    {
        // Start an animation loop to cycle between sprites
        InvokeRepeating(nameof(AnimateSprite), this.animationTime, this.animationTime);
    }

    private void AnimateSprite()
    {
        // Move to the next animation frame
        _animationFrame++;

        // Loop back to the start if the animation frame exceeds the length
        if (_animationFrame >= this.animationSprites.Length) {
            _animationFrame = 0;
        }

        // Set the sprite based on the current animation frame
        _spriteRenderer.sprite = this.animationSprites[_animationFrame];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // The invader dies when hit by the laser
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser")) {
            this.killed?.Invoke(this);
        }
    }

}
