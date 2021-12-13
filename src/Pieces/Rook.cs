namespace UnityChess {
	public class Rook : Piece {
		public Rook(Square startingPosition, Side owner) : base(startingPosition, owner) {}
		public Rook(Rook rookCopy) : base(rookCopy) {}
		
		public override void UpdateLegalMoves(Board board, GameConditions gameConditions) {
			CheckCardinalDirections(board);
		}

		private void CheckCardinalDirections(Board board) {
			foreach (Square offset in SquareUtil.CardinalOffsets) {
				Square testSquare = Position + offset;

				while (testSquare.IsValid()) {
					Movement testMove = new Movement(Position, testSquare);
					
					Square enemyKingPosition = Owner == Side.White
						? board.BlackKing.Position
						: board.WhiteKing.Position;
					
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