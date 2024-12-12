using System.Numerics;
using System.Runtime.Serialization.Formatters;
using System.Security;
using Microsoft.Win32.SafeHandles;

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

    #region Day Twelve
    /// <summary>
    /// Solves the Day 12 portion of the AOC 2024 challenge.
    /// https://adventofcode.com/2024/day/12
    /// </summary>
    public void DayTwelve()
    {
        // Flag to quickly switch inputs (and possibly other things)
        bool usingTestInput = true;

        // The input files
        string fileName = "/Users/jovinjovinsson/GitHub/Advent-of-Code-2024/AOC2024/assets/elevenToFifteen/AOC2024.12.";
        fileName += usingTestInput ? "Test-Input copy.txt" : "Input.txt";

        List<List<char>> gardenMap = new List<List<char>>();

        List<char> uniquePlants = new List<char>();

        // Read the data in from the text file
        using (StreamReader streamReader = new StreamReader(fileName))
        {
            // Placeholder for the current line of the tile
            string currentLine;

            // currentLine will be null when the StreamReader reaches the end of file
            while((currentLine = streamReader.ReadLine()) != null)
            {
                gardenMap.Add(currentLine.ToCharArray().ToList());

                foreach (char c in currentLine)
                {
                    if (uniquePlants.Contains(c)) { continue; }

                    uniquePlants.Add(c);
                }
                // if (rowLength == 0)
                // {
                //     // Get the length of the line
                //     rowLength = currentLine.Length; 
                // }

                // // Create an array of the length
                // char[] rowArray = new char[rowLength];

                // for (int j = 0; j < rowLength; j++)
                // {
                //     if (!gardenPlots.ContainsKey(currentLine[j]))
                //     {
                //         // Convert the array to a list, this will ensure we can store the correct amount of plants
                //         List<char> row = rowArray.ToList();

                //         List<List<char>> freshPlot = new List<List<char>>();

                //         for (int k = 0; k <= i; k++)
                //         {
                //             freshPlot.Add(new List<char>(row));
                //         }

                //         gardenPlots.Add(row[j], freshPlot);
                //     }

                //     // if (gardenPlots)
                // }

                // i++;
            }
        }

        int numberOfRows = gardenMap.Count;
        int numberOfOCols = gardenMap[0].Count;

        // List<int>[0] = Area of plant
        // List<int>[1] = perimeter of plant
        Dictionary<char, List<int>> plotCosts = new Dictionary<char, List<int>>();

        // This dictionary is tracked like so:
        // char = the plant type
        // List<List<char> the map of the plots for that plant with each nested List<char> being a row in the map
        Dictionary<char, List<List<char>>> gardenPlots = new Dictionary<char, List<List<char>>>();

        Dictionary<char, List<List<int>>> gardenFences = new Dictionary<char,List<List<int>>>();

        int gardenCost = 0;

        foreach (char plant in uniquePlants)
        {
            List<List<char>> plotsForPlant = FindPlotsForPlant(gardenMap, plant, out int area);
            List<List<int>> fencesForPlant = GetPerimeterForPlant(ref plotsForPlant, plant, out int perimeter);

            gardenPlots.Add(plant, plotsForPlant);
            gardenFences.Add(plant, fencesForPlant);

            List<int> plantCost = new List<int>();
            plantCost.Add(area);
            plantCost.Add(perimeter);

            gardenCost += area * perimeter;

            OutputGardenMapForPlant(plotsForPlant, plant);
            Console.WriteLine("Cost for Plant: {0}", (area * perimeter));
        }

        Console.WriteLine("---------------");
        Console.WriteLine("Total Cost: {0}", gardenCost);
        Console.WriteLine("---------------");
    }

    private List<List<char>> FindPlotsForPlant(List<List<char>> gardenMap, char plant, out int area)
    {
        List<List<char>> plotsFound = new List<List<char>>();

        area = 0;

        for (int i = 0; i < gardenMap.Count; i++)
        {
            List<char> row = new List<char>( gardenMap[i] );

            for (int j = 0; j < row.Count; j++)
            {
                if (row[j] == plant)
                { 
                    area++;
                    continue; 
                }

                row[j] = '.';
            }

            plotsFound.Add(row);
        }

        return plotsFound;
    }

    public List<List<int>> GetPerimeterForPlant(ref List<List<char>> plotsForPlant, char plant, out int perimeter)
    {
        List<List<int>> fencesForPlant = new List<List<int>>();

        perimeter = 0;

        List<List<int>> offsets = new List<List<int>>{ new List<int> { -1, 0 }, new List<int> { 0, 1 }, new List<int> { 1, 0 }, new List<int> { 0, -1} };

        for (int i = 0; i < plotsForPlant.Count; i++)
        {
            int[] sizer = new int[plotsForPlant[i].Count];
            List<int> fences = sizer.ToList();;

            for (int j = 0; j < plotsForPlant[i].Count; j++)
            {
                fences[j] = 0;

                if (plotsForPlant[i][j] != plant) { continue; }

                foreach (List<int> offset in offsets)
                {
                    List<int> location = Add(new List<int> { i, j }, offset);

                    bool invalidLocation = false;
                    if (location[0] < 0) { fences[j]++; invalidLocation = true; }
                    if (location[0] >= plotsForPlant.Count) { fences[j]++; invalidLocation = true; }
                    if (location[1] < 0) { fences[j]++; invalidLocation = true; }
                    if (location[1] >= plotsForPlant[i].Count) { fences[j]++; invalidLocation = true; }

                    if (invalidLocation) { continue; }

                    char nextPlant = plotsForPlant[location[0]][location[1]];

                    if (nextPlant == plant) { continue; }

                    fences[j]++;
                }

                perimeter += fences[j];
            }

            fencesForPlant.Add(fences);
        }

        return fencesForPlant;
    }

    private void OutputGardenMapForPlant(List<List<char>> plots, char plant)
    {
        Console.WriteLine("\n\n---- Plots for Plant: {0} ----", plant);
        for (int i = 0; i < plots.Count; i++)
        {
            Console.WriteLine(i.ToString().PadLeft(3, '0') + ": " + string.Join("", plots[i].ToArray()));
        }
    }

    /// <summary>
    /// Simple function to add 2 Lists of ints together
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    private List<int> Add(List<int> left, List<int> right)
    {
        return new List<int> { left[0] + right[0], left[1] + right[1] };
    }
    #endregion
}