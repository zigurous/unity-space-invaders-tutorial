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
    public Vector3 leftDestination { get; private set; }

    /// <summary>
    /// The destination point of the mystery ship on the right edge of the
    /// screen.
    /// </summary>
    public Vector3 rightDestination { get; private set; }

    /// <summary>
    /// The direction the mystery ship is traveling, 1=right, -1=left.
    /// </summary>
    public int direction { get; private set; } = -1;

    /// <summary>
    /// Whether the mystery ship is currently spawned.
    /// </summary>
    public bool spawned { get; private set; }

    private void Start()
    {
        // Transform the viewport to world coordinates so we can set the mystery
        // ship's destination points
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        // Offset the destination by a unit so the ship is fully out of sight
        Vector3 left = this.transform.position;
        left.x = leftEdge.x - 1.0f;
        this.leftDestination = left;

        Vector3 right = this.transform.position;
        right.x = rightEdge.x + 1.0f;
        this.rightDestination = right;

        // Start on the left side of the screen
        this.transform.position = this.leftDestination;

        // Make sure the mystery ship is initially despawned. This will also
        // start the spawn cycle timer.
        Despawn();
    }

    private void Spawn()
    {
        // Flip the direction so it moves the opposite way each time it spawns
        this.direction *= -1;

        // Set the spawn point opposite side of the destination
        if (this.direction == 1) {
            this.transform.position = this.leftDestination;
        } else {
            this.transform.position = this.rightDestination;
        }

        // Mark as spawned so it starts moving
        this.spawned = true;
    }

    private void Despawn()
    {
        // Mark as despawned so it stops moving
        this.spawned = false;

        // Move the mystery ship to the destination point so it is hidden off
        // the screen
        if (this.direction == 1) {
            this.transform.position = this.rightDestination;
        } else {
            this.transform.position = this.leftDestination;
        }

        // Start the timer again to respawn the mystery ship
        Invoke(nameof(Spawn), this.cycleTime);
    }

    private void Update()
    {
        // Do not do anything if the mystery ship is not moving
        if (!this.spawned) {
            return;
        }

        if (this.direction == 1)
        {
            // Move the mystery ship to the right based on its speed
            this.transform.position += Vector3.right * this.speed * Time.deltaTime;

            // Despawn the mystery ship once it exceeds the right screen edge
            if (this.transform.position.x >= this.rightDestination.x) {
                Despawn();
            }
        }
        else
        {
            // Move the mystery ship to the left based on its speed
            this.transform.position += Vector3.left * this.speed * Time.deltaTime;

            // Despawn the mystery ship once it exceeds the left screen edge
            if (this.transform.position.x <= this.leftDestination.x) {
                Despawn();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            Despawn();
            this.killed?.Invoke(this);
        }
    }

}
