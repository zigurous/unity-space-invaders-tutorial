using UnityEngine;
using UnityEngine.UI;

public sealed class GameManager : MonoBehaviour
{
    private Player player;
    private Invaders invaders;
    private MysteryShip mysteryShip;
    private Bunker[] bunkers;

    public GameObject gameOverUI;
    public Text scoreText;
    public Text livesText;

    public int score { get; private set; }
    public int lives { get; private set; }

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        invaders = FindObjectOfType<Invaders>();
        mysteryShip = FindObjectOfType<MysteryShip>();
        bunkers = FindObjectsOfType<Bunker>();
    }

    private void Start()
    {
        player.killed += OnPlayerKilled;
        mysteryShip.killed += OnMysteryShipKilled;
        invaders.killed += OnInvaderKilled;

        NewGame();
    }

    private void Update()
    {
        if (lives <= 0 && Input.GetKeyDown(KeyCode.Return)) {
            NewGame();
        }
    }

    private void NewGame()
    {
        gameOverUI.SetActive(false);

        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        invaders.ResetInvaders();
        invaders.gameObject.SetActive(true);

        for (int i = 0; i < bunkers.Length; i++) {
            bunkers[i].ResetBunker();
        }

        Respawn();
    }

    private void Respawn()
    {
        Vector3 position = player.transform.position;
        position.x = 0f;
        player.transform.position = position;
        player.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        gameOverUI.SetActive(true);
        invaders.gameObject.SetActive(false);
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString().PadLeft(4, '0');
    }

    private void SetLives(int lives)
    {
        this.lives = Mathf.Max(lives, 0);
        livesText.text = lives.ToString();
    }

    private void OnPlayerKilled()
    {
        SetLives(lives - 1);

        player.gameObject.SetActive(false);

        if (lives > 0) {
            Invoke(nameof(NewRound), 1f);
        } else {
            GameOver();
        }
    }

    private void OnInvaderKilled(Invader invader)
    {
        SetScore(score + invader.score);

        if (invaders.AmountKilled == invaders.TotalAmount) {
            NewRound();
        }
    }

    private void OnMysteryShipKilled(MysteryShip mysteryShip)
    {
        SetScore(score + mysteryShip.score);
    }

}
