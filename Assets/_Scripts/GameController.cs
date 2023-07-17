using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour
{
    public static GameController Instance {get; private set;}
    
    [Header("References")]
    [SerializeField] BoardController boardController;
    [Header("Resources")]
    [SerializeField] AudioClip soundtrack;
    [SerializeField] Discussion[] discussions = new Discussion[6];
    [SerializeField] Material backgroundMaterial;

    private Discussion currentDiscussion;
    private int discussionIndex = 0,score1,score2;

    // Unity event methods
    private void Awake() {
        Instance = this;
    }
    private void Start() {
        SoundController.Instance.PlaySoundRepeating(soundtrack);
        InitDiscussion(discussions[0]);
        boardController.OnScoreIncrease += ScoreIncrease;
    }

    // Public methods and properties
    public int GetScoreGoal() => currentDiscussion.scoreGoal;
    public float GetStepDelay() => currentDiscussion.stepDelay;
    public Tile GetRandomTile() {
        int randomIdx = Random.Range(0,2);
        return currentDiscussion.gameTiles[randomIdx];
    }
    public void InitDiscussion(Discussion discussion) {
        currentDiscussion = discussion;
        discussion.Initialize();
        score1 = 0;
        score2 = 0;

        // Init UI
        UiController.Instance.InitDiscussion(discussion);

        // Change colors
        backgroundMaterial.SetColor("_ColorA", discussion.colors[0]);
        backgroundMaterial.SetColor("_ColorB", discussion.colors[1]);

        // Init game
        boardController.StartLevel(discussion.gameTiles[0], discussion.gameTiles[1]);
    }
    public void EndDiscussion() {}
    public void ScoreIncrease(int scoreInc1, int scoreInc2) {
        score1 += scoreInc1;
        score2 += scoreInc2;

        Debug.Log("========");
        Debug.Log(score1);
        Debug.Log(score2);

        if (score1 >= currentDiscussion.scoreGoal && score2 >= currentDiscussion.scoreGoal) {
            boardController.StopLevel();
            discussionIndex += 1;
            InitDiscussion(discussions[discussionIndex]);
        }
    }
}
