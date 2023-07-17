using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PieceController : MonoBehaviour
{
    [Header("Resources")]
    [SerializeField] private AudioClip blockHitSound;


    public BoardController board {get; private set;}
    public TetrominoData data {get; private set;}
    public Vector3Int[] cells {get; private set;}
    public Vector3Int position {get; private set;}
    public Tile tile {get; private set;}
    public int rotationIndex {get; private set;}

    private float lockDelay = 0.5f, stepTimeGoal, lockTime;

    // Unity event functions
    private void Update() {
        CountTime();
        ControlPiece();
        CheckTime();
    }

    // Public methods and properties
    public void Initialize(BoardController board, Vector3Int position, TetrominoData tetrominoData, Tile tile) {
        this.board = board;
        this.position = position;
        data = tetrominoData;
        this.tile = tile;
        this.rotationIndex = 0;
        PushStepTimeGoal();

        // Timing
        lockTime = 0f;

        if (this.cells == null) {
            this.cells = new Vector3Int[data.cells.Length];
        }
        for (int i=0; i<data.cells.Length; i++) {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    // Private methods and properties
    private void CheckTime() {
        if (Time.time >= this.stepTimeGoal)
            Step();
    }
    private void ControlPiece() {
        // Movement
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            Move(Vector2Int.left);
        else if  (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            Move(Vector2Int.right);
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            Move(Vector2Int.down);
        else if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(HardDrop());

        // Rotation
        if (Input.GetKeyDown(KeyCode.Q))
            Rotate(-1);
        else if (Input.GetKeyDown(KeyCode.E))
            Rotate(1);
    }
    private void CountTime() {
        lockTime += Time.deltaTime;
    }
    private void PushStepTimeGoal() => stepTimeGoal = Time.time + GameController.Instance.GetStepDelay();
    private void Step() {
        PushStepTimeGoal();
        Move(Vector2Int.down, MovementType.Step);
    }

    // Movement
    private enum MovementType {
        Player,
        Step,
        Wallkick
    }
    private bool Move(Vector2Int translation, MovementType movementType = MovementType.Player) {
        Vector3Int newPosition = this.position + (Vector3Int)translation;

        board.ClearPieceFromTilemap(this);
        bool moveDown = translation == Vector2Int.down;
        bool movementIsValid = board.IsValidPosition(this, newPosition);
        if (movementIsValid) {
            position = newPosition;
            lockTime = 0f;
            if (moveDown)
                PushStepTimeGoal();
        } else {
            bool lockPiece = moveDown && (movementType == MovementType.Player || (movementType == MovementType.Step && this.lockTime >= this.lockDelay));
            if ((moveDown && movementType == MovementType.Player) || (movementType == MovementType.Step && lockPiece))
                SoundController.Instance.PlaySound(blockHitSound);
            if (lockPiece) {
                board.LockPiece(this);
                return false;
            }
        }
        board.SetPieceOnTilemap(this);

        return movementIsValid;
    }
    private IEnumerator HardDrop() {
        while(Move(Vector2Int.down));
        yield break;
    }

    // Rotation
    private void Rotate(int direction)
    {
        board.ClearPieceFromTilemap(this);
        // Store the current rotation in case the rotation fails
        // and we need to revert
        int originalRotation = rotationIndex;

        // Rotate all of the cells using a rotation matrix
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        // Revert the rotation if the wall kick tests fail
        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
        board.SetPieceOnTilemap(this);
    }
    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = TetrisData.RotationMatrix;
        Vector3Int[] oldCells = new Vector3Int[cells.Length];
        cells.CopyTo(oldCells,0);

        // Rotate all of the cells using the rotation matrix
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetrominoType)
            {
                case TetrominoType.I: case TetrominoType.O:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }

        if (!board.IsValidPosition(this, position)) {
            cells = oldCells;
        }
    }
    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation, MovementType.Wallkick)) {
                return true;
            }
        }

        return false;
    }
    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0) {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }
    private int Wrap(int input, int minInclusive, int maxExclusive) {
        if (input < minInclusive)
            return maxExclusive - (minInclusive - input) % (maxExclusive - minInclusive);
        else return minInclusive + (input - minInclusive) % (maxExclusive - minInclusive);
    }
}
