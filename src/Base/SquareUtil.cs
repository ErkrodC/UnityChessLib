using System.Collections.Generic;

namespace UnityChess {
	public static class SquareUtil {
		public static readonly Dictionary<string, int> FileCharToIntMap = new Dictionary<string, int> {
			{"a", 1},
			{"b", 2},
			{"c", 3},
			{"d", 4},
			{"e", 5},
			{"f", 6},
			{"g", 7},
			{"h", 8}
		};
		
		public static readonly Dictionary<int, string> FileIntToCharMap = new Dictionary<int, string> {
			{1, "a"},
			{2, "b"},
			{3, "c"},
			{4, "d"},
			{5, "e"},
			{6, "f"},
			{7, "g"},
			{8, "h"}
		};
	
		public static string SquareToString(Square square) => SquareToString(square.File, square.Rank);
		public static string SquareToString(int file, int rank) {
			if (FileIntToCharMap.TryGetValue(file, out string fileChar)) {
				return $"{fileChar}{rank}";
			}

			return "Invalid";
		}

		public static Square StringToSquare(string squareText) {
			return new Square(
				FileCharToIntMap[squareText[0].ToString()],
				int.Parse(squareText[1].ToString())
			);
		}
	}
}