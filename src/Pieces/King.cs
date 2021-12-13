namespace UnityChess {
	public class King : Piece {
		private static readonly int[] rookFiles = { 1, 8 };
		
		public King(Square startingPosition, Side owner) : base(startingPosition, owner) {}
		public King(King kingCopy) : base(kingCopy) {}

		public override void UpdateLegalMoves(Board board, GameConditions gameConditions) {
			CheckSurroundingSquares(board);

			if (Owner == Side.White) {
				CheckCastlingMoves(
					board,
					gameConditions.WhiteCanCastleKingside,
					gameConditions.WhiteCanCastleQueenside
				);
			} else {
				CheckCastlingMoves(
					board,
					gameConditions.BlackCanCastleKingside,
					gameConditions.BlackCanCastleQueenside
				);
			}
		}

		private void CheckSurroundingSquares(Board board) {
			foreach (Square offset in SquareUtil.SurroundingOffsets) {
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

		private void CheckCastlingMoves(Board board, bool canCastleKingSide, bool canCastleQueenside) {
			if (Rules.IsPlayerInCheck(board, Owner)
				|| !canCastleKingSide && !canCastleQueenside
			) { return; }

			int castlingRank = Owner.CastlingRank();
			
			foreach (int rookFile in rookFiles) {
				bool checkingQueenside = rookFile == 1;
				
				if (board[rookFile, castlingRank] is not Rook rook
				    || rook.Owner != Owner
				    || checkingQueenside && !canCastleQueenside
				    || !checkingQueenside && !canCastleKingSide
				) {
					continue;
				}
				
				Square inBetweenSquare0 = new Square(checkingQueenside ? 4 : 6, castlingRank);
				Square inBetweenSquare1 = new Square(checkingQueenside ? 3 : 7, castlingRank);
				Square inBetweenSquare2 = new Square(2, castlingRank);
				Movement inBetweenMove0 = new Movement(Position, inBetweenSquare0);
				Movement inBetweenMove1 = new Movement(Position, inBetweenSquare1);
				
				if (!board.IsOccupiedAt(inBetweenSquare0)
				    && !board.IsOccupiedAt(inBetweenSquare1)
				    && (!board.IsOccupiedAt(inBetweenSquare2) || !checkingQueenside)
				    && Rules.MoveObeysRules(board, inBetweenMove0, Owner)
				    && Rules.MoveObeysRules(board, inBetweenMove1, Owner)
				) {
					LegalMoves.Add(new CastlingMove(Position, inBetweenSquare1, rook));
				}
			}
		}
	}
}