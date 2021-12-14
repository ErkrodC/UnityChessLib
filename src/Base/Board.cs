using System;

namespace UnityChess {
	/// <summary>An 8x8 matrix representation of a chessboard.</summary>
	public class Board {
		private readonly Piece[,] boardMatrix;
		private King whiteKing;
		private King blackKing;
		
		public Piece this[Square position] {
			get {
				if (position.IsValid()) return boardMatrix[position.File - 1, position.Rank - 1];
				throw new ArgumentOutOfRangeException($"Position was out of range: {position}");
			}

			set {
				if (position.IsValid()) boardMatrix[position.File - 1, position.Rank - 1] = value;
				else throw new ArgumentOutOfRangeException($"Position was out of range: {position}");
			}
		}

		public Piece this[int file, int rank] {
			get => this[new Square(file, rank)];
			set => this[new Square(file, rank)] = value;
		}

		/// <summary>Creates a Board with initial chess game position.</summary>
		public Board(params Piece[] pieces) {
			boardMatrix = new Piece[8, 8];
			
			foreach (Piece piece in pieces) {
				this[piece.Position] = piece;
			}
		}

		/// <summary>Creates a deep copy of the passed Board.</summary>
		public Board(Board board) {
			// TODO optimize this method
			// Creates deep copy (makes copy of each piece and deep copy of their respective ValidMoves lists) of board (list of BasePiece's)
			// this may be a memory hog since each Board has a list of Piece's, and each piece has a list of Movement's
			// avg number turns/Board's per game should be around ~80. usual max number of pieces per board is 32
			boardMatrix = new Piece[8, 8];
			for (int file = 1; file <= 8; file++) {
				for (int rank = 1; rank <= 8; rank++) {
					Piece pieceToCopy = board[file, rank];
					if (pieceToCopy == null) { continue; }

					this[file, rank] = pieceToCopy.DeepCopy();
				}
			}
		}

		public void ClearBoard() {
			for (int file = 1; file <= 8; file++) {
				for (int rank = 1; rank <= 8; rank++) {
					this[file, rank] = null;
				}
			}

			whiteKing = null;
			blackKing = null;
		}

		private static Piece[] startingPositionPieces;
		public static Piece[] GetStartingPositionPieces() {
			if (startingPositionPieces == null) {
				startingPositionPieces = new Piece[32];

				int filled = 0;

				//Row 2/Rank 7 and Row 7/Rank 2, both rows of pawns
				for (int file = 1; file <= 8; file++) {
					foreach (int rank in new[] { 2, 7 }) {
						Square position = new Square(file, rank);
						Side pawnColor = rank == 2
							? Side.White
							: Side.Black;
						startingPositionPieces[filled++] = new Pawn(position, pawnColor);
					}
				}

				//Rows 1 & 8/Ranks 8 & 1, back rows for both players
				for (int file = 1; file <= 8; file++) {
					foreach (int rank in new[] { 1, 8 }) {
						Square position = new Square(file, rank);
						Side pieceColor = rank == 1
							? Side.White
							: Side.Black;
						switch (file) {
							case 1:
							case 8:
								startingPositionPieces[filled++] = new Rook(position, pieceColor);
								break;
							case 2:
							case 7:
								startingPositionPieces[filled++] = new Knight(position, pieceColor);
								break;
							case 3:
							case 6:
								startingPositionPieces[filled++] = new Bishop(position, pieceColor);
								break;
							case 4:
								startingPositionPieces[filled++] = new Queen(position, pieceColor);
								break;
							case 5:
								startingPositionPieces[filled++] = new King(position, pieceColor);
								break;
						}
					}
				}
			}

			return startingPositionPieces;
		}

		public void MovePiece(Movement move) {
			if (!(this[move.Start] is Piece pieceToMove)) throw new ArgumentException($"No piece was found at the given position: {move.Start}");
			
			this[move.Start] = null;
			this[move.End] = pieceToMove;

			pieceToMove.Position = move.End;

			(move as SpecialMove)?.HandleAssociatedPiece(this);
		}
		
		internal bool IsOccupiedAt(Square position) => this[position] != null;

		internal bool IsOccupiedBySideAt(Square position, Side side) => this[position] is Piece piece && piece.Owner == side;

		public King GetKing(Side player) {
			if (whiteKing == null || blackKing == null) {
				for (int file = 1; file <= 8; file++) {
					for (int rank = 1; rank <= 8; rank++) {
						if (this[file, rank] is King king) {
							if (king.Owner == Side.White) {
								whiteKing = king;
							} else {
								blackKing = king;
							}
						}
					}
				}
			}

			return player switch {
				Side.White => whiteKing,
				Side.Black => blackKing,
				_ => null
			};
		}

		public string ToASCIIArt() {
			string result = string.Empty;
			
			for (int rank = 8; rank >= 1; --rank) {
				for (int file = 1; file <= 8; ++file) {
					Piece piece = this[file, rank];
					result += piece == null ? " " : FENSerializer.GetFENPieceSymbol(piece);
					result += file != 8
						? "|"
						: $"\t {rank}";
				}

				result += "\n";
			}
			
			result += "a b c d e f g h";

			return result;
		}
	}
}