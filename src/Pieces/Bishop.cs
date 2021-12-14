namespace UnityChess {
	public class Bishop : Piece<Bishop> {
		public Bishop() : base(Square.Invalid, Side.None) {}
		public Bishop(Square startingPosition, Side owner) : base(startingPosition, owner) {}

		public override void UpdateLegalMoves(Board board, GameConditions gameConditions) {
			CheckDiagonalDirections(board);
		}

		private void CheckDiagonalDirections(Board board) {
			foreach (Square offset in SquareUtil.DiagonalOffsets) {
				Square testSquare = Position + offset;

				while (testSquare.IsValid()) {
					Movement testMove = new Movement(Position, testSquare);

					if (Rules.MoveObeysRules(board, testMove, Owner)) {
						LegalMoves.Add(new Movement(testMove));
					}
					
					if (board.IsOccupiedAt(testSquare)) {
						break;
					}

					testSquare += offset;
				}
			}
		}
	}
}