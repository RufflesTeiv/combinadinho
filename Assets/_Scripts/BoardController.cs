using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardController : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private Vector3Int spawnPosition;
    [SerializeField] private Vector2Int boardSize;
    [Header("References")]
    [SerializeField] private PieceController activePiece;
    [SerializeField] private Tilemap boardTilemap;
    [Header("Resources")]
    [SerializeField] private AudioClip lineClearSound;
    [SerializeField] private TetrominoData[] tetrominos;

    public Action<int,int> OnScoreIncrease;

    private TileBase tileType1, tileType2;
    private bool levelRunning = false;

    private void Awake() {
        foreach(TetrominoData tetromino in tetrominos) {
            tetromino.Initialize();
        }
    }

    // private void Start() {
    //     Debug.Log("Remover aqui");
    //     StartLevel();
    // }

    // Public methods and properties
    public bool IsValidPosition(PieceController piece, Vector3Int position) {
        foreach (Vector3Int cell in piece.cells) {
            Vector3Int tilePosition = cell + position;

            if (this.boardTilemap.HasTile(tilePosition)) {
                return false;
            }
            if (!Bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }
        }

        return true;
    }
    public RectInt Bounds => new RectInt(0, 0, boardSize.x, boardSize.y);
    public void ClearPieceFromTilemap(PieceController piece) {
        foreach(Vector3Int cell in piece.cells) {
            Vector3Int tilePosition = cell + piece.position;
            boardTilemap.SetTile(tilePosition, null);
        }
    }
    public void LockPiece(PieceController piece) {
        SetPieceOnTilemap(piece);
        CheckLineClear();
        if (levelRunning) {
            SpawnPiece();
        }
    }
    public void SetPieceOnTilemap(PieceController piece) {
        foreach(Vector3Int cell in piece.cells) {
            Vector3Int tilePosition = cell + piece.position;
            boardTilemap.SetTile(tilePosition, piece.tile);
        }
    }
    public void SpawnPiece() {
        int random = UnityEngine.Random.Range(0, this.tetrominos.Length);
        TetrominoData data = tetrominos[random];
        activePiece.Initialize(this, spawnPosition, data, GameController.Instance.GetRandomTile());
        SetPieceOnTilemap(activePiece);
    }
    public void StartLevel(TileBase tile1, TileBase tile2) {
        tileType1 = tile1;
        tileType2 = tile2;
        boardTilemap.ClearAllTiles();
        SpawnPiece();
        levelRunning = true;
    }
    public void StopLevel() {
        levelRunning = false;
    }

    // Private methods and properties
    private void BringLinesDown(int startRow) {
        RectInt bounds = Bounds;
        for(int row = startRow+1; row < bounds.yMax; row++) {
            for (int col = bounds.xMin; col < bounds.xMax; col++) {
                Vector3Int startPos = new Vector3Int(col, row, 0);
                TileBase tile = boardTilemap.GetTile(startPos);
                Vector3Int endPos = new Vector3Int(col, row-1, 0);
                boardTilemap.SetTile(endPos, tile);
            }
        }
    }
    private void CheckLineClear() {
        RectInt bounds = Bounds;
        int row = bounds.yMin;
        
        bool lineCleared = false;
        while (row < bounds.yMax) {
            if (IsLineFull(row)) {
                LineClear(row);
                BringLinesDown(row);
                lineCleared = true;
            }
            else row++;
        }

        if (lineCleared)
            SoundController.Instance.PlaySound(lineClearSound);
    }
    private bool IsLineFull(int row) {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++) {
            Vector3Int pos = new Vector3Int(col, row, 0);
            if (!boardTilemap.HasTile(pos))
                return false;
        }
        return true;
    }
    private void LineClear(int row) {
        RectInt bounds = Bounds;
        int count1=0, count2=0;
        for (int col = bounds.xMin; col < bounds.xMax; col++) {
            Vector3Int pos = new Vector3Int(col, row, 0);
            TileBase tileType = boardTilemap.GetTile(pos);
            if (tileType == tileType1)
                count1++;
            else count2++;
            boardTilemap.SetTile(pos,null);
        }

        OnScoreIncrease?.Invoke(count1,count2);
    }
}
