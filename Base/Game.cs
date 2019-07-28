using System;

namespace UnityChess {
	/// <summary>Representation of a standard chess game including a history of moves made.</summary>
	public class Game {
		public Mode Mode { get; }
		public Side CurrentTurnSide { get; private set; }
		public int LatestHalfMoveIndex => HalfMoveTimeline.HeadIndex;
		public GameConditions StartingConditions { get; }
		public Timeline<Board> BoardTimeline { get; }
		public Timeline<HalfMove> HalfMoveTimeline { get; }
		
		private readonly Timeline<Square> enPassantCaptureSquareTimeline;

		/// <summary>Creates a Game instance of a given mode with a standard starting Board.</summary>
		/// <param name="mode">Describes which players are human or AI.</param>
		/// <param name="startingConditions">Conditions at the time the board was set up.</param>
		public Game(Mode mode, GameConditions startingConditions) {
			Mode = mode;
			CurrentTurnSide = Side.White;
			StartingConditions = startingConditions;
			BoardTimeline = new Timeline<Board>();
			HalfMoveTimeline = new Timeline<HalfMove>();
			enPassantCaptureSquareTimeline = new Timeline<Square>();
			
			BoardTimeline.AddNext(new Board());
			enPassantCaptureSquareTimeline.AddNext(Square.Invalid);
			UpdateAllPiecesLegalMoves(BoardTimeline.Current, enPassantCaptureSquareTimeline.Current, Side.White);
		}

		private Board LatestBoard => BoardTimeline.Current;
		
		/// <summary>Executes passed move and switches sides; also adds move to history.</summary>
		public bool TryExecuteMove(Movement move) {
			if (!TryGetLegalMove(move.Start, move.End, out Movement validatedMove)) return false;
			
			//create new copy of previous current board, and execute the move on it
			Board boardBeforeMove = LatestBoard;
			Square enPassantEligibleSquare = boardBeforeMove[validatedMove.Start] is Pawn pawn && Math.Abs(validatedMove.End.Rank - validatedMove.Start.Rank) == 2 ?
				                          new Square(validatedMove.End, 0, pawn.Color == Side.White ? -1 : 1) :
				                          Square.Invalid;
			enPassantCaptureSquareTimeline.AddNext(enPassantEligibleSquare);

			Board resultingBoard = new Board(LatestBoard);
			resultingBoard.MovePiece(validatedMove);

			BoardTimeline.AddNext(resultingBoard);

			CurrentTurnSide = CurrentTurnSide.Complement();
			
			UpdateAllPiecesLegalMoves(resultingBoard, enPassantCaptureSquareTimeline.Current, CurrentTurnSide);

			bool capturedPiece = boardBeforeMove[validatedMove.End] != null || validatedMove is EnPassantMove;
			bool causedCheckmate = Rules.IsPlayerCheckmated(resultingBoard, CurrentTurnSide);
			bool causedStalemate = Rules.IsPlayerStalemated(resultingBoard, CurrentTurnSide);
			bool causedCheck = Rules.IsPlayerInCheck(resultingBoard, CurrentTurnSide) && !causedCheckmate;

			HalfMoveTimeline.AddNext(new HalfMove(boardBeforeMove[validatedMove.Start], validatedMove, capturedPiece, causedCheck, causedStalemate , causedCheckmate));

			return true;
		}
		
		public bool TryGetLegalMove(Square startSquare, Square endSquare, out Movement move) {
			Piece movingPiece = LatestBoard[startSquare];

			if (movingPiece == null) {
				move = null;
				return false;
			}

			bool foundMove = movingPiece.LegalMoves.TryGetLegalMove(startSquare, endSquare, out move);
			return movingPiece.Color == CurrentTurnSide && foundMove;
		}

		public bool ResetGameToHalfMoveIndex(int halfMoveIndex) {
			if (LatestHalfMoveIndex == -1) return false; // i.e. No possible move to reset to

			BoardTimeline.HeadIndex = halfMoveIndex + 1;
			enPassantCaptureSquareTimeline.HeadIndex = halfMoveIndex + 1;
			HalfMoveTimeline.HeadIndex = halfMoveIndex;
			CurrentTurnSide = halfMoveIndex % 2 == 0 ? Side.Black : Side.White;
			
			UpdateAllPiecesLegalMoves(BoardTimeline.Current, enPassantCaptureSquareTimeline.Current, CurrentTurnSide);
			return true;
		}

		private static void UpdateAllPiecesLegalMoves(Board board, Square enPassantEligibleSquare, Side turn) {
			for (int file = 1; file <= 8; file++)
				for (int rank = 1; rank <= 8; rank++) {
					Piece piece = board[file, rank];
					if (piece == null) continue;
						if (piece.Color == turn) piece.UpdateLegalMoves(board, enPassantEligibleSquare);
						else piece.LegalMoves.Clear();
				}
		}
	}
}