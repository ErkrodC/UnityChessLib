namespace UnityChess {
	public class Knight : Piece {
		public Knight(Square startingPosition, Side owningSide) : base(startingPosition, owningSide) {}
		public Knight(Knight knightCopy) : base(knightCopy) {}

		public override void UpdateLegalMoves(Board board, Square enPassantEligibleSquare) {
			LegalMoves.Clear();
			CheckKnightSquares(board);
		}

		private void CheckKnightSquares(Board board) {
			foreach (Square offset in SquareUtil.KnightOffsets) {
				Square testSquare = Position + offset;
				Movement testMove = new Movement(Position, testSquare);
				Square enemyKingPosition = OwningSide == Side.White
					? board.BlackKing.Position
					: board.WhiteKing.Position;
				
				if (testSquare.IsValid()
				    && !board.IsOccupiedBySideAt(testSquare, OwningSide)
				    && Rules.MoveObeysRules(board, testMove, OwningSide)
				    && testSquare != enemyKingPosition
				) {
					LegalMoves.Add(new Movement(testMove));
				}
			}
		}
	}
}