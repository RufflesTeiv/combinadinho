using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TetrominoType
{
    I,
    O,
    T,
    J,
    L,
    S,
    Z
}

[System.Serializable]
public class TetrominoData {
    public TetrominoType tetrominoType;

    public Vector2Int[] cells {get; private set;}
    public Vector2Int[,] wallKicks { get; private set; }

    public void Initialize() {
        this.cells = TetrisData.Cells[tetrominoType];
        this.wallKicks = TetrisData.WallKicks[tetrominoType];
    }
}
