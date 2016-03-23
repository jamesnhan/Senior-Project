/**
 * Name:        ChessBoard
 * Author:      James Nhan
 */

using UnityEngine;
using System.Collections;

/// <summary>
/// This class defines the behaviour of a chess board.
/// </summary>
public class ChessBoard : MonoBehaviour {
    /// <summary>
    /// This is the sprite for the white squares on the board.
    /// </summary>
    public Sprite WhiteSquare;

    /// <summary>
    /// This is the sprite for the black squares on the board.
    /// </summary>
    public Sprite BlackSquare;

    /// <summary>
    /// This is the prefab for a board square for automatic generation.
    /// </summary>
    public GameObject BoardSquarePrefab;

    /// <summary>
    /// This is the prefab for a game piece for automatic generation.
    /// </summary>
    public GameObject GamePiecePrefab;

    /// <summary>
    /// This variable designates whether or not a player is trying to move.
    /// </summary>
    /// <remarks>
    /// If this variable is true, then a square was clicked and the game is
    /// awaiting a second click to move a piece.
    /// </remarks>
    public bool tryingToMove = false;

    /// <summary>
    /// This is the piece that is set to be moved if tryingToMove is true.
    /// </summary>
    public GameObject movingPiece;

    /// <summary>
    /// This is a list of all of the pieces on the board.
    /// </summary>
    public ArrayList pieces;

    /// <summary>
    /// This is a jagged array of all of the squares on the board.
    /// </summary>
    /// <remarks>
    /// The array holds all of the squares on the board in row-major order.
    /// </remarks>
    public GameObject[,] squares;

    /// <summary>
    /// This is the white player's score based on pieces destroyed.
    /// </summary>
    /// <remarks>
    /// Pawn: 1.00 point
    /// Knight: 3.25 points
    /// Bishop: 3.25 points
    /// Rook: 5.00 points
    /// Queen: 9.75 points
    /// King: 100.00 points
    /// </remarks>
    public float whiteScore;

    /// <summary>
    /// This is the black player's score based on pieces destroyed.
    /// </summary>
    /// <remarks>
    /// Pawn: 1.00 point
    /// Knight: 3.25 points
    /// Bishop: 3.25 points
    /// Rook: 5.00 points
    /// Queen: 9.75 points
    /// King: 100.00 points
    /// </remarks>
    public float blackScore;

    /// <summary>
    /// This is the current player.
    /// </summary>
    /// <remarks>
    /// If this is true, it is white's turn.
    /// If this is false, it is black's turn.
    /// </remarks>
    public bool currentPlayer;

    /// <summary>
    /// This is whether or not a player is in check.
    /// </summary>
    /// <remarks>
    /// 0X = White not in check
    /// X0 = Black not in check
    /// 1X = White in check
    /// X1 = Black in check
    /// </remarks>
    public BitArray check;

    /// <summary>
    /// The width of the board in number of squares.
    /// </summary>
    public const int WIDTH = 8;

    /// <summary>
    /// The height of the board in number of squares.
    /// </summary>
    public const int HEIGHT = 8;

