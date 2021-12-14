namespace UnityChess {
	public class Pawn : Piece<Pawn> {
		private static readonly int[] adjacentFileOffsets = {-1, 1};
		
		public Pawn() : base(Square.Invalid, Side.None) {}
		public Pawn(Square startingPosition, Side owner) : base(startingPosition, owner) {}

		public override void UpdateLegalMoves(Board board, GameConditions gameConditions) {
			CheckForwardMovingSquares(board);
			CheckAttackingSquares(board);
			CheckEnPassantCaptures(board, gameConditions.EnPassantSquare);
		}

		private void CheckForwardMovingSquares(Board board) {
			int forwardDirection = Owner.ForwardDirection();
			Square testSquare = new Square(Position, 0, forwardDirection);
			Movement testMove = new Movement(Position, testSquare);
			
			if (!board.IsOccupiedAt(testSquare)
			    && Rules.MoveObeysRules(board, testMove, Owner)
			) {
				LegalMoves.Add(
					Position.Rank == Owner.Complement().PawnRank()
						? new PromotionMove(Position, testSquare)
						: new Movement(testMove)
				);
			}
			
			if (Position.Rank == Owner.PawnRank()) {
				testSquare += new Square(0, forwardDirection);
				testMove = new Movement(Position, testSquare);
				if (!board.IsOccupiedAt(testSquare)
				    && Rules.MoveObeysRules(board, testMove, Owner)
				) {
					LegalMoves.Add(new Movement(testMove));
				}
			}
		}

		private void CheckAttackingSquares(Board board) {
			foreach (int fileOffset in adjacentFileOffsets) {
				Square testSquare = Position + new Square(fileOffset, Owner.ForwardDirection());
				Movement testMove = new Movement(Position, testSquare);

				Square enemyKingPosition = Owner == Side.White
					? board.BlackKing.Position
					: board.WhiteKing.Position;
				
				if (testSquare.IsValid()
				    && board.IsOccupiedBySideAt(testSquare, Owner.Complement())
				    && Rules.MoveObeysRules(board, testMove, Owner)
				    && testSquare != enemyKingPosition
				) {
					LegalMoves.Add(
						Position.Rank == Owner.Complement().PawnRank()
							? new PromotionMove(Position, testSquare)
							: new Movement(testMove)
					);
				}
			}
		}

		private void CheckEnPassantCaptures(Board board, Square enPassantEligibleSquare) {
			int enPassantCaptureRank = Owner == Side.White ? 5 : 4;
			if (Position.Rank != enPassantCaptureRank) {
				return;
			}

			Square lateralSquare = enPassantEligibleSquare + new Square(0, -Owner.ForwardDirection());
			if (lateralSquare.IsValid()
			    && board[lateralSquare] is Pawn enemyPawn
			    && enemyPawn.Owner != Owner
			    && Rules.MoveObeysRules(board, new EnPassantMove(Position, enPassantEligibleSquare, enemyPawn), Owner)
			) {
				LegalMoves.Add(new EnPassantMove(Position, enPassantEligibleSquare, enemyPawn));
			}
		}
	}
}