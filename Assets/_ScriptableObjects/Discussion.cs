using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Discussion", menuName = "Discussion")]
public class Discussion : ScriptableObject
{
    public string theme;
    [TextArea] public string description;
    public Character[] characters = new Character[2];
    public Tile[] gameTiles = new Tile[2];
    public Tile solvedTile;
    public float stepDelay = 1f;
    public Color[] colors = new Color[2];
    public int scoreGoal;

    public void Initialize() {
        foreach(Character character in characters) {
            character.Initialize();
        }
    }
}
