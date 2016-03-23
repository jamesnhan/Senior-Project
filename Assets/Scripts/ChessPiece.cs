/**
 * Name:        ChessPiece
 * Author:      James Nhan
 */

using UnityEngine;

/// <summary>
/// This class defines the behavior of a chess piece
/// </summary>
public class ChessPiece : MonoBehaviour {
    /// <summary>
    /// An enumeration for classifying pieces
    /// </summary>
    public enum PieceType {
        PAWN, KNIGHT, BISHOP, ROOK, QUEEN, KING
    }

    /// <summary>
    /// The sprite sheet for white pieces
    /// </summary>
    public Sprite[] whiteSprite;

    /// <summary>
    /// The sprite sheet for black pieces
    /// </summary>
    public Sprite[] blackSprite;

    /// <summary>
    /// Whether or not the piece is white
    /// </summary>
    public bool white = true;

    /// <summary>
    /// THe piece's type
    /// </summary>
    public PieceType pieceType = PieceType.PAWN;

    /// <summary>
    /// The piece's x coordinate in cell coordinates
    /// </summary>
    public int xCoord;
    
    /// <summary>
    /// The piece's y coordinate in cell coordinates
    /// </summary>
    public int yCoord;

    /// <summary>
    /// Whether or not the piece has yet to make its first move
    /// </summary>
    public bool firstMove = true;

    /// <summary>
    /// A list of the piece's possible moves in unit vector form
    /// </summary>
    public Vector2[] possibleMoves;

    /// <summary>
    /// The value of the piece depending on its type
    /// </summary>
    public int value;

    /// <summary>
    /// The PieceType accessor
    /// </summary>
    /// <returns>The PieceType from the enumeration</returns>
    public PieceType GetPieceType() {
        return pieceType;
    }

    /// <summary>
    /// The PieceType mutator
    /// </summary>
    /// <param name="type">The type from the enumeration</param>
    public void SetPieceType(PieceType type) {
        pieceType = type;

        // Set the piece's attributes
        switch (type) {
            case PieceType.KNIGHT:
                value = 325;
                possibleMoves = new Vector2[8];
                possibleMoves[0] = new Vector2(1, 2);
                possibleMoves[1] = new Vector2(2, 1);
                possibleMoves[2] = new Vector2(2, -1);
                possibleMoves[3] = new Vector2(1, -2);
                possibleMoves[4] = new Vector2(-1, 2);
                possibleMoves[5] = new Vector2(-2, 1);
                possibleMoves[6] = new Vector2(-2, -1);
                possibleMoves[7] = new Vector2(-1, -2);
                break;
            case PieceType.BISHOP:
                value = 325;
                possibleMoves = new Vector2[4];
                possibleMoves[0] = new Vector2(1, 1);
                possibleMoves[1] = new Vector2(1, -1);
                possibleMoves[2] = new Vector2(-1, 1);
                possibleMoves[3] = new Vector2(-1, -1);
                break;
            case PieceType.ROOK:
                value = 500;
                possibleMoves = new Vector2[4];
                possibleMoves[0] = new Vector2(1, 0);
                possibleMoves[1] = new Vector2(0, 1);
                possibleMoves[2] = new Vector2(-1, 0);
                possibleMoves[3] = new Vector2(0, -1);
                break;
            case PieceType.QUEEN:
                value = 975;
                possibleMoves = new Vector2[8];
                possibleMoves[0] = new Vector2(1, 0);
                possibleMoves[1] = new Vector2(0, 1);
                possibleMoves[2] = new Vector2(-1, 0);
                possibleMoves[3] = new Vector2(0, -1);
                possibleMoves[4] = new Vector2(1, 1);
                possibleMoves[5] = new Vector2(1, -1);
                possibleMoves[6] = new Vector2(-1, 1);
                possibleMoves[7] = new Vector2(-1, -1);
                break;
            case PieceType.KING:
                value = 100000;
                possibleMoves = new Vector2[8];
                possibleMoves[0] = new Vector2(1, 0);
                possibleMoves[1] = new Vector2(0, 1);
                possibleMoves[2] = new Vector2(-1, 0);
                possibleMoves[3] = new Vector2(0, -1);
                possibleMoves[4] = new Vector2(1, 1);
                possibleMoves[5] = new Vector2(1, -1);
                possibleMoves[6] = new Vector2(-1, 1);
                possibleMoves[7] = new Vector2(-1, -1);
                break;
            case PieceType.PAWN:
            default:
                value = 100;
                possibleMoves = new Vector2[1];
                possibleMoves[0] = new Vector2(0, 1);
                break;
        }

        // Set the sprite based on the color and type
        if (white) {
            GetComponent<SpriteRenderer>().sprite = whiteSprite[(int)pieceType];
        } else {
            GetComponent<SpriteRenderer>().sprite = blackSprite[(int)pieceType];
        }
    }

    /// <summary>
    /// Changes the color and sprite to white.
    /// </summary>
    public void SetWhite() {
        white = true;
        GetComponent<SpriteRenderer>().sprite = whiteSprite[(int)pieceType];
    }

    /// <summary>
    /// Changes the color and sprite to black.
    /// </summary>
    public void SetBlack() {
        white = false;
        GetComponent<SpriteRenderer>().sprite = blackSprite[(int)pieceType];
    }

    /// <summary>
    /// Sets the cell of the sprite.
    /// </summary>
    /// <param name="row">The row of the cell</param>
    /// <param name="column">The column of the cell</param>
    public void SetCell(int row, int column) {
        xCoord = row;
        yCoord = column;
        Sprite WhiteSquare = FindObjectOfType<ChessBoard>().WhiteSquare;
        transform.localPosition = new Vector3(xCoord * WhiteSquare.bounds.size.x, yCoord * WhiteSquare.bounds.size.x);
    }

    /// <summary>
    /// Gets the cell of the piece in cell coordinates.
    /// </summary>
    /// <returns>The cell coordinates in a Vector2</returns>
    public Vector2 GetCell() {
        return new Vector2(xCoord, yCoord);
    }
}
