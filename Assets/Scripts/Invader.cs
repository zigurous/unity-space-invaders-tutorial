using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Invader : MonoBehaviour
{
    public SpriteRenderer spriteRenderer { get; private set; }
    public Sprite[] animationSprites = new Sprite[0];
    public float animationTime = 1.0f;
    public int animationFrame { get; private set; }
    public int score = 10;
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
