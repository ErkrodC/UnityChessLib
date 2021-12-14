namespace UnityChess {
	public class Knight : Piece<Knight> {
		public Knight() : base(Square.Invalid, Side.None) {}
		public Knight(Square startingPosition, Side owner) : base(startingPosition, owner) {}

		public override void UpdateLegalMoves(Board board, GameConditions gameConditions) {
			CheckKnightSquares(board);
		}

		private void CheckKnightSquares(Board board) {
			foreach (Square offset in SquareUtil.KnightOffsets) {
				Movement testMove = new Movement(Position, Position + offset);

				if (Rules.MoveObeysRules(board, testMove, Owner)) {
					LegalMoves.Add(new Movement(testMove));
				}
			}
		}
	}
}