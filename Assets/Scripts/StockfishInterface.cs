using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class StockfishInterface : MonoBehaviour
{
    private Process stockfishProcess;
    private StreamWriter stockfishInput;
    private StreamReader stockfishOutput;
    private bool isRunning;

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

    public async Task<string> GetBestMove(string fen)
    {
        if (!isRunning)
        {
            //Debug.LogError("Stockfish process is not running.");
            return string.Empty;
        }

        await SendCommand("position fen " + fen);
        await SendCommand("go depth 3"); // Adjust depth as needed  

        string bestMove = string.Empty;
        while (true)
        {
            string output = await stockfishOutput.ReadLineAsync();
            if (output.StartsWith("bestmove"))
            {
                bestMove = output.Split(' ')[1];
                break;
            }
        }

        return bestMove;
    }

    private async Task SendCommand(string command)
    {
        if (!isRunning)
        {
            //Debug.LogError("Stockfish process is not running.");
            return;
        }

        stockfishInput.WriteLine(command);
        await stockfishInput.FlushAsync();
    }

    void OnDestroy()
    {
        if (stockfishProcess != null && !stockfishProcess.HasExited)
        {
            stockfishInput.WriteLine("quit");
            stockfishProcess.Close();
        }
    }
}
