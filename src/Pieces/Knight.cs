namespace UnityChess {
	public class Knight : Piece<Knight> {
		public Knight() : base(Square.Invalid, Side.None) {}
		public Knight(Square startingPosition, Side owner) : base(startingPosition, owner) {}

		public override void UpdateLegalMoves(Board board, GameConditions gameConditions) {
			CheckKnightSquares(board);
		}

		private void CheckKnightSquares(Board board) {
			foreach (Square offset in SquareUtil.KnightOffsets) {
				Square testSquare = Position + offset;
				Movement testMove = new Movement(Position, testSquare);
				Square enemyKingPosition = Owner == Side.White
					? board.BlackKing.Position
					: board.WhiteKing.Position;
				
				if (testSquare.IsValid()
				    && !board.IsOccupiedBySideAt(testSquare, Owner)
				    && Rules.MoveObeysRules(board, testMove, Owner)
				    && testSquare != enemyKingPosition
				) {
					LegalMoves.Add(new Movement(testMove));
				}
			}
		}
	}
}