    /// <summary>
    /// The entry point for a chess game.
    /// </summary>
    private void Start () {
        // Initialize all variables
        check = new BitArray(4);
        // White starts
        currentPlayer = true;
        whiteScore = 0.00f;
        blackScore = 0.00f;
        squares = new GameObject[WIDTH, HEIGHT];
        // For switching between black and white while generating the board
        // Bottom left corner is black
        
        // TODO: Change this away from a bool?
        bool white = false;

        // Generate the board
        for (int i = 0; i < WIDTH; ++i) {
            for (int j = 0; j < HEIGHT; ++j) {
                // Instantiate a new board square
                GameObject square = Instantiate(BoardSquarePrefab, Vector3.zero, Quaternion.identity) as GameObject;
                // Set the size of the collider for click detection to the size of the sprite
                square.GetComponent<BoxCollider2D>().size = new Vector2(WhiteSquare.bounds.size.x, WhiteSquare.bounds.size.y);
                SpriteRenderer renderer = square.GetComponent<SpriteRenderer>();

                // Set the sprite to white and black, alternating
                if (white) {
                    renderer.sprite = WhiteSquare;
                } else {
                    renderer.sprite = BlackSquare;
                }
                white = !white;

                // Parent the new square under the board's transform
                square.transform.SetParent(gameObject.transform);
                // Set its location based on the loop iterators (i, j) and the sprite's width and height
                square.transform.localPosition = new Vector3(i * renderer.sprite.bounds.size.x, j * renderer.sprite.bounds.size.y, 1.0f);

                // Input it into the jagged array
                squares[i, j] = square;
            }
            // Each row should swap its starting color so we have a checker board effect
            white = !white;
        }

        // Create an ArrayList to hold the pieces
        // There are two rows for each player (2) that span the width of the board
        pieces = new ArrayList(WIDTH * 2 * 2);
        // Generate pieces for white
        GeneratePieces(true);
        // Generate pieces for black
        GeneratePieces(false);
    }

    /// <summary>
    /// A method to generate a piece and set all of its parameters
    /// </summary>
    /// <param name="type">The type of GamePiece (PAWN, KNIGHT, BISHOP, ROOK, QUEEN, KING)</param>
    /// <param name="white">Generate a piece for white (true) or black (flase)</param>
    /// <returns>The newly generated piece</returns>
    private GameObject GeneratePiece(ChessPiece.PieceType type, bool white) {
        // Generate the piece and set its type
        GameObject piece = Instantiate(GamePiecePrefab, Vector3.zero, Quaternion.identity) as GameObject;
        piece.transform.SetParent(gameObject.transform);
        piece.GetComponent<ChessPiece>().SetPieceType(type);

        // Set its color
        if (white) {
            piece.GetComponent<ChessPiece>().SetWhite();
        } else {
            piece.GetComponent<ChessPiece>().SetBlack();
        }

        // Add to the ArrayList
        pieces.Add(piece);

        return piece;
    }

