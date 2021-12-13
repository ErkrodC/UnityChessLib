namespace UnityChess {
	public class Pawn : Piece {
		private static readonly int[] adjacentFileOffsets = {-1, 1};
		
		public Pawn(Square startingPosition, Side owningSide) : base(startingPosition, owningSide) {}
		public Pawn(Pawn pawnCopy) : base(pawnCopy) {}

		public override void UpdateLegalMoves(Board board, GameConditions gameConditions) {
			CheckForwardMovingSquares(board);
			CheckAttackingSquares(board);
			CheckEnPassantCaptures(board, gameConditions.EnPassantSquare);
		}

		private void CheckForwardMovingSquares(Board board) {
			int forwardDirection = OwningSide.ForwardDirection();
			Square testSquare = new Square(Position, 0, forwardDirection);
			Movement testMove = new Movement(Position, testSquare);
			
			if (!board.IsOccupiedAt(testSquare)
			    && Rules.MoveObeysRules(board, testMove, OwningSide)
			) {
				LegalMoves.Add(
					Position.Rank == OwningSide.Complement().PawnRank()
						? new PromotionMove(Position, testSquare)
						: new Movement(testMove)
				);
			}
			
			if (Position.Rank == OwningSide.PawnRank()) {
				testSquare += new Square(0, forwardDirection);
				testMove = new Movement(Position, testSquare);
				if (!board.IsOccupiedAt(testSquare)
				    && Rules.MoveObeysRules(board, testMove, OwningSide)
				) {
					LegalMoves.Add(new Movement(testMove));
				}
			}
		}

		private void CheckAttackingSquares(Board board) {
			foreach (int fileOffset in adjacentFileOffsets) {
				Square testSquare = Position + new Square(fileOffset, OwningSide.ForwardDirection());
				Movement testMove = new Movement(Position, testSquare);

				Square enemyKingPosition = OwningSide == Side.White
					? board.BlackKing.Position
					: board.WhiteKing.Position;
				
				if (testSquare.IsValid()
				    && board.IsOccupiedBySideAt(testSquare, OwningSide.Complement())
				    && Rules.MoveObeysRules(board, testMove, OwningSide)
				    && testSquare != enemyKingPosition
				) {
					LegalMoves.Add(
						Position.Rank == OwningSide.Complement().PawnRank()
							? new PromotionMove(Position, testSquare)
							: new Movement(testMove)
					);
				}
			}
		}

		private void CheckEnPassantCaptures(Board board, Square enPassantEligibleSquare) {
			int enPassantCaptureRank = OwningSide == Side.White ? 5 : 4;
			if (Position.Rank != enPassantCaptureRank) {
				return;
			}

			Square lateralSquare = enPassantEligibleSquare + new Square(0, -OwningSide.ForwardDirection());
			if (lateralSquare.IsValid()
			    && board[lateralSquare] is Pawn enemyPawn
			    && enemyPawn.OwningSide != OwningSide
			    && Rules.MoveObeysRules(board, new EnPassantMove(Position, enPassantEligibleSquare, enemyPawn), OwningSide)
			) {
				LegalMoves.Add(new EnPassantMove(Position, enPassantEligibleSquare, enemyPawn));
			}
		}
	}
}