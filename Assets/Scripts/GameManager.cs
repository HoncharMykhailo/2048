using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TileBoard board;
    public CanvasGroup gameOver;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiscoreText;
    
    public Button newGame;

    public TMP_InputField loginInput; 
    public TextMeshProUGUI nameText; 
    public DatabaseManager dbManager;


    private int score;

    private int playerId;

    private void Start()
    {
        dbManager.TestConnection();


        LoadPlayerName();
        gameOver.interactable = false;
        newGame.interactable = false;
        loginInput.onSubmit.AddListener(delegate 
        { 
            SavePlayerName();
            loginInput.gameObject.SetActive(false);
            gameOver.interactable = true;
            newGame.interactable = true;

            NewGame();
        });
    }


    public void LoadPlayerName()
    {
        string savedName = PlayerPrefs.GetString("player_name", "Player"); 
        loginInput.text = savedName;   
        nameText.text = savedName;   
        
    }


    public void SavePlayerName()
    {
        string playerName = loginInput.text;
        PlayerPrefs.SetString("player_name", playerName);
        nameText.text = playerName; 
        playerId = dbManager.InsertPlayer(playerName,0);
        Debug.Log(playerId);
    }


    public void NewGame()
    {
        SetScore(0);


       // hiscoreText.text = LoadHiscore().ToString();

        hiscoreText.text = dbManager.GetHiScore(playerId).ToString();

        gameOver.alpha = 0f;
        gameOver.interactable = false;

        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()
    {
        board.enabled = false;
        gameOver.interactable = true;

        Debug.Log("Best Num = " + board.bestNum);

        dbManager.InsertGame(playerId, score, board.bestNum);

        StartCoroutine(Fade(gameOver, 1f, 1f));

    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed/duration);
            elapsed+=Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
    }


    public void IncreaseScore(int points)
    {
        SetScore(score + points);
    }


    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();

        SaveHiscore();
    }



    private void SaveHiscore()
    {
        int hiscore = LoadHiscore();
        if(score>hiscore)
        {
            PlayerPrefs.SetInt("hiscore",score);
        }
    }

    private int LoadHiscore()
    {
        return PlayerPrefs.GetInt("hiscore", 0);
    }
}
