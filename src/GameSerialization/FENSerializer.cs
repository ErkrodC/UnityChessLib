using System;

namespace UnityChess {
	public class FENSerializer : IGameSerializer {
		public string Serialize(Game game) {
			GameConditions currentConditions = game.ConditionsTimeline.Current;
			Square currentEnPassantSquare = currentConditions.EnPassantSquare;

			return
				$"{CalculateBoardString(game.BoardTimeline.Current)}"
				+ $" {(currentConditions.WhiteToMove ? "w" : "b")}"
				+ $" {CalculateCastlingInfoString(currentConditions)}"
				+ $" {(currentEnPassantSquare.IsValid() ? SquareUtil.SquareToString(currentEnPassantSquare) : "-")}"
				+ $" {currentConditions.HalfMoveClock}"
				+ $" {currentConditions.TurnNumber}";
		}
		
		// TODO implement
		public Game Deserialize(string fen) {
			throw new NotImplementedException();
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

		public static string GetFENPieceSymbol(Piece piece) {
			bool isWhite = piece?.OwningSide == Side.White;

			switch (piece) {
				case Bishop:
					return isWhite ? "B" : "b";
				case King:
					return isWhite ? "K" : "k";
				case Knight:
					return isWhite ? "N" : "n";
				case Pawn:
					return isWhite ? "P" : "p";
				case Queen:
					return isWhite ? "Q" : "q";
				case Rook:
					return isWhite ? "R" : "r";
				default:
					return string.Empty;
			}
		}

		private static string CalculateCastlingInfoString(GameConditions currentGameConditions) {
			bool anyCastlingAvailable = currentGameConditions.WhiteCanCastleKingside || currentGameConditions.WhiteCanCastleQueenside || currentGameConditions.BlackCanCastleKingside || currentGameConditions.BlackCanCastleQueenside;
			string castlingInfoString = "";

			if (anyCastlingAvailable) {
				(bool WhiteCanCastleKingside, string)[] castleFlagSymbolPairs = {
					(currentGameConditions.WhiteCanCastleKingside, "K"),
					(currentGameConditions.WhiteCanCastleQueenside, "Q"),
					(currentGameConditions.BlackCanCastleKingside, "k"),
					(currentGameConditions.BlackCanCastleQueenside, "q")
				};

				foreach ((bool canCastle, string fenCastleSymbol) in castleFlagSymbolPairs)
					if (canCastle) castlingInfoString += fenCastleSymbol;
			} else castlingInfoString = "-";

			return castlingInfoString;
		}
	}
}