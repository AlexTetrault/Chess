using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.PunBasics;
using UnityEngine.UI;

public class OnlineGameManager : MonoBehaviourPunCallbacks
{
    public List<GameObject> blackPieces = new List<GameObject>();
    public List<GameObject> whitePieces = new List<GameObject>();

    public GameObject blackKing;
    public GameObject whiteKing;

    public Canvas blackWins;
    public Canvas whiteWins;
    public Canvas staleMate;

    public GameObject opponentIndicator;

    public PhotonView photonView;

    public GameObject enPassantVictim;
    public GameObject startGameButton;

    public GameObject clientWaitingScreen;
    public GameObject masterWaitingScreen;
    public GameObject masterWaitingBanner;

    public GameObject infectIndicator;

    [HideInInspector] public GameObject movingSquare;
    [HideInInspector] public GameObject destinationSquare;
    [HideInInspector] public List<GameObject> destinationSquares = new List<GameObject>();
    [HideInInspector] public List<string> legalMoves = new List<string>();
    [HideInInspector] public bool gameHasStarted;
    [HideInInspector] public List<GameObject> availSquares;
    [HideInInspector] public List<Vector2> possibleMoves = new List<Vector2>();
    [HideInInspector] public bool isPlayersMove;
    [HideInInspector] public string moveCode;
    [HideInInspector] public bool isWhitesMove;

    public OnlineFenCalculator fenCalculator;
    public OnlineChessBoard chessBoard;
    public OnlineGameOptions gameOptions;

    int layerMask = 1 << 7;

    private void Start()
    {
        isWhitesMove = true;
        possibleMoves.Clear();

        photonView = GetComponent<PhotonView>();

        //master client is always white, therefore it is their move at the start of the game. 
        isPlayersMove = PhotonNetwork.IsMasterClient ? true : false;

        if (PhotonNetwork.IsMasterClient)
        {
            masterWaitingScreen.SetActive(true);
        }
        else
        {
            clientWaitingScreen.SetActive(true);
        }
     }

    public void ChangeTurn()
    {
        isWhitesMove = isWhitesMove ? false : true;
    }

    public void AttackEnemyPiece(GameObject activePiece)
    {
        Vector3 activePiecePos = activePiece.transform.position;
        bool pieceIsWhite = activePiece.GetComponent<ChessPiece>().isWhite;

        //if moving piece is white, check to see if its position is the same as any of the black pieces.
        if (pieceIsWhite)
        {
            foreach (GameObject blackPiece in blackPieces)
            {
                Vector3 blackPiecePos = blackPiece.transform.position;
                if (activePiecePos == blackPiecePos)
                {
                    //moving to a black piece, take black piece off of the board. 
                    blackPiece.GetComponent<SpriteRenderer>().enabled = false;
                    blackPiece.GetComponent<Collider>().enabled = false;

                    //attacking an opponents piece resets the half move counter to 0.
                    fenCalculator.halfMoveNumber = 0;

                    blackPieces.Remove(blackPiece);
                    chessBoard.chessPieces.Remove(blackPiece);

                    //game logic for infect-chess.
                    if (!gameOptions.isInfect)
                    {
                        break;
                    }

                    //kings cannot be infected.
                    if (activePiece.tag == "King")
                    {
                        break;
                    }

                    if (blackPiece.tag == "Pawn")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = gameOptions.whitePawn;
                    }
                    if (blackPiece.tag == "Rook")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = gameOptions.whiteRook;
                    }
                    if (blackPiece.tag == "Knight")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = gameOptions.whiteKnight;
                    }
                    if (blackPiece.tag == "Bishop")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = gameOptions.whiteBishop;
                    }
                    if (blackPiece.tag == "Queen")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = gameOptions.whiteQueen;
                    }

                    activePiece.tag = blackPiece.tag;
                    activePiece.name = blackPiece.name.ToUpper();

                    break;
                }
            }
        }

        //if moving piece is black, check to see if its position is the same as any of the white pieces.
        else
        {
            foreach (GameObject whitePiece in whitePieces)
            {
                Vector3 whitePiecePos = whitePiece.transform.position;
                if (activePiecePos == whitePiecePos)
                {
                    //moving to a white piece, take white piece off of the board. 
                    whitePiece.GetComponent<SpriteRenderer>().enabled = false;
                    whitePiece.GetComponent<Collider>().enabled = false;

                    //attacking an opponents piece resets the half move counter to 0.
                    fenCalculator.halfMoveNumber = 0;

                    whitePieces.Remove(whitePiece);
                    chessBoard.chessPieces.Remove(whitePiece);

                    //game logic for infect-chess.
                    if (!gameOptions.isInfect)
                    {
                        break;
                    }

                    //kings cannot be infected.
                    if (activePiece.tag == "King")
                    {
                        break;
                    }

                    if (whitePiece.tag == "Pawn")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = gameOptions.blackPawn;
                    }
                    if (whitePiece.tag == "Rook")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = gameOptions.blackRook;
                    }
                    if (whitePiece.tag == "Knight")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = gameOptions.blackKnight;
                    }
                    if (whitePiece.tag == "Bishop")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = gameOptions.blackBishop;
                    }
                    if (whitePiece.tag == "Queen")
                    {
                        activePiece.GetComponent<SpriteRenderer>().sprite = gameOptions.blackQueen;
                    }

                    activePiece.tag = whitePiece.tag;
                    activePiece.name = whitePiece.name.ToLower();

                    break;
                }
            }
        }
    }

    [PunRPC]
    public void GenerateOpponentMove(string moveCode)
    {
        //find the sqaure the opponent wants to move from and to.
        movingSquare = GameObject.Find(moveCode.Substring(0, 2));
        destinationSquare = GameObject.Find(moveCode.Substring(2, 2));

        //get reference to the chess piece the opponent wants to move and move it to the same x and y position as the destination square.
        RaycastHit hit;
        Vector3 rayOrigin = new Vector3(movingSquare.transform.position.x, movingSquare.transform.position.y, -5);

        Vector2 newPos = new Vector2(destinationSquare.transform.localPosition.x, destinationSquare.transform.localPosition.y);

        if (Physics.Raycast(rayOrigin, Vector3.forward, out hit, Mathf.Infinity, layerMask))
        {
            hit.collider.GetComponent<OnlinePieceDrag>().initialPos = hit.collider.transform.localPosition;
            hit.collider.GetComponent<OnlinePieceDrag>().GenerateMove(newPos);
            isPlayersMove = true;
        }
    }

    [PunRPC]
    public void TakeEnPassantVictim()
    {
        enPassantVictim.GetComponent<SpriteRenderer>().enabled = false;
        enPassantVictim.GetComponent<Collider>().enabled = false;
        chessBoard.chessPieces.Remove(enPassantVictim);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        opponentIndicator.SetActive(true);
        startGameButton.SetActive(true);
        masterWaitingBanner.SetActive(false);
    }

    [PunRPC]
    public void CloseClientWaitingScreen()
    {
        clientWaitingScreen.SetActive(false);
    }

    [PunRPC]
    public void SetGameToInfect()
    {
        gameOptions.isInfect = true;
        infectIndicator.SetActive(true);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        opponentIndicator.SetActive(false);
    }

    public void DetermineWinner()
    {
        Canvas winner = isWhitesMove ? blackWins : whiteWins;
        winner.enabled = true;
    }

    public void StaleMate()
    {
        staleMate.enabled = true;
    }
}
