using UnityEngine;
using System.Collections;

public class MinMaxAI : MonoBehaviour {
    public GameObject board;

    private const int DEPTH = 3;
    private Vector4 move;
    private bool white;
    private int estimationValue;
    private int stepCounter = 0;
    private int estCost = 0;
    private int estAttackCost = 0;

    public Vector4 MakeMove(bool white) {
        MinMax(white, DEPTH);
        return move;
    }

    public void MinMax(bool white, int depth) {
        this.white = white;
        estimationValue = 0;
        if (white) {
            estimationValue = Max(depth);
        } else {
            estimationValue = Min(depth);
        }
    }

    private int Max(int depth) {
        if (depth == 0) {
            return Estimate();
        }

        int best = -int.MaxValue;

        return 0;
    }

    private int Min(int depth) {
        return 0;
    }

    private int GetCost() {
        int output = 0;

        foreach (GameObject piece in board.GetComponent<ChessBoard>().pieces) {
            output += piece.GetComponent<ChessPiece>().value * (piece.GetComponent<ChessPiece>().white ? 1 : -1);
        }

        return output;
    }

    private int GetAttackCost() {
        int output = 0;

        foreach (GameObject piece in board.GetComponent<ChessBoard>().pieces) {
            Vector2 cell = piece.GetComponent<ChessPiece>().GetCell();
            ChessBoardSquare square = board.GetComponent<ChessBoard>().GetSquareInCell((int)cell.x, (int)cell.y).GetComponent<ChessBoardSquare>();
            ArrayList moveableSquares = square.GetAttackedSquares(piece.GetComponent<ChessPiece>(), cell);
            foreach (GameObject move in moveableSquares) {
                cell = board.GetComponent<ChessBoard>().GetCell(move);
                GameObject pieceAtTarget = board.GetComponent<ChessBoard>().GetPieceInCell((int)cell.x, (int)cell.y);
                output += pieceAtTarget.GetComponent<ChessPiece>().value * (pieceAtTarget.GetComponent<ChessPiece>().white ? 1 : -1);
            }
        }

        return -output;
    }

    private int Estimate() {
        ++stepCounter;

        int dc = GetCost() - estCost;
        int dac = GetAttackCost() - estAttackCost;

        return dc * 10 + dac;
    }
}
