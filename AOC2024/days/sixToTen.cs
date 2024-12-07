using System.Diagnostics.Metrics;
using System.IO.MemoryMappedFiles;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;

public class SixToTen
{
    #region Day Six
    /// <summary>
    /// Solves the Day Six portion of the AOC 2024 challenge.
    /// https://adventofcode.com/2024/day/6
    /// </summary>
    public void DaySix()
    {
        // The input files
        string fileName = "assets/AOC2024.6.Input.txt";
        // string fileName = "assets/AOC2024.6.Test-Input.txt";

        // List of lists to map out the room and the guard positions
        // The List<List> portion contains the Y traversing (up/down)
        // The List<char> postion contains the X traversing (left/right)
        List<List<char>> map = new List<List<char>>();

        // Used to track the position of the guard on the map
        int guardRow = -1;
        int guardColumn = -1;
        // Our starting direction
        char guardFacing = '_';

        // Valid guard facings
        List<char> validFacings = new List<char> {'<', '^', '>', 'v'};

        // The count of positions on the map the guard will occupy before leaving
        int countOfPositions = 0;

        // Read the data in from the text file
        using (StreamReader streamReader = new StreamReader(fileName))
        {
            // Placeholder for the current line of the tile
            string currentLine;
            // currentLine will be null when the StreamReader reaches the end of file
            while((currentLine = streamReader.ReadLine()) != null)
            {
                // Extract the current line into a list of chars
                map.Add(currentLine.ToCharArray().ToList());

                // If we haven't yet found the guard, let's check if it's been found
                if (guardColumn == -1)
                {
                    // Iterate over the valid facings
                    foreach (char facing in validFacings)
                    {
                        // Check if the currentLine has the guard on it
                        if (currentLine.Contains(facing))
                        {
                            // It does, so grab the index in the List<char> (column)
                            guardColumn = currentLine.IndexOf(facing);
                            // Grab the initial facing of the guard
                            guardFacing = facing;
                            break;
                        }
                    }

                    // Iterate the row each time if we haven't yet found the guard, and 1 last time after finding
                    guardRow++;
                }
            }
        }

        // Count the number of loops found
        int countOfPossibleLoops = 0;

        // Brute force the way through by changing one position at a time
        for (int i =0; i < map.Count; i++)
        {
            for (int j = 0; j < map[i].Count; j++)
            {
                // Skip this one if it's already a #
                if (map[i][j] == '#') { continue; }

                // Store the original char from the map
                char original = map[i][j];
                // Update the position to be an obstacle
                map[i][j] = '#';

                // Find out how many distinct positions the guard has, if this is -1 it's a loop
                countOfPositions = CheckGuardRoute(ref map, ref validFacings, guardRow, guardColumn, guardFacing);

                // If we have a loop, increment our loop counter
                if (countOfPositions == -1)
                {
                    countOfPossibleLoops++;
                }

                // Revert the map to the original
                map[i][j] = original;
            }
        }

        // Console.WriteLine("Count of Locations Travelled by Guard: {0}", countOfPositions);
        Console.WriteLine("Count of Possible Loops: {0}", countOfPossibleLoops);
        Console.WriteLine("--- Final Map ---");

        foreach (List<char> row in map)
        {
            Console.WriteLine(string.Join("", row.ToArray()));
        }
    }

    private int CheckGuardRoute(ref List<List<char>> map, ref List<char> validFacings, int originalRow, int originalColumn, char originalFacing)
    {
        // Have a timeout clause
        int timeout = 10000;
        int timer = 0;

        // Count the distinct positions of the guard
        int countOfPositions = 0;

        // Allows us to quit the do loop updating the map
        bool guardIsOnMap = true;

        int guardRow = originalRow;
        int guardColumn = originalColumn;
        char guardFacing = originalFacing;

        // Continually update the facings & location of the guard and check for whether on map
        do
        {
            // Temporary stores that we can modify without damaging the originals
            int testRow = guardRow;
            int testColumn = guardColumn;

            // Modify the row or column value based on the guards current facing
            switch (guardFacing)
            {
                case '<': testColumn--; break;
                case '^': testRow--; break;
                case '>': testColumn++; break;
                case 'v': testRow++; break;
                default: break;
            }

            // If both indices are within the bounds of the map, let's proceed
            if ((testRow >= 0 && testRow < map.Count) && (testColumn >= 0 && testColumn < map[0].Count))
            {
                // If the next position for the guard would be obstructed let's rotate 90deg to the right
                if (map[testRow][testColumn] == '#')
                {
                    // Get the index of the current facing and increment it, then modulo
                    // by the count to ensure it wraps around to 0 when it's too high
                    int newFacingIndex = (validFacings.IndexOf(guardFacing) + 1) % validFacings.Count;

                    // Update the guard's facing
                    guardFacing = validFacings[newFacingIndex];
                }
                // The next position isn't obstructed, so let's update our map & counts 
                else
                {
                    // If the guard hasn't yet been in this position
                    if (map[guardRow][guardColumn] != 'X')
                    {
                        // Mark it with an X
                        map[guardRow][guardColumn] = 'X';
                        // Increment our distinct positions count
                        countOfPositions++;
                    }
                    
                    // Update the current position of the guard to the new position
                    guardRow = testRow;
                    guardColumn = testColumn;
                }
            } 
            // The guard has left the map
            else
            {
                // Flip our flag so we end the do loop
                guardIsOnMap = false;
                // Mark the last position before leaing with an X
                map[guardRow][guardColumn] = 'X';
                // Increment the number of positions one last time (the last position)
                countOfPositions++;
            }

            timer++;

        } while (guardIsOnMap && timer < timeout);

        // If we timed out, return -1, otherwise return the count of positions
        return timer >= timeout ? -1 : countOfPositions;
    }
    #endregion Day Six

