namespace UnityChess {
	public class Pawn : Piece {
		public Pawn(Square startingPosition, Side owningSide) : base(startingPosition, owningSide) {}
		public Pawn(Pawn pawnCopy) : base(pawnCopy) {}

		public override void UpdateLegalMoves(Board board, Square enPassantEligibleSquare) {
			LegalMoves.Clear();

			CheckForwardMovingSquares(board);
			CheckAttackingSquares(board);
			CheckEnPassantCaptures(board, enPassantEligibleSquare);
		}

		private void CheckForwardMovingSquares(Board board) {
			int advancingDirection = OwningSide == Side.White ? 1 : -1;
			Square testSquare = new Square(Position, 0, advancingDirection);
			Movement testMove = new Movement(Position, testSquare);
			
			if (!board.IsOccupied(testSquare) && Rules.MoveObeysRules(board, testMove, OwningSide)) {
				bool amOnSecondToLastRank = Position.Rank == (OwningSide == Side.White ? 7 : 2);
				LegalMoves.Add(amOnSecondToLastRank ? new PromotionMove(Position, testSquare) : new Movement(testMove));
			}
			
			if (!HasMoved) {
				testSquare = new Square(testSquare, 0, advancingDirection);
				testMove = new Movement(Position, testSquare);
				if (!board.IsOccupied(testSquare) && Rules.MoveObeysRules(board, testMove, OwningSide))
					LegalMoves.Add(new Movement(testMove));
			}
		}

		private void CheckAttackingSquares(Board board) {
			foreach (int fileOffset in new[] {-1, 1}) {
				int rankOffset = OwningSide == Side.White ? 1 : -1;
				Square testSquare = new Square(Position, fileOffset, rankOffset);
				Movement testMove = new Movement(Position, testSquare);

				Square enemyKingPosition = OwningSide == Side.White ? board.BlackKing.Position : board.WhiteKing.Position;
				if (testSquare.IsValid() && board.IsOccupiedBySide(testSquare, OwningSide.Complement()) && Rules.MoveObeysRules(board, testMove, OwningSide) && testSquare != enemyKingPosition) {
					bool pawnAtSecondToLastRank = Position.Rank == (OwningSide == Side.White ? 7 : 2);
					Movement move = pawnAtSecondToLastRank ? new PromotionMove(Position, testSquare) : new Movement(testMove);
					LegalMoves.Add(move);
				}
			}
		}

		private void CheckEnPassantCaptures(Board board, Square enPassantEligibleSquare) {
			if (OwningSide == Side.White ? Position.Rank == 5 : Position.Rank == 4) {
				foreach (int fileOffset in new[] {-1, 1}) {
					Square lateralSquare = new Square(Position, fileOffset, 0);

					if (lateralSquare.IsValid() && board[lateralSquare] is Pawn enemyLateralPawn && enemyLateralPawn.OwningSide != OwningSide) {
						Square squareToCheckWithEligibleSquare = new Square(enemyLateralPawn.Position, 0, enemyLateralPawn.OwningSide == Side.White ? -1 : 1);
						if (squareToCheckWithEligibleSquare.Equals(enPassantEligibleSquare)) {
							EnPassantMove testMove = new EnPassantMove(Position, enPassantEligibleSquare, enemyLateralPawn);

							if (Rules.MoveObeysRules(board, testMove, OwningSide))
								LegalMoves.Add(new EnPassantMove(Position, enPassantEligibleSquare, enemyLateralPawn));
						}
					}
				}
			}
		}
	}
}