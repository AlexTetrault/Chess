using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPieceClone : MonoBehaviour
{
    public bool isWhite;
    public bool hasMoved;
    public GameManager gameManager;
    Collider myCollider;
    int layerMask;

    private void Start()
    {
        myCollider = GetComponent<Collider>();
        layerMask = LayerMask.GetMask("Piece");
    }

    public void LineOfSight()
    {
        if (tag == "Pawn")
        {
            Pawn();
        }
        if (tag == "Rook")
        {
            Rook();
        }
        if (tag == "Bishop")
        {
            Bishop();
        }
        if (tag == "Queen")
        {
            Queen();
        }
        if (tag == "King")
        {
            King();
        }
        if (tag == "Knight")
        {
            Knight();
        }
    }

    void Pawn()
    {
        int frontRayDistance = hasMoved ? 1 : 2;
        Vector3 forward = isWhite ? Vector3.up : Vector3.down;
        Vector3 leftDiagonal = isWhite ? new Vector3(-0.5f, 0.5f, 0) : new Vector3(-0.5f, -0.5f, 0);
        Vector3 rightDiagonal = isWhite ? new Vector3(0.5f, 0.5f, 0) : new Vector3(0.5f, -0.5f, 0);

        myCollider.enabled = false;

        Ray ray = new Ray(transform.position, forward);
        Ray leftAttackRay = new Ray(transform.position, leftDiagonal);
        Ray rightAttackRay = new Ray(transform.position, rightDiagonal);

        RaycastHit hit;
        RaycastHit leftAttack;
        RaycastHit rightAttack;

        if (Physics.Raycast(ray, out hit, frontRayDistance))
        {
            Debug.Log(hit.collider);
            float spacesAvailable = Mathf.Abs(hit.transform.localPosition.y - transform.localPosition.y) - 1;
            for (int i = 1; i <= spacesAvailable; i++)
            {
                gameManager.possibleMoves.Add(new Vector2(0f, isWhite ? i : -i));
            }
        }
        else
        {
            for (int i = 1; i <= frontRayDistance; i++)
            {
                gameManager.possibleMoves.Add(new Vector2(0f, isWhite ? i : -i));
            }
        }

        if (Physics.Raycast(leftAttackRay, out leftAttack, 2))
        {
            if (leftAttack.collider.gameObject.GetComponent<ChessPiece>().isWhite != isWhite)
            {
                gameManager.possibleAttacks.Add(leftAttack.collider.gameObject);
                gameManager.possibleMoves.Add(new Vector2(-1, isWhite ? 1 : -1));
            }

        }

        if (Physics.Raycast(rightAttackRay, out rightAttack, 2))
        {
            if (rightAttack.collider.gameObject.GetComponent<ChessPiece>().isWhite != isWhite)
            {
                gameManager.possibleAttacks.Add(rightAttack.collider.gameObject);
                gameManager.possibleMoves.Add(new Vector2(1, isWhite ? 1 : -1));
            }
        }

        myCollider.enabled = true;
    }

    void Rook()
    {
        myCollider.enabled = false;

        RookAddMoves(Vector3.up, Vector2.up);
        RookAddMoves(Vector3.down, Vector2.down);
        RookAddMoves(Vector3.left, Vector2.left);
        RookAddMoves(Vector3.right, Vector2.right);

        myCollider.enabled = true;
    }

    void RookAddMoves(Vector3 direction, Vector2 moveDirection)
    {
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20))
        {
            float numberOfFreeSpaces = (direction == Vector3.up || direction == Vector3.down) ?
                Mathf.Abs(hit.collider.transform.localPosition.y - transform.localPosition.y) :
                Mathf.Abs(hit.collider.transform.localPosition.x - transform.localPosition.x);

            if (hit.collider.gameObject.GetComponent<ChessPiece>().isWhite != isWhite)
            {
                numberOfFreeSpaces++;
            }

            for (int i = 1; i < numberOfFreeSpaces; i++)
            {
                gameManager.possibleMoves.Add(moveDirection * i);
            }
        }
        else
        {
            for (int i = 1; i < 8; i++)
            {
                gameManager.possibleMoves.Add(moveDirection * i);
            }
        }
    }

    void Bishop()
    {
        myCollider.enabled = false;

        BishopAddMoves(Vector3.up + Vector3.right, new Vector2(1f, 1f));   // up-right
        BishopAddMoves(Vector3.up + Vector3.left, new Vector2(-1f, 1f));  // up-left
        BishopAddMoves(Vector3.down + Vector3.right, new Vector2(1f, -1f));    // down-right
        BishopAddMoves(Vector3.down + Vector3.left, new Vector2(-1f, -1f));    // down-left

        myCollider.enabled = true;
    }

    void BishopAddMoves(Vector3 direction, Vector2 moveDirection)
    {
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20))
        {
            float numberOfFreeSpaces = Mathf.Abs(hit.collider.transform.localPosition.y - transform.localPosition.y);

            if (hit.collider.gameObject.GetComponent<ChessPiece>().isWhite != isWhite)
            {
                numberOfFreeSpaces++;
            }

            for (int i = 1; i < numberOfFreeSpaces; i++)
            {
                gameManager.possibleMoves.Add(moveDirection * i);
            }
        }
        else
        {
            for (int i = 1; i < 8; i++)
            {
                gameManager.possibleMoves.Add(moveDirection * i);
            }
        }
    }

    void King()
    {
        myCollider.enabled = false;

        KingAddMoves(Vector3.up + Vector3.right, new Vector2(1f, 1f));   // up-right
        KingAddMoves(Vector3.up + Vector3.left, new Vector2(-1f, 1f));  // up-left
        KingAddMoves(Vector3.down + Vector3.right, new Vector2(1f, -1f));    // down-right
        KingAddMoves(Vector3.down + Vector3.left, new Vector2(-1f, -1f));    // down-left
        KingAddMoves(Vector3.up, Vector2.up);
        KingAddMoves(Vector3.down, Vector2.down);
        KingAddMoves(Vector3.left, Vector2.left);
        KingAddMoves(Vector3.right, Vector2.right);
        if (!hasMoved)
        {
            KingCheckForCastle(Vector3.right, Vector2.right);
            KingCheckForCastle(Vector3.left, Vector2.left);
        }

        myCollider.enabled = true;
    }

    void KingAddMoves(Vector3 direction, Vector2 moveDirection)
    {
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1.5f))
        {
            float numberOfFreeSpaces = (direction == Vector3.up || direction == Vector3.down) ?
                Mathf.Abs(hit.collider.transform.localPosition.y - transform.localPosition.y) :
                Mathf.Abs(hit.collider.transform.localPosition.x - transform.localPosition.x);

            if (hit.collider.gameObject.GetComponent<ChessPiece>().isWhite != isWhite)
            {
                numberOfFreeSpaces++;
            }

            for (int i = 1; i < numberOfFreeSpaces; i++)
            {
                gameManager.possibleMoves.Add(moveDirection * i);
            }
        }
        else
        {
            for (int i = 1; i <= 1; i++)
            {
                gameManager.possibleMoves.Add(moveDirection * i);
            }
        }
    }

    void KingCheckForCastle(Vector3 direction, Vector2 moveDirection)
    {
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5))
        {
            if (hit.collider.gameObject.tag == "Rook")
            {
                if (hit.collider.gameObject.GetComponent<ChessPiece>().hasMoved == false)
                {
                    float moveMagnitude = Mathf.Abs(transform.localPosition.x - hit.transform.localPosition.x) - 1;
                    Vector2 castleMove = moveDirection * moveMagnitude;
                    gameManager.possibleMoves.Add(castleMove);
                }
            }
        }
    }

    void Queen()
    {
        myCollider.enabled = false;

        QueenAddMoves(Vector3.up + Vector3.right, new Vector2(1f, 1f));   // up-right
        QueenAddMoves(Vector3.up + Vector3.left, new Vector2(-1f, 1f));  // up-left
        QueenAddMoves(Vector3.down + Vector3.right, new Vector2(1f, -1f));    // down-right
        QueenAddMoves(Vector3.down + Vector3.left, new Vector2(-1f, -1f));    // down-left
        QueenAddMoves(Vector3.up, Vector2.up);
        QueenAddMoves(Vector3.down, Vector2.down);
        QueenAddMoves(Vector3.left, Vector2.left);
        QueenAddMoves(Vector3.right, Vector2.right);

        myCollider.enabled = true;
    }

    void QueenAddMoves(Vector3 direction, Vector2 moveDirection)
    {
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20))
        {
            float numberOfFreeSpaces = (direction == Vector3.up || direction == Vector3.down) ?
                Mathf.Abs(hit.collider.transform.localPosition.y - transform.localPosition.y) :
                Mathf.Abs(hit.collider.transform.localPosition.x - transform.localPosition.x);

            if (hit.collider.gameObject.GetComponent<ChessPiece>().isWhite != isWhite)
            {
                numberOfFreeSpaces++;
            }

            for (int i = 1; i < numberOfFreeSpaces; i++)
            {
                gameManager.possibleMoves.Add(moveDirection * i);
            }
        }
        else
        {
            for (int i = 1; i < 8; i++)
            {
                gameManager.possibleMoves.Add(moveDirection * i);
            }
        }
    }

    void Knight()
    {
        myCollider.enabled = false;

        Vector3[] possibleMoves = new Vector3[]
        {
        new Vector3(2, 1, 0),
        new Vector3(2, -1, 0),
        new Vector3(-2, 1, 0),
        new Vector3(-2, -1, 0),
        new Vector3(1, 2, 0),
        new Vector3(1, -2, 0),
        new Vector3(-1, 2, 0),
        new Vector3(-1, -2, 0)
        };

        foreach (Vector2 move in possibleMoves)
        {
            Vector3 testPos = transform.localPosition + new Vector3(0, 0, -1) + new Vector3(move.x, move.y, 0);
            RaycastHit hit;

            if (Physics.Raycast(testPos, Vector3.forward, out hit, 2f, layerMask))
            {
                ChessPiece hitPiece = hit.collider.gameObject.GetComponent<ChessPiece>();
                if (hitPiece.isWhite != isWhite)
                {
                    gameManager.possibleMoves.Add(move);
                }
            }
            else
            {
                gameManager.possibleMoves.Add(move);
            }

        }

        myCollider.enabled = true;
    }

}
