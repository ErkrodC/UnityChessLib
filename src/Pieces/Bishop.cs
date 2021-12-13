namespace UnityChess {
	public class Bishop : Piece {
		public Bishop(Square startingPosition, Side owningSide) : base(startingPosition, owningSide) {}
		public Bishop(Bishop bishopCopy) : base(bishopCopy) {}

		public override void UpdateLegalMoves(Board board, GameConditions gameConditions) {
			CheckDiagonalDirections(board);
		}

		private void CheckDiagonalDirections(Board board) {
			Square enemyKingPosition = OwningSide == Side.White
				? board.BlackKing.Position
				: board.WhiteKing.Position;
			
			foreach (Square offset in SquareUtil.DiagonalOffsets) {
				Square testSquare = Position + offset;

				while (testSquare.IsValid()) {
					Movement testMove = new Movement(Position, testSquare);
					
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