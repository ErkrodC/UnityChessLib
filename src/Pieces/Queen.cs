namespace UnityChess {
	public class Queen : Piece {
		public Queen(Square startingPosition, Side owner) : base(startingPosition, owner) {}
		public Queen(Queen queenCopy) : base(queenCopy) {}

		public override void UpdateLegalMoves(Board board, GameConditions gameConditions) {
			CheckRoseDirections(board);
		}

		private void CheckRoseDirections(Board board) {
			Square enemyKingPosition = Owner == Side.White
				? board.BlackKing.Position
				: board.WhiteKing.Position;
			
			foreach (Square offset in SquareUtil.SurroundingOffsets) {
				Square testSquare = Position + offset;

				while (testSquare.IsValid()) {
					Movement testMove = new Movement(Position, testSquare);

					if (board.IsOccupiedAt(testSquare)) {
						if (!board.IsOccupiedBySideAt(testSquare, Owner)
						    && Rules.MoveObeysRules(board, testMove, Owner)
						    && testSquare != enemyKingPosition
						) {
							LegalMoves.Add(new Movement(testMove));
						}

						break;
					}

					if (Rules.MoveObeysRules(board, testMove, Owner)
					    && testSquare != enemyKingPosition
					) {
						LegalMoves.Add(new Movement(testMove));
					}

					testSquare += offset;
				}
			}
		}
	}
}