    /// <summary>
    /// A method to generate the pieces on the board
    /// </summary>
    /// <param name="white">Generate pieces for white (true) or black (flase)</param>
    private void GeneratePieces(bool white) {
        // First create an array sized to the width of the board to hold the pawns for later use
        GameObject[] pawns = new GameObject[WIDTH];
        for (int i = 0; i < WIDTH; ++i) {
            // Instantiate the pawns and parent them under the game board
            pawns[i] = GeneratePiece(ChessPiece.PieceType.PAWN, white);
        }

        // Repeat the above for the left and right rooks
        GameObject rook = GeneratePiece(ChessPiece.PieceType.ROOK, white);
        GameObject rook2 = GeneratePiece(ChessPiece.PieceType.ROOK, white);

        // Repeat the above for the left and right knights
        GameObject knight = GeneratePiece(ChessPiece.PieceType.KNIGHT, white);
        GameObject knight2 = GeneratePiece(ChessPiece.PieceType.KNIGHT, white);

        // Repeat the above for the left and right bishops
        GameObject bishop = GeneratePiece(ChessPiece.PieceType.BISHOP, white);
        GameObject bishop2 = GeneratePiece(ChessPiece.PieceType.BISHOP, white);

        // Repeat the above for the queen
        GameObject queen = GeneratePiece(ChessPiece.PieceType.QUEEN, white);

        // Repeat the above for the king
        GameObject king = GeneratePiece(ChessPiece.PieceType.KING, white);

        // Set the positions based on the color
        if (white) {
            // Set the pawns to the bottom row of cells
            for (int i = 0; i < WIDTH; ++i) {
                pawns[i].GetComponent<ChessPiece>().SetCell(i, 1);
            }

            // Set the rooks to the left bottom and right bottom
            rook.GetComponent<ChessPiece>().SetCell(0, 0);
            rook2.GetComponent<ChessPiece>().SetCell(WIDTH - 1, 0);

            // Set the knights to the left bottom and right bottom
            knight.GetComponent<ChessPiece>().SetCell(1, 0);
            knight2.GetComponent<ChessPiece>().SetCell((WIDTH - 2), 0);

            // Set the bishops to the left bottom and right bottom
            bishop.GetComponent<ChessPiece>().SetCell(2, 0);
            bishop2.GetComponent<ChessPiece>().SetCell((WIDTH - 3), 0);

            // Set the queen and king to the left bottom and right bottom
            queen.GetComponent<ChessPiece>().SetCell(3, 0);
            king.GetComponent<ChessPiece>().SetCell((WIDTH - 4), 0);
        } else {
            // Set the pawns to the bottom row of cells
            for (int i = 0; i < WIDTH; ++i) {
                pawns[i].GetComponent<ChessPiece>().SetCell(i, (HEIGHT - 2));
            }

            // Set the rooks to the left top and right top
            rook.GetComponent<ChessPiece>().SetCell(0, (HEIGHT - 1));
            rook2.GetComponent<ChessPiece>().SetCell(WIDTH - 1, (HEIGHT - 1));

            // Set the knights to the left top and right top
            knight.GetComponent<ChessPiece>().SetCell(1, (HEIGHT - 1));
            knight2.GetComponent<ChessPiece>().SetCell((WIDTH - 2), (HEIGHT - 1));

            // Set the bishops to the left top and right top
            bishop.GetComponent<ChessPiece>().SetCell(2, (HEIGHT - 1));
            bishop2.GetComponent<ChessPiece>().SetCell((WIDTH - 3), (HEIGHT - 1));

            // Set the queen and king to the left top and right top
            queen.GetComponent<ChessPiece>().SetCell(3, (HEIGHT - 1));
            king.GetComponent<ChessPiece>().SetCell((WIDTH - 4), (HEIGHT - 1));
        }
    }

    public void KillPiece(GameObject piece) {
        ChessPiece chessPieceComponent = piece.GetComponent<ChessPiece>();
        if (chessPieceComponent.white) {
            whiteScore += chessPieceComponent.value;
        } else {
            blackScore += chessPieceComponent.value;
        }

        pieces.Remove(piece);
        Destroy(piece);
    }

    /// <summary>
    /// Gets the game piece currently residing in a cell
    /// </summary>
    /// <param name="row">The row</param>
    /// <param name="column">The column</param>
    /// <returns>The GamePiece as a GameObject</returns>
    public GameObject GetPieceInCell(int row, int column) {
        Vector2 cell = new Vector2(row, column);

        foreach (GameObject piece in pieces) {
            if (piece.GetComponent<ChessPiece>().GetCell() == cell) {
                return piece.gameObject;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Gets the square currently residing in a cell
    /// </summary>
    /// <param name="row">The row</param>
    /// <param name="column">The column</param>
    /// <returns>The square as a GameObject</returns>
    public GameObject GetSquareInCell(int row, int column) {
        Vector2 cell = new Vector2(row, column);

        foreach (GameObject square in squares) {
            if (GetCell(square) == cell) {
                return square;
            }
        }
        return null;
    }

    /// <summary>
    /// Get the cell the square is currently residing in
    /// </summary>
    /// <param name="theSquare">The square</param>
    /// <returns>The cell as a Vector2</returns>
    public Vector2 GetCell(GameObject theSquare) {
        foreach (GameObject square in squares) {
            if (theSquare == square) {
                SpriteRenderer renderer = square.GetComponent<SpriteRenderer>();
                int xCoord = (int)(square.transform.localPosition.x / renderer.sprite.bounds.size.x);
                int yCoord = (int)(square.transform.localPosition.y / renderer.sprite.bounds.size.y);
                return new Vector2(xCoord, yCoord);
            }
        }
        return new Vector2(-1, -1);
    }
}
