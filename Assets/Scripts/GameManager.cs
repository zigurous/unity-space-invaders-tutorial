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
    [Tooltip("A reference to the player game object.")]
    public Player player;

    /// <summary>
    /// A reference to the invaders game object.
    /// </summary>
    [Tooltip("A reference to the invaders game object.")]
    public Invaders invaders;

    /// <summary>
    /// A reference to the MysteryShip game object.
    /// </summary>
    [Tooltip("A reference to the MysteryShip game object.")]
    public MysteryShip mysteryShip;

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
        if (this.lives == 0 && Input.GetKeyDown(KeyCode.Return)) {
            NewGame();
        }
    }

    private void NewGame()
    {
        // Reset score and lives
        SetScore(0);
        SetLives(3);

        // Reset all of the invaders
        this.invaders.ResetInvaders();
        this.invaders.gameObject.SetActive(true);

        // Hide the game over UI
        this.gameOverUI.SetActive(false);

        // Spawn the player
        Respawn();
    }

    private void GameOver()
    {
        // Show the game over UI and hide the invaders
        this.gameOverUI.SetActive(true);
        this.invaders.gameObject.SetActive(false);
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

    private void SetScore(int score)
    {
        // Set score and update UI text
        this.score = score;
        this.scoreText.text = this.score.ToString().PadLeft(4, '0');
    }

    private void SetLives(int lives)
    {
        // Set lives and update UI text
        this.lives = lives;
        this.livesText.text = this.lives.ToString();
    }

    private void OnPlayerKilled()
    {
        // Decrement lives by 1
        SetLives(this.lives - 1);

        // Temporarily disable the player game object
        this.player.gameObject.SetActive(false);

        // Respawn the player after 1 second or trigger the game over state
        // after running out of lives
        if (this.lives > 0) {
            Invoke(nameof(Respawn), 1.0f);
        } else {
            GameOver();
        }
    }

    private void OnInvaderKilled(Invader invader)
    {
        // Increment score by how much the invader is worth
        SetScore(this.score + invader.score);
    }

    private void OnMysteryShipKilled(MysteryShip mysteryShip)
    {
        // Increment score by how much the mystery ship is worth
        SetScore(this.score + mysteryShip.score);
    }

}
