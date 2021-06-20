using UnityEngine;

/// <summary>
/// Handles the movement and shooting of the player.
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// How fast the player moves.
    /// </summary>
    [Tooltip("How fast the player moves.")]
    public float speed = 5.0f;

    /// <summary>
    /// The prefab that is cloned when a laser shot is fired.
    /// </summary>
    [Tooltip("The prefab that is cloned when a laser shot is fired.")]
    public Projectile laserPrefab;

    /// <summary>
    /// The callback invoked when the player is killed.
    /// </summary>
    public System.Action killed;

    /// <summary>
    /// Whether a laser shot is currently active so we can ensure only one laser
    /// is active at a given time.
    /// </summary>
    public bool laserActive { get; private set; }

    private void Update()
    {
        Vector3 position = this.transform.position;

        // Check for input to move the player either left or right
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            position.x -= this.speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            position.x += this.speed * Time.deltaTime;
        }

        // Clamp the position of the character so they do not go out of bounds
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
        position.x = Mathf.Clamp(position.x, leftEdge.x, rightEdge.x);
        this.transform.position = position;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            Shoot();
        }
    }

    private void Shoot()
    {
        // Only one laser can be active at a given time so first check that
        // there is not already an active laser
        if (!this.laserActive)
        {
            this.laserActive = true;

            Projectile laser = Instantiate(this.laserPrefab, this.transform.position, Quaternion.identity);
            laser.destroyed += OnLaserDestroyed;
        }
    }

    private void OnLaserDestroyed(Projectile laser)
    {
        // Once the laser is destroyed we can shoot again
        this.laserActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Missile") ||
            other.gameObject.layer == LayerMask.NameToLayer("Invader")) {
            this.killed?.Invoke();
        }
    }

}
