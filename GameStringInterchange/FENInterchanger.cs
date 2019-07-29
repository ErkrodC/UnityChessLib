using System;

namespace UnityChess {
	public class FENInterchanger : IGameStringInterchanger {
		public string Export(Game game) {
			Board currentBoard = game.BoardTimeline.Current;
			GameConditions currentGameConditions =
				game.StartingConditions.CalculateEndingGameConditions(currentBoard,
					game.HalfMoveTimeline.GetStartToCurrent());

			return ConvertCurrentGameStateToFEN(currentBoard, currentGameConditions);
		}

		// TODO implement
		public Game Import(string fen) {
			throw new NotImplementedException();
		}

		private static string ConvertCurrentGameStateToFEN(Board currentBoard, GameConditions currentGameConditions) {
			string toMoveString = currentGameConditions.WhiteToMove ? "w" : "b";

			string enPassantSquareString = currentGameConditions.EnPassantSquare.IsValid
				? SquareUtil.SquareToString(currentGameConditions.EnPassantSquare)
				: "-";

			return
				$"{CalculateBoardString(currentBoard)} {toMoveString} {CalculateCastlingInfoString(currentGameConditions)} {enPassantSquareString} {currentGameConditions.HalfMoveClock} {currentGameConditions.TurnNumber}";
		}

		private static string CalculateBoardString(Board currentBoard) {
			string[] rankStrings = new string[8];
			for (int rank = 1; rank <= 8; rank++) {
				int emptySquareCount = 0;
				int rankStringsIndex = 7 - (rank - 1);
				rankStrings[rankStringsIndex] = "";
				for (int file = 1; file <= 8; file++) {
					Piece piece = currentBoard[file, rank];
					if (piece == null) {
						emptySquareCount++;

						if (file == 8) { // reached end of rank, append empty square count to rankString
							rankStrings[rankStringsIndex] += emptySquareCount;
							emptySquareCount = 0;
						}
					} else {
						if (emptySquareCount > 0) { // found piece, append empty square count to rankString
							rankStrings[rankStringsIndex] += emptySquareCount;
							emptySquareCount = 0;
						}

						rankStrings[rankStringsIndex] += GetFENPieceSymbol(piece);
					}
				}
			}

			return string.Join("/", rankStrings);
		}

		private static string GetFENPieceSymbol(Piece piece) {
			bool isWhitePiece = piece.Color == Side.White;

			if (piece is Bishop)	{ return isWhitePiece ? "B" : "b"; } // NOTE discards not supported in Mono 6.0.0.0
			if (piece is King)		{ return isWhitePiece ? "K" : "k"; }
			if (piece is Knight)	{ return isWhitePiece ? "N" : "n"; }
			if (piece is Pawn)		{ return isWhitePiece ? "P" : "p"; }
			if (piece is Queen)		{ return isWhitePiece ? "Q" : "q"; }
			if (piece is Rook)		{ return isWhitePiece ? "R" : "r"; }

			throw new ArgumentException();
		}

		private static string CalculateCastlingInfoString(GameConditions currentGameConditions) {
			bool anyCastlingAvailable = currentGameConditions.WhiteCanCastleKingside ||
			                            currentGameConditions.WhiteCanCastleQueenside ||
			                            currentGameConditions.BlackCanCastleKingside ||
			                            currentGameConditions.BlackCanCastleQueenside;
			string castlingInfoString = "";

			if (anyCastlingAvailable) {
				if (currentGameConditions.WhiteCanCastleKingside) { castlingInfoString += "K"; }
				if (currentGameConditions.WhiteCanCastleQueenside) { castlingInfoString += "Q"; }
				if (currentGameConditions.BlackCanCastleKingside) { castlingInfoString += "k"; }
				if (currentGameConditions.BlackCanCastleQueenside) { castlingInfoString += "q"; }
			} else castlingInfoString = "-";

			return castlingInfoString;
		}
	}
}