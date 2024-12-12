public class ElevenToFifteen
{
    #region Day Eleven
    /// <summary>
    /// Solves the Day 11 portion of the AOC 2024 challenge.
    /// https://adventofcode.com/2024/day/11
    /// </summary>
    public void DayEleven()
    {
        // Flag to quickly switch inputs (and possibly other things)
        bool usingTestInput = false;

        // The input files
        string fileName = "/Users/jovinjovinsson/GitHub/Advent-of-Code-2024/AOC2024/assets/elevenToFifteen/AOC2024.11.";
        fileName += usingTestInput ? "Test-Input.txt" : "Input.txt";

        List<long> stoneLabels = new List<long>();

        // Read the data in from the text file
        using (StreamReader streamReader = new StreamReader(fileName))
        {
            // Placeholder for the current line of the tile
            string currentLine;

            // currentLine will be null when the StreamReader reaches the end of file
            while((currentLine = streamReader.ReadLine()) != null)
            {
                // Split the string with spaces and add it to our list
                string[] stoneStrings = currentLine.Split(" ");

                foreach (string stoneString in stoneStrings)
                {
                    stoneLabels.Add(long.Parse(stoneString));
                }
            }
        }

        OutputStones(ref stoneLabels, "Initial arrangement:");

        // Number of times we need to iterate, the test sample is much smaller
        // int numberOfBlinks = usingTestInput ? 6 : 25;
        int numberOfBlinks = 75;

        // long count = CalculateStoneCount(stoneLabels, numberOfBlinks);
        long count = CalculateTotalStones(stoneLabels, numberOfBlinks);

        Console.WriteLine("=============================");
        Console.WriteLine("=== Number of Stones: {0} ===", count);
        Console.WriteLine("=============================");
    }

    /// <summary>
    /// Simple function to output the lsit of stones
    /// </summary>
    /// <param name="stoneLabels"></param>
    /// <param name="title"></param>
    private void OutputStones(ref List<long> stoneLabels, string title)
    {
        Console.WriteLine("\n" + title);
        Console.WriteLine(string.Join(" ", stoneLabels.ToArray()));
    }

    /// <summary>
    /// Calculates the number of stones after a given number of transformations.
    /// </summary>
    /// <param name="filename">The input file containing the initial stones.</param>
    /// <param name="transformations">The number of transformations to perform.</param>
    /// <returns>The total number of stones after transformations.</returns>
    private static long CalculateStoneCount(List<long> stones, int numberOfBlinks)
    {
        // Perform the transformations.
        for (int i = 0; i < numberOfBlinks; i++)
        {
            List<long> transformedStones = new List<long>();

            foreach (long stone in stones)
            {
                if (stone == 0)
                {
                    // If the stone value is 0, replace it with 1.
                    transformedStones.Add(1);
                }
                else if (stone.ToString().Length % 2 == 0)
                {
                    // Split even-length stones into two halves.
                    string stoneStr = stone.ToString();
                    string leftPart = stoneStr.Substring(0, stoneStr.Length / 2);
                    string rightPart = stoneStr.Substring(stoneStr.Length / 2);
                    transformedStones.Add(int.Parse(leftPart));
                    transformedStones.Add(int.Parse(rightPart));
                }
                else
                {
                    // Multiply odd-length stones by 2024.
                    transformedStones.Add(stone * 2024);
                }
            }

            // Update the list of stones for the next transformation.
            stones = transformedStones;
        }

        // Return the total number of stones after all transformations.
        return stones.Count;
    }

    /// <summary>
    /// Calculates the total number of stones, accounting for counts of identical stones.
    /// </summary>
    /// <param name="filename">The input file containing the initial stones.</param>
    /// <param name="transformations">The number of transformations to perform.</param>
    /// <returns>The total number of stones after transformations.</returns>
    private static long CalculateTotalStones(List<long> stones, int numberOfBlinks)
    {
        // Create a dictionary to store stone values and their counts.
        Dictionary<long, long> stoneCounts = new Dictionary<long, long>();

        foreach (long stone in stones)
        {
            if (!stoneCounts.ContainsKey(stone))
            {
                stoneCounts[stone] = 0;
            }
            stoneCounts[stone]++;
        }

        // Perform the transformations.
        for (int i = 0; i < numberOfBlinks; i++)
        {
            Dictionary<long, long> transformedStoneCounts = new Dictionary<long, long>();

            foreach (var kvp in stoneCounts)
            {
                long stone = kvp.Key;
                long count = kvp.Value;

                if (stone == 0)
                {
                    // Replace 0 with 1, carrying over the count.
                    if (!transformedStoneCounts.ContainsKey(1))
                    {
                        transformedStoneCounts[1] = 0;
                    }
                    transformedStoneCounts[1] += count;
                }
                else if (stone.ToString().Length % 2 == 0)
                {
                    // Split even-length stones into two halves.
                    string stoneStr = stone.ToString();
                    string leftPart = stoneStr.Substring(0, stoneStr.Length / 2);
                    string rightPart = stoneStr.Substring(stoneStr.Length / 2);

                    int leftStone = int.Parse(leftPart);
                    int rightStone = int.Parse(rightPart);

                    if (!transformedStoneCounts.ContainsKey(leftStone))
                    {
                        transformedStoneCounts[leftStone] = 0;
                    }
                    transformedStoneCounts[leftStone] += count;

                    if (!transformedStoneCounts.ContainsKey(rightStone))
                    {
                        transformedStoneCounts[rightStone] = 0;
                    }
                    transformedStoneCounts[rightStone] += count;
                }
                else
                {
                    // Multiply odd-length stones by 2024.
                    long multipliedStone = stone * 2024;
                    if (!transformedStoneCounts.ContainsKey(multipliedStone))
                    {
                        transformedStoneCounts[multipliedStone] = 0;
                    }
                    transformedStoneCounts[multipliedStone] += count;
                }
            }

            // Update the dictionary of stone counts for the next transformation.
            stoneCounts = transformedStoneCounts;
        }

        // Return the total number of stones after all transformations.
        return stoneCounts.Values.Sum();
    }
    #endregion
}