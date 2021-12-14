namespace UnityChess {
	public class Queen : Piece<Queen> {
		public Queen() : base(Square.Invalid, Side.None) {}
		public Queen(Square startingPosition, Side owner) : base(startingPosition, owner) {}

		public override void UpdateLegalMoves(Board board, GameConditions gameConditions) {
			CheckRoseDirections(board);
		}

		private void CheckRoseDirections(Board board) {
			foreach (Square offset in SquareUtil.SurroundingOffsets) {
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