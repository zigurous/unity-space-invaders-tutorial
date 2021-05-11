using UnityEngine;

/// <summary>
/// Handle bunker destruction simulations.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Bunker : MonoBehaviour
{
    /// <summary>
    /// The texture that will be used as an alpha mask when a projectile splats
    /// on the bunker.
    /// </summary>
    [Tooltip("The texture that will be used as an alpha mask when a projectile splats on the bunker.")]
    public Texture2D splat;

    /// <summary>
    /// The sprite renderer component of the bunker.
    /// </summary>
    private SpriteRenderer _spriteRenderer;

    /// <summary>
    /// The box collider component of the bunker.
    /// </summary>
    private BoxCollider2D _collider;

    /// <summary>
    /// The original texture of the sprite so we can clone it.
    /// </summary>
    private Texture2D _originalTexture;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
        _originalTexture = _spriteRenderer.sprite.texture;

        ResetBunker();
    }

    public void ResetBunker()
    {
        // Each bunker needs a unique instance of the sprite texture since we
        // will be modifying it at the source
        CopyTexture(_originalTexture);

        this.gameObject.SetActive(true);
    }

    private void CopyTexture(Texture2D source)
    {
        // Create a copy of the source texture with the same properties
        Texture2D copy = new Texture2D(source.width, source.height, source.format, false);
        copy.filterMode = source.filterMode;
        copy.alphaIsTransparency = source.alphaIsTransparency;
        copy.anisoLevel = source.anisoLevel;
        copy.wrapMode = source.wrapMode;
        copy.SetPixels(source.GetPixels());
        copy.Apply();

        // Create a new sprite using the copied texture
        Sprite sprite = Sprite.Create(copy, _spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f), _spriteRenderer.sprite.pixelsPerUnit);
        _spriteRenderer.sprite = sprite;
    }

    public bool CheckPoint(Vector3 hitPoint, out int px, out int py)
    {
        // Transform the point from world space to local space
        Vector3 localPoint = this.transform.InverseTransformPoint(hitPoint);

        // Offset the point to the corner of the object instead of the center so
        // we can transform to uv coordinates
        localPoint.x += _collider.size.x / 2;
        localPoint.y += _collider.size.y / 2;

        Texture2D texture = _spriteRenderer.sprite.texture;

        // Transform the point from local space to uv coordinates
        px = (int)((localPoint.x / _collider.size.x) * texture.width);
        py = (int)((localPoint.y / _collider.size.y) * texture.height);

        // Return true if the pixel is not empty (not transparent)
        return texture.GetPixel(px, py).a != 0.0f;
    }

    public bool Splat(Vector3 hitPoint)
    {
        int px;
        int py;

        // Only proceed if the point maps to a non-empty pixel
        if (!CheckPoint(hitPoint, out px, out py)) {
            return false;
        }

        Texture2D texture = _spriteRenderer.sprite.texture;

        // Offset the point by half the size of the splat texture so the splat
        // is centered around the hit point
        px -= this.splat.width / 2;
        py -= this.splat.height / 2;

        int startX = px;

        // Loop through all of the coordinates in the splat texture so we can
        // alpha mask the bunker texture with the splat texture
        for (int y = 0; y < this.splat.height; y++)
        {
            px = startX;

            for (int x = 0; x < this.splat.width; x++)
            {
                // Multiply the alpha of the splat pixel with the alpha of the
                // bunker texture to make it look like parts of the bunker are
                // being destroyed
                Color pixel = texture.GetPixel(px, py);
                pixel.a *= this.splat.GetPixel(x, y).a;
                texture.SetPixel(px, py, pixel);
                px++;
            }

            py++;
        }

        // Apply any changes made to the texture
        texture.Apply();

        return true;
    }

    public bool CheckCollision(BoxCollider2D other, Vector3 hitPoint)
    {
        Vector2 offset = other.size / 2;

        // Check the hit point and each edge of the colliding object to see if
        // it splats with the bunker for more accurate collision detection
        return Splat(hitPoint) ||
               Splat(hitPoint + (Vector3.down * offset.y)) ||
               Splat(hitPoint + (Vector3.up * offset.y)) ||
               Splat(hitPoint + (Vector3.left * offset.x)) ||
               Splat(hitPoint + (Vector3.right * offset.x));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Invader")) {
            this.gameObject.SetActive(false);
        }
    }

}
