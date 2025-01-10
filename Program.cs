using System.Diagnostics;

namespace CircularDominoChain
{
    internal class Program
    {
        /*==========Challenge
            Given a random set of dominos, compute a way to order the set in such a way that they form a correct circular domino chain. For every stone the dots on one half of a stone match the dots on the neighboring half of an adjacent stone.
            For example given the stones [2|1], [2|3] and [1|3] you should compute something like [1|2] [2|3] [3|1] or [3|2] [2|1] [1|3] or [1|3] [3|2] [2|1] etc, where the first and last numbers are the same meaning they’re in a circle.
            For stones [1|2], [4|1] and [2|3] the resulting chain is not valid: [4|1] [1|2] [2|3]'s first and last numbers are not the same so it’s not a circle.

            Write a program in C# which computes the chain for a random set of dominos. If a circular chain is not possible the program should output this.
        */
        private static bool _printDebugInfo = true;
        static void Main(string[] args)
        {
            // for empty pips please use 0
            var dominosValid3Stones = new List<int[]> {
                new int[2] { 2, 1 },
                new int[2] { 1, 3 },
                new int[2] { 2, 3 }
            };

            var dominosValid5Stones = new List<int[]> {
                new int[2] { 0, 2 },
                new int[2] { 2, 1 },
                new int[2] { 3, 4 },
                new int[2] { 1, 3 },
                new int[2] { 0, 4 }
            };

            var dominosInvalid3Stones = new List<int[]> {
                new int[2] { 4, 1 },
                new int[2] { 1, 2 },
                new int[2] { 2, 3 }
            };

            var dominosInvalid16Stones = new List<int[]>
            {
                new int[2] { 0, 1 },
                new int[2] { 0, 2 },
                new int[2] { 0, 3 },
                new int[2] { 0, 4 },
                new int[2] { 0, 5 },
                new int[2] { 1, 1 },
                new int[2] { 1, 2 },
                new int[2] { 1, 4 },
                new int[2] { 2, 2 },
                new int[2] { 2, 3 },
                new int[2] { 2, 4 },
                new int[2] { 2, 5 },
                new int[2] { 3, 1 },
                new int[2] { 3, 3 },
                new int[2] { 3, 4 },
                new int[2] { 3, 5 },
            };

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            try
            {
                PrintDominos(dominosValid5Stones, "original: ", false);
                var ordered = Solution(dominosValid5Stones);
                PrintDominos(ordered, "first found of valid circular domino chain: ", false);

                PrintDominos(dominosValid3Stones, "original: ", false);
                ordered = Solution(dominosValid3Stones);
                PrintDominos(ordered, "first found of valid circular domino chain: ", false);

                PrintDominos(dominosInvalid16Stones, "original: ", false);
                ordered = Solution(dominosInvalid16Stones);
                PrintDominos(ordered, "first found of valid circular domino chain: ", false);

                PrintDominos(dominosInvalid3Stones, "original: ", false);
                ordered = Solution(dominosInvalid3Stones);
                PrintDominos(ordered, "first found of valid circular domino chain: ", false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed);
        }

        public static List<int[]> Solution(List<int[]> dominos)
        {
            var stoneHalfCounts = dominos
                                    .SelectMany(array => array)
                                    .GroupBy(n => n);
            if (stoneHalfCounts.Any(item => item.Count() % 2 != 0))
                throw new InvalidDataException("Impossible to correctly order set with odd count of same domino halfs");

            if (dominos.Count < 3)
                throw new InvalidDataException("Impossible to correctly order set of less than 3 dominos");

            List<int[]>? orderedDominos = null;
            for (int i = 0; i < dominos.Count; i++)
            {
                orderedDominos = OrderDominos(dominos, dominos[i], dominos[i]);
                if (orderedDominos.Count == dominos.Count && orderedDominos[0][0] == orderedDominos[orderedDominos.Count - 1][1])
                {
                    return orderedDominos;
                }

                orderedDominos = OrderDominos(dominos, dominos[i], [dominos[i][1], dominos[i][0]]);
                if (orderedDominos.Count == dominos.Count && orderedDominos[0][0] == orderedDominos[orderedDominos.Count - 1][1])
                {
                    return orderedDominos;
                }
            }

            throw new InvalidDataException("Impossible to correctly order set");
        }

        public static List<int[]> OrderDominos(List<int[]> dominos, int[] originalDomino, int[] currentDomino)
        {
            var usedDominos = new HashSet<int[]>() { originalDomino };
            var orderedDominos = new List<int[]> { currentDomino };
            var foundDomino = true;
            var j = 0;

            while (dominos.Count != usedDominos.Count && (j < dominos.Count || foundDomino))
            {
                if (j == dominos.Count)
                {
                    foundDomino = false;
                    j = 0;
                }

                var tmpDomino = dominos[j];
                if (!usedDominos.Contains(tmpDomino))
                {
                    if (currentDomino[1] == tmpDomino[0])
                    {
                        usedDominos.Add(tmpDomino);
                        currentDomino = tmpDomino;
                        orderedDominos.Add(currentDomino);
                        foundDomino = true;
                    }
                    else if (currentDomino[1] == tmpDomino[1])
                    {
                        usedDominos.Add(tmpDomino);
                        currentDomino = [tmpDomino[1], tmpDomino[0]];
                        orderedDominos.Add(currentDomino);
                        foundDomino = true;
                    }
                }

                j++;
            }

            PrintDominos(orderedDominos, $"tried to order starting from [{usedDominos.First()[0]}|{usedDominos.First()[1]}]: ");
            return orderedDominos;
        }

        public static void PrintDominos(List<int[]> dominos, string message, bool debugInfo = true)
        {
            if (!_printDebugInfo && debugInfo)
                return;

            Console.Write(message);
            foreach (var domino in dominos)
            {
                Console.Write($"[{domino[0]}|{domino[1]}] ");
            }

            Console.WriteLine();
        }
    }
}
