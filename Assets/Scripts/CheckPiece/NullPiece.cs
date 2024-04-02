using System;
using System.Collections.Generic;
using UnityEngine;

public class NullPiece : ChessPiece
{
    protected override void Awake()
    {
        base.Awake();

        this.currX = -1;
        this.currY = -1;
        this.piece_Type = Chess_Piece_Type.NullPiece;
    }

    protected override List<Vector2Int> GetAllPossibleMove()
    {
        return new List<Vector2Int>();
    }
}