    #region Day Seven
    /// <summary>
    /// Solves the Day Seven portion of the AOC 2024 challenge.
    /// https://adventofcode.com/2024/day/7
    /// </summary>
    public void DaySeven()
    {
        // The input files
        // string fileName = "assets/AOC2024.7.Test-Input.txt";
        string fileName = "assets/AOC2024.7.Input.txt";

        // Count the number of tests performed
        Dictionary<Int64, List<Int64>> resultAndOperands = new Dictionary<Int64, List<Int64>>();

        // List of valid results that can be summed
        Dictionary<Int64, List<List<char>>> validOperationsPerResult = new Dictionary<Int64, List<List<char>>>();

        // The sum of all valid results
        Int64 totalCalibrationResult = 0;

        // Valid guard facings
        List<char> validOperators = new List<char> {'+', '*'};

        // Read the data in from the text file
        using (StreamReader streamReader = new StreamReader(fileName))
        {
            // Placeholder for the current line of the tile
            string currentLine;
            // currentLine will be null when the StreamReader reaches the end of file
            while((currentLine = streamReader.ReadLine()) != null)
            {
                // Split the currentLine into result & operands
                string[] splitLine = currentLine.Split(':');
                // Split the operands into individual numbers and trim the white space
                string[] operandsString = splitLine[1].Trim().Split(' ');

                // Convert the result to Int64 (very large number)
                Int64 result = Int64.Parse(splitLine[0]);
                // Create a list to store the operands
                List<Int64> operands = new List<Int64>();
                // Iterate over each operand and convert to Int64
                foreach (string operand in operandsString)
                {
                    // Store the operand the list of operands
                    operands.Add(Int32.Parse(operand));
                }

                // Store this one for later processing (if required)
                resultAndOperands.Add(result, operands);

                // A list of the list of valid operator combinations for the equation
                List<List<char>> validOperations = new List<List<char>>();

                // The number of operators required for n operands is n-1 (2+2 = 2 operands w/ 1 operand)
                int operatorsRequired = operands.Count - 1;

                // Calculate the total number of combinations
                int totalCombinations = (int)Math.Pow(validOperators.Count, operatorsRequired);

                // Go through all operations possible
                for (int i = 0; i < totalCombinations; i++)
                {
                    // Convert i into binary, then replace the 0's and 1's with the operators
                    string operatorCombination = Convert.ToString(i, 2).PadLeft(operatorsRequired, '0').Replace('0', validOperators[0]).Replace('1', validOperators[1]);

                    // The initial current result would start with the first operand
                    Int64 currentResult = operands[0];

                    // Iterate over each operand
                    for (int j = 0; j < operatorsRequired; j++)
                    {
                        // If the next operator is +, add the result, otherwise multiply
                        if (operatorCombination[j] == '+')
                        {
                            currentResult += operands[j + 1];
                        } else if (operatorCombination[j] == '*')
                        {
                            currentResult *= operands[j + 1];
                        }
                    }

                    // If the resulting equation matches the result, add it as a valid operation
                    if (currentResult == result)
                    {
                        validOperations.Add(operatorCombination.ToList());
                    }
                }

                // If the number of valid operations is >0 add this to the valid operations dictionary
                if (validOperations.Count > 0)
                {
                    validOperationsPerResult.Add(result, validOperations);
                    // Also add the result to the running total
                    totalCalibrationResult += result;
                }
            }
        }

        Console.WriteLine("Count of Results Tested: {0}", resultAndOperands.Count);
        Console.WriteLine("Count of Valid Results: {0}", validOperationsPerResult.Count);
        Console.WriteLine("Total Calibration Result: {0}", totalCalibrationResult);
    }
    #endregion Day Seven
}