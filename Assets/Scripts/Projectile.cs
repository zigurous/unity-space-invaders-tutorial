using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Projectile : MonoBehaviour
{
    public float speed = 20.0f;
    public Vector3 direction = Vector3.up;
    public System.Action<Projectile> destroyed;
    public new BoxCollider2D collider { get; private set; }

    private void Awake()
    {
        this.collider = GetComponent<BoxCollider2D>();
    }

    private void OnDestroy()
    {
        if (this.destroyed != null) {
            this.destroyed.Invoke(this);
        }
    }

    private void Update()
    {
        this.transform.position += this.direction * this.speed * Time.deltaTime;
    }

    private void CheckCollision(Collider2D other)
    {
        Bunker bunker = other.gameObject.GetComponent<Bunker>();

        if (bunker == null || bunker.CheckCollision(this.collider, this.transform.position)) {
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
