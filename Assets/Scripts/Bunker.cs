using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Bunker : MonoBehaviour
{
    public Texture2D splat;
    public Texture2D originalTexture { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public new BoxCollider2D collider { get; private set; }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        originalTexture = spriteRenderer.sprite.texture;

        ResetBunker();
    }

    public void ResetBunker()
    {
        // Each bunker needs a unique instance of the sprite texture since we
        // will be modifying it at the source
        CopyTexture(originalTexture);

        gameObject.SetActive(true);
    }

    private void CopyTexture(Texture2D source)
    {
        Texture2D copy = new Texture2D(source.width, source.height, source.format, false);
        copy.filterMode = source.filterMode;
        copy.anisoLevel = source.anisoLevel;
        copy.wrapMode = source.wrapMode;
        copy.SetPixels(source.GetPixels());
        copy.Apply();

        Sprite sprite = Sprite.Create(copy, spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f), spriteRenderer.sprite.pixelsPerUnit);
        spriteRenderer.sprite = sprite;
    }

    public bool CheckPoint(Vector3 hitPoint, out int px, out int py)
    {
        // Transform the point from world space to local space
        Vector3 localPoint = transform.InverseTransformPoint(hitPoint);

        // Offset the point to the corner of the object instead of the center so
        // we can transform to uv coordinates
        localPoint.x += collider.size.x / 2;
        localPoint.y += collider.size.y / 2;

        Texture2D texture = spriteRenderer.sprite.texture;

        // Transform the point from local space to uv coordinates
        px = (int)((localPoint.x / collider.size.x) * texture.width);
        py = (int)((localPoint.y / collider.size.y) * texture.height);

        // Return true if the pixel is not empty (not transparent)
        return texture.GetPixel(px, py).a != 0f;
    }

    public bool Splat(Vector3 hitPoint)
    {
        int px;
        int py;

        // Only proceed if the point maps to a non-empty pixel
        if (!CheckPoint(hitPoint, out px, out py)) {
            return false;
        }

        Texture2D texture = spriteRenderer.sprite.texture;

        // Offset the point by half the size of the splat texture so the splat
        // is centered around the hit point
        px -= splat.width / 2;
        py -= splat.height / 2;

        int startX = px;

        // Loop through all of the coordinates in the splat texture so we can
        // alpha mask the bunker texture with the splat texture
        for (int y = 0; y < splat.height; y++)
        {
            px = startX;

            for (int x = 0; x < splat.width; x++)
            {
                // Multiply the alpha of the splat pixel with the alpha of the
                // bunker texture to make it look like parts of the bunker are
                // being destroyed
                Color pixel = texture.GetPixel(px, py);
                pixel.a *= splat.GetPixel(x, y).a;
                texture.SetPixel(px, py, pixel);
                px++;
            }

            py++;
        }

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
            gameObject.SetActive(false);
        }
    }

}
