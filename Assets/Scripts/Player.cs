using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5.0f;
    public Projectile laserPrefab;
    public System.Action killed;
    public bool laserActive { get; private set; }

    private void Update()
    {
        Vector3 position = this.transform.position;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            position.x -= this.speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            position.x += this.speed * Time.deltaTime;
        }

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        // Clamp the position of the character so they do not go out of bounds
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
        this.laserActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Missile") ||
            other.gameObject.layer == LayerMask.NameToLayer("Invader"))
        {
            if (this.killed != null) {
                this.killed.Invoke();
            }
        }
    }

}
