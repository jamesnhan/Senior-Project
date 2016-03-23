/**
 * Name:        ChessBoard
 * Author:      James Nhan
 */
 
using UnityEngine;
using System.Collections;

// TODO: Some of the logic here should be moved to ChessBoard

/// <summary>
/// This class defines the behaviour of a chess board square.
/// </summary>
public class ChessBoardSquare : MonoBehaviour {
    /// <summary>
    /// This is the game board component of the parent game object.
    /// </summary>
    private ChessBoard board;

    /// <summary>
    /// This is the square's renderer component.
    /// </summary>
    private Renderer spriteRenderer;

    /// <summary>
    /// An ArrayList that contains all of the squares that can be moved to.
    /// </summary>
    private static ArrayList movableSquares = new ArrayList();

    /// <summary>
    /// The intialization of a board square object
    /// </summary>
    void Start() {
        // Get the parent's ChessBoard object and this object's renderer
        board = gameObject.transform.parent.gameObject.GetComponent<ChessBoard>();
        spriteRenderer = GetComponent<Renderer>();
    }

    /// <summary>
    /// When the mouse coordinates overlap the object's collider boundaries, highlight
    /// the square.
    /// </summary>
    private void OnMouseEnter() {
        // Highlight a selectable square
        // But don't highlight any squares when the game is showing eligible moves
        if (!board.tryingToMove) {
            spriteRenderer.material.color = Color.red;
        }
    }

    /// <summary>
    /// When the mouse coordinates no longer overlap the object's collider boundaries,
    /// unhighlight the square.
    /// </summary>
    private void OnMouseExit() {
        // Unhighlight the square we're no longer hovering over
        // Unless the game is showing eligible moves
        if (!board.tryingToMove) {
            spriteRenderer.material.color = Color.white;
        }
    }

    /// <summary>
    /// Retreive a list of squares that a piece is attacking based on its cell coordinates.
    /// </summary>
    /// <param name="piece">The piece that is attacking</param>
    /// <param name="cell">The piece's cell coordinates</param>
    /// <returns>An ArrayList of squares that are in the attack range of the piece</returns>
    public ArrayList GetAttackedSquares(ChessPiece piece, Vector2 cell) {
        ArrayList squares = new ArrayList();

        // For each possible move direction as defined by the rules for the piece's type
        foreach (Vector2 move in piece.GetComponent<ChessPiece>().possibleMoves) {
            Vector2 position = cell;
            
            // White moves upwards and black moves downwards
            if (piece.GetComponent<ChessPiece>().white) {
                position += move;
            } else {
                position -= move;
            }

            // Check for its type so we can extend the range if it is a Bishop, Rook, or Queen
            switch (piece.GetComponent<ChessPiece>().GetPieceType()) {
                case ChessPiece.PieceType.BISHOP:
                case ChessPiece.PieceType.ROOK:
                case ChessPiece.PieceType.QUEEN:
                    // Move until we are out of bounds on either the x or y axis
                    while (position.x < ChessBoard.WIDTH && position.x >= 0 && position.y < ChessBoard.HEIGHT && position.y >= 0) {
                        // Attempt to highlight a square
                        if (HighlightSquare(position.x, position.y, piece.gameObject)) {
                            // Add the square to the ArrayList
                            GameObject square = board.GetSquareInCell((int)position.x, (int)position.y);
                            squares.Add(square);
                        } else {
                            // If we can't highlight a square, then we are done in this direction
                            break;
                        }

                        // Bishops, Rooks, and Queens can't hop over other pieces
                        if (board.GetPieceInCell((int)position.x, (int)position.y) != null) {
                            break;
                        }

                        // Advance a space in the direction
                        if (piece.GetComponent<ChessPiece>().white) {
                            position += move;
                        } else {
                            position -= move;
                        }
                    }
                    break;
                case ChessPiece.PieceType.PAWN:
                default:
                    // Highlight the square in the direction of movement
                    if (HighlightSquare(position.x, position.y, piece.gameObject)) {
                        GameObject square = board.GetSquareInCell((int)position.x, (int)position.y);
                        squares.Add(square);
                    }
                    break;
            }
        }
        // Special case for pawns' diagonal attack pattern and double first move
        if (piece.GetComponent<ChessPiece>().GetPieceType() == ChessPiece.PieceType.PAWN) {
            // If it's the pawn's first move
            if (piece.GetComponent<ChessPiece>().firstMove) {
                // Highlight the square two spaces ahead
                Vector2 position = cell;
                if (piece.GetComponent<ChessPiece>().white) {
                    position += 2 * Vector2.up;
                } else {
                    position += 2 * Vector2.down;
                }
                if (HighlightSquare(position.x, position.y, piece.gameObject)) {
                    GameObject square = board.GetSquareInCell((int)position.x, (int)position.y);
                    squares.Add(square);
                }
            }

            Vector2 front = cell;
            // Get the cell directly in front of the piece
            if (piece.GetComponent<ChessPiece>().white) {
                front += Vector2.up;
            } else {
                front += Vector2.down;
            }

            // Find the front left, front right, and front squares
            GameObject frontLeft = board.GetPieceInCell((int)front.x - 1, (int)front.y);
            GameObject frontRight = board.GetPieceInCell((int)front.x + 1, (int)front.y);
            GameObject frontStraight = board.GetPieceInCell((int)front.x, (int)front.y);

            // If there is an enemy piece in the front left, highlight it
            if (frontLeft != null) {
                if (HighlightSquare(front.x - 1, front.y, piece.gameObject)) {
                    GameObject square = board.GetSquareInCell((int)front.x - 1, (int)front.y);
                    squares.Add(square);
                }
            }

            // If there is an enemy piece in the front right, highlight it
            if (frontRight != null) {
                if (HighlightSquare(front.x + 1, front.y, piece.gameObject)) {
                    GameObject square = board.GetSquareInCell((int)front.x + 1, (int)front.y);
                    squares.Add(square);
                }
            }

            // If there is a piece in the front, unhighlight it because pawns can only attack diagonally
            if (frontStraight != null) {
                UnhighlightSquare(front.x, front.y);
            }
        }

        return squares;
    }

