using UnityEngine;

/// <summary>
/// Handles all invader creation, movement, and missile attacks.
/// </summary>
public class Invaders : MonoBehaviour
{
    /// <summary>
    /// The prefab that is cloned for each row of invaders.
    /// </summary>
    [Tooltip("The prefab that is cloned for each row of invaders.")]
    [Header("Invaders")]
    public Invader[] prefabs = new Invader[5];

    /// <summary>
    /// The speed at which the invaders move (y-axis) relative to the percentage
    /// of invaders killed (x-axis). Typically the invaders move faster the less
    /// there are alive.
    /// </summary>
    [Tooltip("The speed at which the invaders move (y-axis) relative to the percentage of invaders killed (x-axis). Typically the invaders move faster the less there are alive.")]
    public AnimationCurve speed = new AnimationCurve();

    /// <summary>
    /// The direction the invaders are moving.
    /// </summary>
    public Vector3 direction { get; private set; } = Vector3.right;

    /// <summary>
    /// The initial position of the invaders so they can be reset.
    /// </summary>
    private Vector3 _initialPosition;

    /// <summary>
    /// The callback invoked when an invader is killed.
    /// </summary>
    public System.Action<Invader> killed;

    /// <summary>
    /// The amount of invaders that have been killed.
    /// </summary>
    public int AmountKilled { get; private set; }

    /// <summary>
    /// The amount of invaders that are still alive.
    /// </summary>
    public int AmountAlive => this.TotalAmount - this.AmountKilled;

    /// <summary>
    /// The total amount of invaders (dead or alive).
    /// </summary>
    public int TotalAmount => this.rows * this.columns;

    /// <summary>
    /// The percentage of invaders that have been killed.
    /// </summary>
    public float PercentKilled => (float)this.AmountKilled / (float)this.TotalAmount;

    /// <summary>
    /// The number of rows of invaders.
    /// </summary>
    [Tooltip("The number of rows of invaders.")]
    [Header("Grid")]
    public int rows = 5;

    /// <summary>
    /// The number of columns of invaders.
    /// </summary>
    [Tooltip("The number of columns of invaders.")]
    public int columns = 11;

    /// <summary>
    /// The prefab that is cloned when an invader shoots a missile.
    /// </summary>
    [Tooltip("The prefab that is cloned when an invader shoots a missile.")]
    [Header("Missiles")]
    public Projectile missilePrefab;

    /// <summary>
    /// The amount of seconds between missile attacks.
    /// </summary>
    [Tooltip("The amount of seconds between missile attacks.")]
    public float missileSpawnRate = 1.0f;

    private void Awake()
    {
        // Store the initial position so we can reset after each round
        _initialPosition = this.transform.position;

        // Form the grid of invaders
        for (int i = 0; i < this.rows; i++)
        {
            // Calculate the position of the row
            float width = 2.0f * (this.columns - 1);
            float height = 2.0f * (this.rows - 1);
            Vector2 centerOffset = new Vector2(-width * 0.5f, -height * 0.5f);
            Vector3 rowPosition = new Vector3(centerOffset.x, (2.0f * i) + centerOffset.y, 0.0f);

            for (int j = 0; j < this.columns; j++)
            {
                // Create an invader and parent it to this transform
                Invader invader = Instantiate(this.prefabs[i], this.transform);
                invader.killed += OnInvaderKilled;

                // Calculate and set the position of the invader in the row
                Vector3 position = rowPosition;
                position.x += 2.0f * j;
                invader.transform.localPosition = position;
            }
        }
    }

    private void Start()
    {
        // Invoke a missile attack every given amount of seconds
        InvokeRepeating(nameof(MissileAttack), this.missileSpawnRate, this.missileSpawnRate);
    }

    private void MissileAttack()
    {
        int amountAlive = this.AmountAlive;

        // No missiles should spawn when no invaders are alive
        if (amountAlive == 0) {
            return;
        }

        foreach (Transform invader in this.transform)
        {
            // Any invaders that are killed cannot shoot missiles
            if (!invader.gameObject.activeInHierarchy) {
                continue;
            }

            // Random chance to spawn a missile based upon how many invaders are
            // alive (the more invaders alive the lower the chance)
            if (Random.value < (1.0f / (float)amountAlive))
            {
                Instantiate(this.missilePrefab, invader.position, Quaternion.identity);
                break;
            }
        }
    }

    private void Update()
    {
        // Evaluate the speed of the invaders based on how many have been killed
        float speed = this.speed.Evaluate(this.PercentKilled);

        // Move all of the invaders in the current direction
        this.transform.position += this.direction * speed * Time.deltaTime;

        // Transform the viewport to world coordinates so we can check when the
        // invaders reach the edge of the screen
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        // The invaders will advance to the next row after reaching the edge of
        // the screen
        foreach (Transform invader in this.transform)
        {
            // Skip any invaders that have been killed
            if (!invader.gameObject.activeInHierarchy) {
                continue;
            }

            // Check the left edge or right edge based on the current direction
            if (this.direction == Vector3.right && invader.position.x >= (rightEdge.x - 1.0f))
            {
                AdvanceRow();
                break;
            }
            else if (this.direction == Vector3.left && invader.position.x <= (leftEdge.x + 1.0f))
            {
                AdvanceRow();
                break;
            }
        }
    }

    private void AdvanceRow()
    {
        // Flip the direction the invaders are moving
        this.direction = new Vector3(-this.direction.x, 0.0f, 0.0f);

        // Move the entire grid of invaders down a row
        Vector3 position = this.transform.position;
        position.y -= 1.0f;
        this.transform.position = position;
    }

    private void OnInvaderKilled(Invader invader)
    {
        // Disable the invader that was killed
        invader.gameObject.SetActive(false);

        // Increment the amount of invaders killed so the game manager can check
        // if they are all dead
        this.AmountKilled++;

        // Invoke the kill callback
        this.killed(invader);
    }

    public void ResetInvaders()
    {
        // Reset state
        this.AmountKilled = 0;
        this.direction = Vector3.right;

        // Reset the position of the invaders back to the top
        this.transform.position = _initialPosition;

        // Re-enable all of the invaders that were killed
        foreach (Transform invader in this.transform) {
            invader.gameObject.SetActive(true);
        }
    }

}
