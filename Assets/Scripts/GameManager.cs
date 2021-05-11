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
        // Register callbacks for game state
        this.player.killed += OnPlayerKilled;
        this.mysteryShip.killed += OnMysteryShipKilled;
        this.invaders.killed += OnInvaderKilled;

        // Start a new game
        NewGame();
    }

    private void Update()
    {
        // Start a new game once the player presses 'Return'
        if (this.lives <= 0 && Input.GetKeyDown(KeyCode.Return)) {
            NewGame();
        }
    }

    private void NewGame()
    {
        // Hide the game over UI
        this.gameOverUI.SetActive(false);

        // Reset score and lives
        SetScore(0);
        SetLives(3);

        // Start the first round
        NewRound();
    }

    private void NewRound()
    {
        // Reset all of the invaders
        this.invaders.ResetInvaders();
        this.invaders.gameObject.SetActive(true);

        // Reset all of the bunkers
        for (int i = 0; i < this.bunkers.Length; i++) {
            this.bunkers[i].ResetBunker();
        }

        // Spawn the player
        Respawn();
    }

    private void Respawn()
    {
        // Reset the position of the player
        Vector3 position = this.player.transform.position;
        position.x = 0.0f;
        this.player.transform.position = position;

        // Re-enable the player game object
        this.player.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        // Show the game over UI and hide the invaders
        this.gameOverUI.SetActive(true);
        this.invaders.gameObject.SetActive(false);
    }

    private void SetScore(int score)
    {
        // Set score and update UI text
        this.score = score;
        this.scoreText.text = this.score.ToString().PadLeft(4, '0');
    }

    private void SetLives(int lives)
    {
        // Set lives and update UI text
        this.lives = Mathf.Max(lives, 0);
        this.livesText.text = this.lives.ToString();
    }

    private void OnPlayerKilled()
    {
        // Decrement lives by 1
        SetLives(this.lives - 1);

        // Temporarily disable the player game object
        this.player.gameObject.SetActive(false);

        // Start the round over again after 1 second or trigger the game over
        // state if out of lives
        if (this.lives > 0) {
            Invoke(nameof(NewRound), 1.0f);
        } else {
            GameOver();
        }
    }

    private void OnInvaderKilled(Invader invader)
    {
        // Increment score by how much the invader is worth
        SetScore(this.score + invader.score);

        // Start a new round after all invaders have been killed
        if (this.invaders.amountKilled == this.invaders.totalAmount) {
            NewRound();
        }
    }

    private void OnMysteryShipKilled(MysteryShip mysteryShip)
    {
        // Increment score by how much the mystery ship is worth
        SetScore(this.score + mysteryShip.score);
    }

}
