namespace UnityChess {
	public class Rook : Piece {
		public Rook(Square startingPosition, Side owningSide) : base(startingPosition, owningSide) {}
		public Rook(Rook rookCopy) : base(rookCopy) {}
		
		public override void UpdateLegalMoves(Board board, Square enPassantEligibleSquare) {
			LegalMoves.Clear();
			CheckCardinalDirections(board);
		}

		private void CheckCardinalDirections(Board board) {
			foreach (Square offset in SquareUtil.CardinalOffsets) {
				Square testSquare = Position + offset;

				while (testSquare.IsValid()) {
					Movement testMove = new Movement(Position, testSquare);
					
					Square enemyKingPosition = OwningSide == Side.White
						? board.BlackKing.Position
						: board.WhiteKing.Position;
					
					if (board.IsOccupiedAt(testSquare)) {
						if (!board.IsOccupiedBySideAt(testSquare, OwningSide)
						    && Rules.MoveObeysRules(board, testMove, OwningSide)
						    && testSquare != enemyKingPosition
						) {
							LegalMoves.Add(new Movement(testMove));
						}

						break;
					}

					if (Rules.MoveObeysRules(board, testMove, OwningSide)
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