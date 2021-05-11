using UnityEngine;

/// <summary>
/// Handles the movement and collision of a projectile.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Projectile : MonoBehaviour
{
    /// <summary>
    /// How fast the projectile moves.
    /// </summary>
    [Tooltip("How fast the projectile moves.")]
    public float speed = 20.0f;

    /// <summary>
    /// The direction of the projectile.
    /// </summary>
    [Tooltip("The direction of the projectile.")]
    public Vector3 direction = Vector3.up;

    /// <summary>
    /// The callback invoked when the projectile is destroyed.
    /// </summary>
    public System.Action<Projectile> destroyed;

    /// <summary>
    /// The box collider component of the projectile.
    /// </summary>
    private BoxCollider2D _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // Move the projectile in the given direction by its speed
        this.transform.position += this.direction * this.speed * Time.deltaTime;
    }

    private void CheckCollision(Collider2D other)
    {
        // Check if the object we are colliding with is a bunker, in which case
        // it will have a Bunker component
        Bunker bunker = other.gameObject.GetComponent<Bunker>();

        // The projectile is destroyed when colliding with any object other than
        // a bunker or when it splats on a bunker
        if (bunker == null || bunker.CheckCollision(_collider, this.transform.position))
        {
            this.destroyed?.Invoke(this);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckCollision(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        CheckCollision(other);
    }

}
