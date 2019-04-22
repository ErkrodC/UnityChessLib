using System;

namespace UnityChess {
	/// <summary>Representation of a standard chess game including a history of moves made.</summary>
	public class Game {
		public Mode Mode { get; }
		public Side CurrentTurnSide { get; private set; }
		public int HalfMoveCount => PreviousMoves.HeadIndex;
		public GameConditions StartingConditions { get; }
		public Timeline<Board> BoardTimeline { get; }
		public Timeline<HalfMove> PreviousMoves { get; }

		private readonly Timeline<Square> enPassantCaptureSquareTimeline;

		/// <summary>Creates a Game instance of a given mode with a standard starting Board.</summary>
		/// <param name="mode">Describes which players are human or AI.</param>
		/// <param name="startingConditions">Conditions at the time the board was set up.</param>
		public Game(Mode mode, GameConditions startingConditions) {
			Mode = mode;
			CurrentTurnSide = Side.White;
			StartingConditions = startingConditions;
			BoardTimeline = new Timeline<Board>();
			PreviousMoves = new Timeline<HalfMove>();
			enPassantCaptureSquareTimeline = new Timeline<Square>();
			
			BoardTimeline.AddNext(new Board());
			enPassantCaptureSquareTimeline.AddNext(Square.Invalid);
			UpdateAllPiecesLegalMoves(BoardTimeline.Current, enPassantCaptureSquareTimeline.Current, Side.White);
		}

		private Board LatestBoard => BoardTimeline.Current;
		
		/// <summary>Executes passed move and switches sides; also adds move to history.</summary>
		public bool TryExecuteMove(Movement move) {
			if (!GetInternalLegalMoveIfPossible(ref move)) return false;
			
			//create new copy of previous current board, and execute the move on it
			Board boardBeforeMove = LatestBoard;
			Square enPassantEligibleSquare = boardBeforeMove[move.Start] is Pawn pawn && Math.Abs(move.End.Rank - move.Start.Rank) == 2 ?
				                          new Square(move.End, 0, pawn.Color == Side.White ? -1 : 1) :
				                          Square.Invalid;
			enPassantCaptureSquareTimeline.AddNext(enPassantEligibleSquare);

			Board resultingBoard = new Board(LatestBoard);
			resultingBoard.MovePiece(move);

			BoardTimeline.AddNext(resultingBoard);

			CurrentTurnSide = CurrentTurnSide.Complement();
			
			UpdateAllPiecesLegalMoves(resultingBoard, enPassantCaptureSquareTimeline.Current, CurrentTurnSide);

			bool capturedPiece = boardBeforeMove[move.End] != null || move is EnPassantMove;
			bool causedCheckmate = Rules.IsPlayerCheckmated(resultingBoard, CurrentTurnSide);
			bool causedStalemate = Rules.IsPlayerStalemated(resultingBoard, CurrentTurnSide);
			bool causedCheck = Rules.IsPlayerInCheck(resultingBoard, CurrentTurnSide) && !causedCheckmate;
			PreviousMoves.AddNext(new HalfMove(boardBeforeMove[move.Start], move, capturedPiece, causedCheck, causedStalemate , causedCheckmate));

			return true;
		}
		
		private bool GetInternalLegalMoveIfPossible(ref Movement move) {
			Piece movingPiece = LatestBoard[move.Start];
			if (movingPiece == null) return false;

			return movingPiece.Color == CurrentTurnSide && movingPiece.LegalMoves.TryGetLegalMoveUsingBaseMove(ref move);
		}

		public void ResetGameToHalfMoveIndex(int halfMoveIndex) {
			BoardTimeline.HeadIndex = halfMoveIndex + 1;
			enPassantCaptureSquareTimeline.HeadIndex = halfMoveIndex + 1;
			PreviousMoves.HeadIndex = halfMoveIndex;
			CurrentTurnSide = halfMoveIndex % 2 == 0 ? Side.Black : Side.White;
			
			UpdateAllPiecesLegalMoves(BoardTimeline.Current, enPassantCaptureSquareTimeline.Current, CurrentTurnSide);
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