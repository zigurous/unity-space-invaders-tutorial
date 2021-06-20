using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the overall state of the game, including starting a new game and
/// handling scoring and lives.
/// </summary>
public sealed class GameManager : MonoBehaviour
{
    /// <summary>
    /// A reference to the player game object.
    /// </summary>
    private Player player;

    /// <summary>
    /// A reference to the invaders game object.
    /// </summary>
    private Invaders invaders;

    /// <summary>
    /// A reference to the MysteryShip game object.
    /// </summary>
    private MysteryShip mysteryShip;

    /// <summary>
    /// A reference to all of the bunker game objects.
    /// </summary>
    private Bunker[] bunkers;

    /// <summary>
    /// The UI text that displays the player's score.
    /// </summary>
    [Tooltip("The UI text that displays the player's score.")]
    public Text scoreText;

    /// <summary>
    /// The UI text that displays the player's lives.
    /// </summary>
    [Tooltip("The UI text that displays the player's lives.")]
    public Text livesText;

    /// <summary>
    /// The UI displayed during the game over state.
    /// </summary>
    [Tooltip("The UI displayed during the game over state.")]
    public GameObject gameOverUI;

    /// <summary>
    /// The player's current score.
    /// </summary>
    public int score { get; private set; }

    /// <summary>
    /// The player's current lives.
    /// </summary>
    public int lives { get; private set; }

    private void Awake()
    {
        this.player = FindObjectOfType<Player>();
        this.invaders = FindObjectOfType<Invaders>();
        this.mysteryShip = FindObjectOfType<MysteryShip>();
        this.bunkers = FindObjectsOfType<Bunker>();
    }

    private void Start()
    {
        this.player.killed += OnPlayerKilled;
        this.mysteryShip.killed += OnMysteryShipKilled;
        this.invaders.killed += OnInvaderKilled;

        NewGame();
    }

    private void Update()
    {
        if (this.lives <= 0 && Input.GetKeyDown(KeyCode.Return)) {
            NewGame();
        }
    }

    private void NewGame()
    {
        this.gameOverUI.SetActive(false);

        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        this.invaders.ResetInvaders();
        this.invaders.gameObject.SetActive(true);

        for (int i = 0; i < this.bunkers.Length; i++) {
            this.bunkers[i].ResetBunker();
        }

        Respawn();
    }

    private void Respawn()
    {
        Vector3 position = this.player.transform.position;
        position.x = 0.0f;
        this.player.transform.position = position;
        this.player.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        this.gameOverUI.SetActive(true);
        this.invaders.gameObject.SetActive(false);
    }

    private void SetScore(int score)
    {
        this.score = score;
        this.scoreText.text = this.score.ToString().PadLeft(4, '0');
    }

    private void SetLives(int lives)
    {
        this.lives = Mathf.Max(lives, 0);
        this.livesText.text = this.lives.ToString();
    }

    private void OnPlayerKilled()
    {
        SetLives(this.lives - 1);

        this.player.gameObject.SetActive(false);

        if (this.lives > 0) {
            Invoke(nameof(NewRound), 1.0f);
        } else {
            GameOver();
        }
    }

    private void OnInvaderKilled(Invader invader)
    {
        SetScore(this.score + invader.score);

        if (this.invaders.AmountKilled == this.invaders.TotalAmount) {
            NewRound();
        }
    }

    private void OnMysteryShipKilled(MysteryShip mysteryShip)
    {
        SetScore(this.score + mysteryShip.score);
    }

}