    /// <summary>
    /// Actions upon clicking a square
    /// </summary>
    private void OnMouseDown() {
        // TODO: Extract the logic into two separate methods to shorten this
        // If we are clicking for the first time
        if (!board.tryingToMove) {
            // Highlight the possible moves
            // Get the cell of the clicked square
            Vector2 cell = board.GetCell(gameObject);
            // Get the piece in the cell
            GameObject piece = board.GetPieceInCell((int)cell.x, (int)cell.y);
            // Ensure there is a piece we can move in the cell
            if (piece != null) {
                if (piece.GetComponent<ChessPiece>().white != board.currentPlayer) {
                    return;
                }
                // The next click should attempt to move the piece
                board.tryingToMove = true;
                board.movingPiece = piece;
                // Highlight the clicked square as green
                spriteRenderer.material.color = Color.green;
                // Set up the ArrayList of squares we can move to
                movableSquares.Clear();
                movableSquares = GetAttackedSquares(piece.GetComponent<ChessPiece>(), cell);
                // Ensure we don't accidentally move to the same square
                movableSquares.Remove(board.GetSquareInCell((int)cell.x, (int)cell.y));
            }
        } else {
            // Attempt to move the piece into the new cell
            Vector2 cell = board.GetCell(gameObject);
            GameObject piece = board.movingPiece;

            // Ensure we can actually move to that space
            if (movableSquares.Contains(board.GetSquareInCell((int)cell.x, (int)cell.y))) {
                // Check to see if the player is in check
                ArrayList attackedSquares = new ArrayList();
                GameObject kingPiece = null;

                // Get a list of all squares being attacked by the enemy pieces
                foreach (GameObject p in board.pieces) {
                    if (p.GetComponent<ChessPiece>().white != piece.GetComponent<ChessPiece>().white) {
                        ArrayList temp = GetAttackedSquares(p.GetComponent<ChessPiece>(), p.GetComponent<ChessPiece>().GetCell());
                        attackedSquares.AddRange(temp);
                    } else if (p.GetComponent<ChessPiece>().GetPieceType() == ChessPiece.PieceType.KING) {
                        // Get this player's king at the same time
                        kingPiece = p;
                    }
                }

                // See if the king piece is in the set of squares
                if (kingPiece != null) {
                    Vector2 c = kingPiece.GetComponent<ChessPiece>().GetCell();
                    if (attackedSquares.Contains(board.GetSquareInCell((int)c.x, (int)c.y))) {
                        if (kingPiece.GetComponent<ChessPiece>().white) {
                            board.check.Set(0, true);
                        } else {
                            board.check.Set(1, true);
                        }
                        // TODO: There has to be a better way to do this
                        // TODO: Check if the move would put the king out of check
                        goto ResetBoard;
                    } else {
                        if (kingPiece.GetComponent<ChessPiece>().white) {
                            board.check.Set(0, false);
                        } else {
                            board.check.Set(1, false);
                        }
                    }
                }

                // Check to see if there is an enemy piece there
                GameObject targetPiece = board.GetPieceInCell((int)cell.x, (int)cell.y);
                if (targetPiece != null) {
                    // If so, kill it
                    board.KillPiece(targetPiece.gameObject);
                }

                // Move the piece
                piece.GetComponent<ChessPiece>().SetCell((int)cell.x, (int)cell.y);

                // If it was the first move for the piece, it no longer is
                if (piece.GetComponent<ChessPiece>().firstMove) {
                    piece.GetComponent<ChessPiece>().firstMove = false;
                }

                // Switch players
                board.currentPlayer = !board.currentPlayer;
            }

            ResetBoard:
            // Reset the click status
            board.tryingToMove = false;
            board.movingPiece = null;

            // Unhighlight any squares that were previously highlighted
            foreach (GameObject square in board.squares) {
                Vector2 squarePos = board.GetCell(square);
                UnhighlightSquare(squarePos.x, squarePos.y);
            }

            movableSquares.Clear();
        }
    }

