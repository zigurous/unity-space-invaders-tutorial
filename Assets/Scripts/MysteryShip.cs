using UnityEngine;

/// <summary>
/// Handles the spawning and movement of the mystery ship object.
/// </summary>
public class MysteryShip : MonoBehaviour
{
    /// <summary>
    /// How fast the mystery ship moves.
    /// </summary>
    [Tooltip("How fast the mystery ship moves.")]
    public float speed = 5.0f;

    /// <summary>
    /// The amount of seconds between mystery ship sightings.
    /// </summary>
    [Tooltip("The amount of seconds between mystery ship sightings.")]
    public float cycleTime = 30.0f;

    /// <summary>
    /// The amount of points the mystery ship is worth when killed.
    /// </summary>
    [Tooltip("The amount of points the mystery ship is worth when killed.")]
    public int score = 300;

    /// <summary>
    /// The callback invoked when the mystery ship is killed.
    /// </summary>
    public System.Action<MysteryShip> killed;

    /// <summary>
    /// The destination point of the mystery ship on the left edge of the
    /// screen.
    /// </summary>
    private Vector3 _leftDestination;

    /// <summary>
    /// The destination point of the mystery ship on the right edge of the
    /// screen.
    /// </summary>
    private Vector3 _rightDestination;

    /// <summary>
    /// The direction the mystery ship is traveling, 1=right, -1=left.
    /// </summary>
    private int _direction = -1;

    /// <summary>
    /// Whether the mystery ship is currently spawned.
    /// </summary>
    private bool _spawned;

    private void Start()
    {
        // Transform the viewport to world coordinates so we can set the mystery
        // ship's destination points
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        _leftDestination = this.transform.position;
        _leftDestination.x = leftEdge.x - 1.0f;

        _rightDestination = this.transform.position;
        _rightDestination.x = rightEdge.x + 1.0f;

        // Position the mystery ship initially on the left side of the screen
        this.transform.position = _leftDestination;

        // Make sure the mystery ship is initially despawned. This will also
        // start the spawn cycle timer.
        Despawn();
    }

    private void Spawn()
    {
        // Flip the direction so it moves the opposite way each time it spawns
        _direction *= -1;

        // Set the spawn point opposite side of the destination
        if (_direction == 1) {
            this.transform.position = _leftDestination;
        } else {
            this.transform.position = _rightDestination;
        }

        // Mark as spawned so it starts moving
        _spawned = true;
    }

    private void Despawn()
    {
        // Mark as despawned so it stops moving
        _spawned = false;

        // Move the mystery ship to the destination point so it is hidden off
        // the screen
        if (_direction == 1) {
            this.transform.position = _rightDestination;
        } else {
            this.transform.position = _leftDestination;
        }

        // Start the timer again to respawn the mystery ship
        Invoke(nameof(Spawn), this.cycleTime);
    }

    private void Update()
    {
        // Do not do anything if the mystery ship is not moving
        if (!_spawned) {
            return;
        }

        if (_direction == 1)
        {
            // Move the mystery ship to the right based on its speed
            this.transform.position += Vector3.right * this.speed * Time.deltaTime;

            // Despawn the mystery ship once it exceeds the right screen edge
            if (this.transform.position.x >= _rightDestination.x) {
                Despawn();
            }
        }
        else
        {
            // Move the mystery ship to the left based on its speed
            this.transform.position += Vector3.left * this.speed * Time.deltaTime;

            // Despawn the mystery ship once it exceeds the left screen edge
            if (this.transform.position.x <= _leftDestination.x) {
                Despawn();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // The mystery ship despawns when hit by a laser and the callback is
        // invoked so score can be increased
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            Despawn();
            this.killed?.Invoke(this);
        }
    }

}
