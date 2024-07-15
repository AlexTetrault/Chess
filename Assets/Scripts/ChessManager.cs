using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessManager : MonoBehaviour
{
    public StockfishInterface stockfish;

    void Start()
    {

    }

    public async void GetBestMoveFromStockfish(string fen)
    {
        string bestMove = await stockfish.GetBestMove(fen, 5);
        Debug.Log("Best Move: " + bestMove);
        // Implement the move in your game logic here
    }
}
