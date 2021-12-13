namespace UnityChess {
	public class King : Piece {
		private static readonly int[] rookFiles = { 1, 8 };
		
		public King(Square startingPosition, Side owningSide) : base(startingPosition, owningSide) {}
		public King(King kingCopy) : base(kingCopy) {}

		public override void UpdateLegalMoves(Board board, GameConditions gameConditions) {
			CheckSurroundingSquares(board);
			CheckCastlingMoves(board);
		}

		private void CheckSurroundingSquares(Board board) {
			foreach (Square offset in SquareUtil.SurroundingOffsets) {
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

		private void CheckCastlingMoves(Board board) {
			if (HasMoved || Rules.IsPlayerInCheck(board, OwningSide)) {
				return;
			}

			int castlingRank = OwningSide.CastlingRank();
			
			foreach (int rookFile in rookFiles) {
				if (board[rookFile, castlingRank] is not Rook rook
				    || rook.OwningSide != OwningSide
				    || rook.HasMoved
				) {
					continue;
				}

				bool checkingQueenside = rookFile == 1;
				Square inBetweenSquare0 = new Square(checkingQueenside ? 4 : 6, castlingRank);
				Square inBetweenSquare1 = new Square(checkingQueenside ? 3 : 7, castlingRank);
				Square inBetweenSquare2 = new Square(2, castlingRank);
				Movement inBetweenMove0 = new Movement(Position, inBetweenSquare0);
				Movement inBetweenMove1 = new Movement(Position, inBetweenSquare1);
				
				if (!board.IsOccupiedAt(inBetweenSquare0)
				    && !board.IsOccupiedAt(inBetweenSquare1)
				    && (!board.IsOccupiedAt(inBetweenSquare2) || !checkingQueenside)
				    && Rules.MoveObeysRules(board, inBetweenMove0, OwningSide)
				    && Rules.MoveObeysRules(board, inBetweenMove1, OwningSide)
				) {
					LegalMoves.Add(new CastlingMove(Position, inBetweenSquare1, rook));
				}
			}
		}
	}
}