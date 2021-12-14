namespace UnityChess {
	public class Rook : Piece<Rook> {
		public Rook() : base(Square.Invalid, Side.None) {}
		public Rook(Square startingPosition, Side owner) : base(startingPosition, owner) {}

		public override void UpdateLegalMoves(Board board, GameConditions gameConditions) {
			CheckCardinalDirections(board);
		}

		private void CheckCardinalDirections(Board board) {
			foreach (Square offset in SquareUtil.CardinalOffsets) {
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