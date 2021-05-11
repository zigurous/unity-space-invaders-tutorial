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
    private bool _laserActive;

    private void Update()
    {
        // Check for input to move the player either left or right
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            this.transform.position += Vector3.left * this.speed * Time.deltaTime;
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            this.transform.position += Vector3.right * this.speed * Time.deltaTime;
        }

        // Check for input to shoot a laser
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            Shoot();
        }
    }

    private void Shoot()
    {
        // Only one laser can be active at a given time so first check that
        // there is not already an active laser
        if (!_laserActive)
        {
            _laserActive = true;

            // Create a new laser projectile and register a callback so we know
            // when it is destroyed
            Projectile laser = Instantiate(this.laserPrefab, this.transform.position, Quaternion.identity);
            laser.destroyed += OnLaserDestroyed;
        }
    }

    private void OnLaserDestroyed(Projectile laser)
    {
        // Deactivate the laser so we can fire again
        _laserActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // The player is killed when hit by a missile or invader
        if (other.gameObject.layer == LayerMask.NameToLayer("Missile") ||
            other.gameObject.layer == LayerMask.NameToLayer("Invader")) {
            this.killed?.Invoke();
        }
    }

}
