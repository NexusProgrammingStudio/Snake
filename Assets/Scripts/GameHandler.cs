using UnityEngine;


public class GameHandler : MonoBehaviour {

    private static GameHandler instance;

    [SerializeField] private PlayerMovement snake;
    [SerializeField] private PlayerMovement snake2;

    private LevelGrid levelGrid;

    private void Awake() {
        instance = this;
        Score.InitializeStatic();
        Time.timeScale = 1f;
    }

    private void Start() {

        levelGrid = new LevelGrid(21, 21);

        snake.Setup(levelGrid);
        levelGrid.Setup(snake);
        snake2.Setup(levelGrid);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (IsGamePaused()) {
                GameHandler.ResumeGame();
            } else {
                GameHandler.PauseGame();
            }
        }
    }

    public static void SnakeDied() {
        bool isNewHighscore = Score.TrySetNewHighscore();
        GameOverWindow.ShowStatic(isNewHighscore);
        Time.timeScale = 0f;
        ScoreWindow.HideStatic();
    }

    public static void ResumeGame() {
        PauseWindow.HideStatic();
        Time.timeScale = 1f;
    }

    public static void PauseGame() {
        PauseWindow.ShowStatic();
        Time.timeScale = 0f;
    }

    public static bool IsGamePaused() {
        return Time.timeScale == 0f;
    }
}
