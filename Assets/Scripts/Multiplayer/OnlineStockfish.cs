using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class OnlineStockfish : MonoBehaviour
{
    private Process stockfishProcess;
    private StreamWriter stockfishInput;
    private StreamReader stockfishOutput;
    private bool isRunning;

    public OnlineGameManager gameManager;

    void Start()
    {
        StartStockfish();
    }

    void StartStockfish()
    {
        stockfishProcess = new Process();
        stockfishProcess.StartInfo.FileName = Application.streamingAssetsPath + "/Stockfish/stockfish.exe"; // Adjust path as needed
        stockfishProcess.StartInfo.RedirectStandardInput = true;
        stockfishProcess.StartInfo.RedirectStandardOutput = true;
        stockfishProcess.StartInfo.UseShellExecute = false;
        stockfishProcess.StartInfo.CreateNoWindow = true;
        stockfishProcess.Start();

        stockfishInput = stockfishProcess.StandardInput;
        stockfishOutput = stockfishProcess.StandardOutput;

        stockfishInput.WriteLine("uci");
        stockfishInput.Flush();

        // Read initial responses from Stockfish
        while (true)
        {
            string line = stockfishOutput.ReadLine();
            if (line.Contains("uciok")) break;
        }

        isRunning = true;
    }

    //ensure stockfish terminates when we stop the game.
    void OnDestroy()
    {
        if (stockfishProcess != null && !stockfishProcess.HasExited)
        {
            stockfishInput.WriteLine("quit");
            stockfishProcess.Close();
        }
    }

    //create a send command function so we can tell stockfish to do things.
    public async Task SendCommand(string command)
    {
        try
        {
            if (!isRunning)
            {
                UnityEngine.Debug.LogError("Stockfish process is not running.");
                return;
            }

            await stockfishInput.WriteLineAsync(command);
            await stockfishInput.FlushAsync();
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"IOException occurred: {ex.Message}");
        }
    }

    public async Task<string> CheckIfMate(string fen)
    {
        if (!isRunning)
        {
            UnityEngine.Debug.LogError("Stockfish process is not running.");
            return string.Empty;
        }

        await SendCommand("ucinewgame");
        await SendCommand("position fen " + fen);
        await SendCommand("go depth 1");

        string output = await GetStockfishResponse();

        if (output.Contains("score cp 0"))
        {
            gameManager.StaleMate();
            return "stalemate";
        }
        else
        {
            gameManager.DetermineWinner();
            return "checkmate";
        }
    }

    private async Task<string> GetStockfishResponse()
    {
        string output = "";
        UnityEngine.Debug.Log("Waiting for Stockfish response...");
        while (true)
        {
            string line = await stockfishOutput.ReadLineAsync();
            if (string.IsNullOrEmpty(line))
                break;

            output += line + "\n";
            UnityEngine.Debug.Log("Received line from Stockfish: " + line);

            // Stockfish will finish after sending 'bestmove' or 'info'
            if (line.StartsWith("info depth"))
                break;
        }
        UnityEngine.Debug.Log("Stockfish analysis completed.");
        return output;
    }

    public async Task<List<string>> CalculateLegalMoveList(string fenCode)
    {
        //ensure we always start with a fresh list of legal moves.
        gameManager.legalMoves.Clear();

        //check for possible run error
        List<string> legalMoveList = new List<string>();

        if (!isRunning)
        {
            UnityEngine.Debug.LogError("Stockfish process is not running.");
        }

        //provide stockfish with the current fenCode and ask it to calculate all legal moves.
        await SendCommand($"position fen {fenCode}");
        await SendCommand("go perft 1");

        //add all legal moves to the legal move list in the game manager.
        while (true)
        {
            string output = await stockfishOutput.ReadLineAsync();

            if (output.StartsWith("Nodes searched"))
            {
                //signifies the end of the search, no need to continue.
                break;
            }

            if (output.Contains(": "))
            {
                string legalMove = output.Substring(0, 4);
                legalMoveList.Add(legalMove);
            }
        }

        return legalMoveList;
    }
}