    /// <summary>
    /// Highlight a square and add it to the ArrayList of potential destination squares.
    /// </summary>
    /// <param name="posX">The square's x position in cell coordinates.</param>
    /// <param name="posY">The square's y position in cell coordinates.</param>
    /// <param name="piece">The piece to find enemies of.</param>
    /// <returns>True if the square was successfully highlighted, and false if the square
    /// was not successfully highlighted.</returns>
    private bool HighlightSquare(float posX, float posY, GameObject piece) {
        // TODO: Streamline the return paths
        // Ensure coordinates are clamped
        if (posX < ChessBoard.WIDTH && posX >= 0 && posY < ChessBoard.HEIGHT && posY >= 0) {
            // Find the object currently occupying the cell
            GameObject occupying = board.GetPieceInCell((int)posX, (int)posY);
            // Ensure it exists and then check whether or not it's an enemy
            if (occupying == null || occupying.GetComponent<ChessPiece>().white != piece.GetComponent<ChessPiece>().white) {
                // If it is an enemy, highlight the square and return true
                GameObject square = board.GetSquareInCell((int)posX, (int)posY);
                square.GetComponent<SpriteRenderer>().material.color = Color.green;
                return true;
            } else {
                // Else return false
                return false;
            }
        }
        // Return false if the coordinates are not clamped
        return false;
    }

    /// <summary>
    /// Unhighlight a square and remove it from the ArrayList of potential destination squares.
    /// </summary>
    /// <param name="posX">The square's x position in cell coordinates</param>
    /// <param name="posY">The square's y position in cell coordinates</param>
    private void UnhighlightSquare(float posX, float posY) {
        // Ensure coordinates are clamped
        if (posX < ChessBoard.WIDTH && posX >= 0 && posY < ChessBoard.HEIGHT && posY >= 0) {
            // Get the square in the cell coordinates, unhighlight it, and remove it from the ArrayList
            GameObject square = board.GetSquareInCell((int)posX, (int)posY);
            square.GetComponent<SpriteRenderer>().material.color = Color.white;
            movableSquares.Remove(square);
        }
    }
}
