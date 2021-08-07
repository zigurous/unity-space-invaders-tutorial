using UnityEngine;

public class MysteryShip : MonoBehaviour
{
    public float speed = 5.0f;
    public float cycleTime = 30.0f;
    public int score = 300;
    public System.Action<MysteryShip> killed;

    public Vector3 leftDestination { get; private set; }
    public Vector3 rightDestination { get; private set; }
    public int direction { get; private set; } = -1;
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

        this.transform.position = this.leftDestination;
        Despawn();
    }

    private void Update()
    {
        if (!this.spawned) {
            return;
        }

        if (this.direction == 1) {
            MoveRight();
        } else {
            MoveLeft();
        }
    }

    private void MoveRight()
    {
        this.transform.position += Vector3.right * this.speed * Time.deltaTime;

        if (this.transform.position.x >= this.rightDestination.x) {
            Despawn();
        }
    }

    private void MoveLeft()
    {
        this.transform.position += Vector3.left * this.speed * Time.deltaTime;

        if (this.transform.position.x <= this.leftDestination.x) {
            Despawn();
        }
    }

    private void Spawn()
    {
        this.direction *= -1;

        if (this.direction == 1) {
            this.transform.position = this.leftDestination;
        } else {
            this.transform.position = this.rightDestination;
        }

        this.spawned = true;
    }

    private void Despawn()
    {
        this.spawned = false;

        if (this.direction == 1) {
            this.transform.position = this.rightDestination;
        } else {
            this.transform.position = this.leftDestination;
        }

        Invoke(nameof(Spawn), this.cycleTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            Despawn();

            if (this.killed != null) {
                this.killed.Invoke(this);
            }
        }
    }

}
