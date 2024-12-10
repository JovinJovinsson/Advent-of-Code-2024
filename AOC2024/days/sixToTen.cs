using System.Diagnostics.Metrics;
using System.Globalization;
using System.IO.MemoryMappedFiles;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Xml.Schema;

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
        List<char> validOperators = new List<char> {'+', '*', '|'};

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
                    // Convert i into the appropriate base and replace with operators
                    string operatorCombination = ConvertToBase(i, validOperators.Count, operatorsRequired, validOperators);

                    // The initial current result would start with the first operand
                    Int64 currentResult = operands[0];

                    // Iterate over each operand
                    for (int j = 0; j < operatorsRequired; j++)
                    {
                        // Find out the next operator
                        // + will add the next operand to the current result
                        // * will multiply the current result by the next operand
                        // | will concatenate the current result by the next operand (e.g. 15|3 = 153)
                        switch(operatorCombination[j])
                        {
                            case '+': currentResult += operands[j + 1]; break;
                            case '*': currentResult *= operands[j + 1]; break;
                            case '|': currentResult = Int64.Parse("" + currentResult + "" + operands[j + 1]); break;
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

    /// <summary>
    /// Converts a given integer into a string representation using a specified base 
    /// and maps each digit to a corresponding character from a provided operator set.
    /// </summary>
    /// <param name="number">The number to convert.</param>
    /// <param name="baseValue">The base to use for conversion (e.g., 2 for binary, 4 for quaternary).</param>
    /// <param name="length">The desired length of the resulting string. Pads with leading characters if necessary.</param>
    /// <param name="operators">
    /// An array of characters representing the symbols to use for each digit (e.g., { '+', '*', '|' }).
    /// </param>
    /// <returns>
    /// A string representation of the number in the specified base, where each digit is replaced 
    /// by the corresponding character from the operators array.
    /// </returns>
    private string ConvertToBase(int number, int baseValue, int length, List<char> operators)
    {
        // The combination of operators
        char[] result = new char[length];

        // For each operator required
        for (int i = length - 1; i >= 0; i--)
        {
            // Modulo by the base and find the appropriate operator
            result[i] = operators[number % baseValue];
            // Modify the remaining number to identify the next value to modulo
            number /= baseValue;
        }

        return new string(result);
    }
    #endregion Day Seven

    #region Day Eight
    /// <summary>
    /// Solves the Day Eight portion of the AOC 2024 challenge.
    /// https://adventofcode.com/2024/day/8
    /// </summary>
    public void DayEight()
    {
        // The input files
        // string fileName = "assets/AOC2024.8.Test-Input.txt";
        string fileName = "assets/AOC2024.8.Input.txt";

        // List of List of chars representing the map of the town
        List<List<char>> emitterMap = new List<List<char>>();

        // A dictionary of the emitter types where the emitter type is the key
        // A list of list of ints contains the locations of that emitter in the map
        Dictionary<char, List<List<int>>> emitterTypes = new Dictionary<char, List<List<int>>>();

        // A map of only the antinodes, # marks an antinode on the example, but I will use
        // a number to indicate this, in case there are multiple antinodes. It will start
        // at 0 and increment to 9 before using the alphabet.
        List<List<char>> antinodeMap = new List<List<char>>();
        // A count of the number of antinotes
        int antinodeCount = 0;
        int uniqueAntinodeCount = 0;

        // Read the data in from the text file
        using (StreamReader streamReader = new StreamReader(fileName))
        {
            // Placeholder for the current line of the tile
            string currentLine;

            // The current row we're processing
            int row = 0;

            // currentLine will be null when the StreamReader reaches the end of file
            while((currentLine = streamReader.ReadLine()) != null)
            {
                // Convert the string to a list of chars
                emitterMap.Add(currentLine.ToList());
                // Create an antinode map of the same dimensions using only .
                antinodeMap.Add("".PadLeft(currentLine.Length, '.').ToCharArray().ToList());

                // iterate over each character in the current line
                for (int i = 0; i < currentLine.Length; i++)
                {
                    // If it's a '.' move on
                    if (currentLine[i] == '.') { continue; }

                    // Identify the row & column of this antenna
                    List<int> location = new List<int> { row, i };

                    // If the antenna type has already been found
                    if (emitterTypes.Keys.Contains(currentLine[i]))
                    {
                        // Add the new location to the dictionary for that antenna
                        emitterTypes[currentLine[i]].Add(location);
                    } else
                    {
                        // Otherwise, let's add a new antenna type & it's 1st location
                        emitterTypes.Add(currentLine[i], new List<List<int>> { location });
                    }
                }

                // Increment the row
                row++;
            }
        }

        // Iterate over each emitter type (antenna type)
        foreach (KeyValuePair<char, List<List<int>>> emitter in emitterTypes)
        {
            if (emitter.Value.Count == 1) { continue; }

            // For each location, we'll check against every other location
            for (int i = 0; i < emitter.Value.Count; i++)
            {
                // The other location
                for (int j = 0; j < emitter.Value.Count; j++)
                {
                    // Skip this emitter as it's the original one
                    if (i == j) { continue; }

                    // Grab the location of the origin checking emitter
                    int row = emitter.Value[i][0];
                    int col = emitter.Value[i][1];

                    // This is now an antinode as well, update our map & unique count accordingly
                    uniqueAntinodeCount += UpdateAntinodeMap(ref antinodeMap, row, col) ? 1 : 0;
                    // Also update our count of antinodes in total
                    antinodeCount++;
                    
                    // Calculate the row & column offset between emitter-i and emitter-j
                    int rowOffset = emitter.Value[i][0] - emitter.Value[j][0];
                    int colOffset = emitter.Value[i][1] - emitter.Value[j][1];

                    // Grab the maximum boundaries for the map
                    int rowMax = emitterMap.Count;
                    int colMax = emitterMap[i].Count;

                    // Calling IsAntinodeWithinBounds will calculate the location of the next antinode
                    // If the location is on the map, then let's proceed
                    // This will loop until we reach an antinode that is not on the map
                    while (IsAntinodeWithinBounds(row, col, rowOffset, colOffset, rowMax, colMax, out int antinodeRow, out int antinodeCol))
                    {
                        // Update our antinode map accordingly, and if required our unique antinode count
                        uniqueAntinodeCount += UpdateAntinodeMap(ref antinodeMap, antinodeRow, antinodeCol) ? 1 : 0;
                        // Also update our total count
                        antinodeCount++;

                        // Lastly, set our current row & col to the antinodes position so we can calculate the next antinode
                        row = antinodeRow;
                        col = antinodeCol;
                    }
                }
            }
        }

        Console.WriteLine("     -------------------");
        Console.WriteLine("     --- Emitter Map ---");
        Console.WriteLine("     -------------------");
        for (int i = 0; i < emitterMap.Count; i++)
        {
            Console.WriteLine("{0}: " + string.Join("", emitterMap[i]), i.ToString().PadLeft(4, '0'));   
        }
        Console.WriteLine("\n     ----------------------------");
        Console.WriteLine("     --- Emitters & Locations ---");
        Console.WriteLine("     ----------------------------");
        foreach (KeyValuePair<char, List<List<int>>> emitterDetails in emitterTypes)
        {
            Console.WriteLine("|| Emitter {0} ||", emitterDetails.Key);

            for (int i = 0; i < emitterDetails.Value.Count; i++)
            {
                Console.WriteLine("{0}: [ {1}, {2} ]", i.ToString().PadLeft(4, '0'), emitterDetails.Value[i][0], emitterDetails.Value[i][1]);
            }
        }
        Console.WriteLine("\n     --------------------");
        Console.WriteLine("     --- Antinode Map --- (Total: {0} | Unique: {1})", antinodeCount, uniqueAntinodeCount);
        Console.WriteLine("     --------------------");
        for (int i = 0; i < emitterMap.Count; i++)
        {
            Console.WriteLine("{0}: " + string.Join("", antinodeMap[i]), i.ToString().PadLeft(4, '0'));   
        }
    }

    private bool IsAntinodeWithinBounds(int row, int col, int rowOffset, int colOffset, int rowMax, int colMax, out int antinodeRow, out int antinodeCol)
    {
        // Calculate the location of the antinode by adding the offset to the row & column
        // We add so that negatives are correctly subtracted
        // These are also "out" variables allowing us to use them in the while loop
        antinodeRow = row + rowOffset;
        antinodeCol = col + colOffset;

        // If it's out of bounds, return false
        if (antinodeRow < 0 || antinodeRow >= rowMax || antinodeCol < 0 || antinodeCol >= colMax)
        {
            return false;
        }

        return true;
    }

    private bool UpdateAntinodeMap(ref List<List<char>> antinodeMap, int row, int col)
    {
        // Flag to determine whether this antinode was unique
        bool isUnique = false;

        // Grab the marker for the antinode
        char antinode = antinodeMap[row][col];

        // If it's a '.' we don't have an antinode here yet
        if (antinode == '.')
        {
            // Set the first antinode here
            antinodeMap[row][col] = '0';
            isUnique = true;
        } else
        {
            // We already have an antinode here, let's increase the marker to show multiples
            // Increment the char ID
            antinodeMap[row][col]++;
        }

        return isUnique;
    }
    #endregion Day Eight

    #region Day Nine
    /// <summary>
    /// Solves the Day 9 portion of the AOC 2024 challenge.
    /// https://adventofcode.com/2024/day/9
    /// </summary>
    public void DayNine()
    {
        // The input files
        // string fileName = "assets/AOC2024.9.Test-Input copy.txt";
        string fileName = "assets/AOC2024.9.Input.txt";

        // This will be the holistic diskmap with both data & free space blocks
        List<string> diskMap = new List<string>();

        // Read the data in from the text file
        using (StreamReader streamReader = new StreamReader(fileName))
        {
            // Placeholder for the current line of the tile
            string currentLine;

            // This helps us keep track of what block we're processing
            // True = Data Block
            // False = Free Space Block
            bool isDataBlock = true;

            // The below lists will be used to store the alternating data blocks & free space
            // e.g. 12345 would result in existingDataBlocks having the values "0", "111", "22222"
            // meanwhile freeSpaceBlocks would have ".." and "...."
            // Each List<string> contains the IDs (e.g. 1, 1, 1 or ., ., .,)
            List<List<string>> dataBlocks = new List<List<string>>();
            List<List<string>> freeSpaceBlocks = new List<List<string>>();

            // This helps us insert the IDs of the data blocks
            int blockID = 0;

            // currentLine will be null when the StreamReader reaches the end of file
            while((currentLine = streamReader.ReadLine()) != null)
            {
                foreach (char blockChar in currentLine)
                {
                    // Holder for the size of the 
                    int blockSize = 0;

                    // Parse the char into an string then Int
                    if (!Int32.TryParse(blockChar.ToString(), out blockSize))
                    { 
                        Console.WriteLine("Error parsing {0}", blockChar);
                        break;
                    }

                    if (isDataBlock)
                    {
                        dataBlocks.Add(CreateBlock(blockSize, blockID.ToString()));
                        // Increment the ID by 1
                        blockID++;
                    } else
                    {
                        freeSpaceBlocks.Add(CreateBlock(blockSize));
                    }

                    // Alternate the flag between data & free space
                    isDataBlock = !isDataBlock;
                }
            }

            // Combine the dataBlock and freeSpaceBlocks lists into a singular diskMap
            diskMap = BuildDiskMap(ref dataBlocks, ref freeSpaceBlocks);
            // Show the initial state
            OutputDiskMap(ref diskMap, "Fragmented Data");

            // Starting with the last data block, iterate backwards to try and move it
            for (int i = dataBlocks.Count - 1; i >= 0; i--)
            {
                // Start looking for space in the earliest freespace block
                for (int j = 0; j < freeSpaceBlocks.Count; j++)
                {
                    // If the freespace block is higher than the data block we'd be moving it later not earlier
                    if (i <= j) { break; }

                    // If there is no free space, ignore it
                    if (freeSpaceBlocks[j].Count < 1) { continue; }

                    // Check if this is a fresh free space block denoted by a . in the 1st position
                    bool isFreshBlock = freeSpaceBlocks[j][0] == "." ? true : false;

                    // If it is a fresh block, but there isn't enough space, let's check the next one
                    if (isFreshBlock && dataBlocks[i].Count > freeSpaceBlocks[j].Count) { continue; }

                    // Some holders for when it's not a fresh block
                    int countActualFree = 0;
                    int indexOffset = -1;
                    // If it's not a fresh block, let's see if there's some space to squeeze into
                    if (!isFreshBlock)
                    {
                        // Iterate over each bit and check if it's free
                        for (int k = 0; k < freeSpaceBlocks[j].Count; k++)
                        {
                            // If it's got a value, skip it, it's used up
                            if (freeSpaceBlocks[j][k] != ".") { continue; }

                            // We got here, so it's a free bit
                            countActualFree++;
                            // If this is the first spot of the free bits in the space, update our index offset
                            if (indexOffset == -1) { indexOffset = k; }
                        }

                        // Turns out it wasn't big eough afterall, onto the next one
                        if (dataBlocks[i].Count > countActualFree) { continue; }
                    }

                    // We can move out block into this space!
                    // If our index offset is still -1, set it to 0
                    if (indexOffset == -1) { indexOffset = 0; }

                    // Now, let's iterate over each it and move it across
                    for (int k = 0; k < dataBlocks[i].Count; k++)
                    {
                        freeSpaceBlocks[j][indexOffset + k] = new string(dataBlocks[i][k]);
                        dataBlocks[i][k] = ".";
                    }
                }
            }
            // Rebuild the disk map
            diskMap = BuildDiskMap(ref dataBlocks, ref freeSpaceBlocks);

            // Show the final state
            OutputDiskMap(ref diskMap, "Defragmented Data");

            // Calculate the check sum
            Int128 checkSum = CalculateCheckSum(ref diskMap);
            Console.WriteLine("Checksum: {0}", checkSum);
        }
    }

    /// <summary>
    /// Simple function for outputting the diskMap
    /// </summary>
    /// <param name="diskMap">The list of strings that represent the diskMap</param>
    /// <param name="title">The title of the diskMap</param>
    private void OutputDiskMap(ref List<string> diskMap, string title)
    {
        Console.WriteLine("--------------------------------");
        Console.WriteLine("--- {0} Disk Map ---", title);
        Console.WriteLine("--------------------------------");
        Console.WriteLine(string.Join("", diskMap.ToArray()));
    }

    /// <summary>
    /// A simple function for calculating the CheckSum based on the challenge.
    /// </summary>
    /// <param name="diskMap">The list of strings that represent the diskMap</param>
    /// <returns>The checkSum</returns>
    private Int128 CalculateCheckSum(ref List<string> diskMap)
    {
        Int128 checkSum = 0;

        // Iterate over every bit in the diskMap
        for (int i = 0; i < diskMap.Count; i++)
        {
            string currentBit = diskMap[i];

            // If it's free space let's stop calculating as it's only free after that
            if (currentBit == ".") { continue; }

            // Parse the blockID into an Int and multiply by the index before adding
            checkSum += Int32.Parse(currentBit) * i;
        }

        return checkSum;
    }

    /// <summary>
    /// Simple function that creates a list of bits for a block based on parameters
    /// </summary>
    /// <param name="size">The block size (count of bits)</param>
    /// <param name="blockID">The block IDs (or . for free space)</param>
    /// <returns></returns>
    private List<string> CreateBlock(int size, string blockID = ".")
    {
        // Holding list for the block
        List<string> block = new List<string>();

        // Add as many blockIDs as required to the block
        for (int i = 0; i < size; i++)
        {
            block.Add(blockID);
        }

        return block;
    }

    /// <summary>
    /// Combines the two lists of dataBlocks and freeSpace blocks together in order (alternating)
    /// </summary>
    /// <param name="dataBlocks">The list of data blocks</param>
    /// <param name="freeSpaceBlocks">The list of free space blocks</param>
    /// <returns>The combined diskMap list</returns>
    private List<string> BuildDiskMap(ref List<List<string>> dataBlocks, ref List<List<string>> freeSpaceBlocks)
    {
        // Holding list for the diskMap
        List<string> diskMap = new List<string>();
        // This will allow us to ensure we don't exceed the overall count of blocks in the list
        int total = 0;
        // i will be used for each list of blocks so they're added in order
        for (int i = 0; total < dataBlocks.Count + freeSpaceBlocks.Count; i++)
        {
            // Secondary check to make sure we aren't adding a block that doesn't exist
            if (i < dataBlocks.Count)
            {
                // Add each individual identifier to the diskMap
                for (int j = 0; j < dataBlocks[i].Count; j++)
                {
                    diskMap.Add(dataBlocks[i][j]);
                }
                // Increment the total
                total++;
            }

            // Same as above
            if (i < freeSpaceBlocks.Count)
            {
                for (int j = 0; j < freeSpaceBlocks[i].Count; j++)
                {
                    diskMap.Add(freeSpaceBlocks[i][j]);
                }
                total++;
            }
        }
        return diskMap;
    }
    #endregion

    #region Day Ten
    /// <summary>
    /// Solves the Day 10 portion of the AOC 2024 challenge.
    /// https://adventofcode.com/2024/day/10
    /// </summary>
    public void DayTen()
    {
        // The input files
        // string fileName = "assets/AOC2024.10.Test-Input.txt";
        string fileName = "assets/AOC2024.10.Input.txt";

        List<List<int>> topographicMap = new List<List<int>>();

        // Read the data in from the text file
        using (StreamReader streamReader = new StreamReader(fileName))
        {
            // Placeholder for the current line of the tile
            string currentLine;

            // currentLine will be null when the StreamReader reaches the end of file
            while((currentLine = streamReader.ReadLine()) != null)
            {
                List<int> row = new List<int>();

                foreach (char height in currentLine)
                {
                    int heightInt = Int32.Parse(height.ToString());

                    row.Add(heightInt);    
                }

                topographicMap.Add(row);
            }

            OutputTopographicMap(ref topographicMap, "Map");

            List<List<int>> trailheads = FindTrailheads(ref topographicMap);

            int sumOfTrailheadScores = 0;

            foreach (List<int> trailhead in trailheads)
            {
                List<List<int>> discoveredPeaks = new List<List<int>>();
                FindTrails(ref topographicMap, trailhead, 1, new List<List<int>>(), ref discoveredPeaks);

                sumOfTrailheadScores += discoveredPeaks.Count;
            }

            Console.WriteLine("Trails Found: {0}", sumOfTrailheadScores);
        }
    }

    // <summary>
    /// Simple function for outputting the diskMap
    /// </summary>
    /// <param name="diskMap">The list of strings that represent the diskMap</param>
    /// <param name="title">The title of the diskMap</param>
    private void OutputTopographicMap(ref List<List<int>> topographicMap, string title)
    {
        Console.WriteLine("--------------------------------");
        Console.WriteLine("--- {0} ---", title);
        Console.WriteLine("--------------------------------");
        foreach (List<int> row in topographicMap)
        {
            Console.WriteLine(string.Join("", row.ToArray()));
        }
    }

    private List<List<int>> FindTrailheads(ref List<List<int>> topographicMap)
    {
        List<List<int>> trailheads = new List<List<int>>();

        for (int i = 0; i < topographicMap.Count; i++)
        {
            for (int j = 0; j < topographicMap[i].Count; j++)
            {
                if (topographicMap[i][j] != 0) { continue; }

                trailheads.Add(new List<int>() { i, j });
            }
        }

        return trailheads;
    }

    private void FindTrails(ref List<List<int>> topographicMap, List<int> position, int targetHeight, List<List<int>> currentTrail, ref List<List<int>> discoveredPeaks)
    {
        List<List<int>> possibleNextSteps = new List<List<int>> { new List<int> { -1, 0 }, new List<int> { 1, 0 }, new List<int> { 0, -1 }, new List<int> { 0, 1 } };

        int currentScore = 0;

        for (int i = 0; i < possibleNextSteps.Count; i++)
        {
            List<int> next = Add(position, possibleNextSteps[i]);

            if (PositionOutOfBounds(topographicMap.Count, topographicMap[position[0]].Count, next)) { continue; }

            int nextHeight = topographicMap[next[0]][next[1]];

            if (nextHeight != targetHeight) { continue; }

            if (nextHeight == 9)
            {
                // OutputTopographicMap(ref currentTrail, "Trail Found");

                // bool alreadyFound = false;

                // foreach (List<int> peak  in discoveredPeaks)
                // {
                //     int peakX = peak[0];
                //     int peakY = peak[1];
                //     int nextX = next[0];
                //     int nextY = next[1];

                //     if (peak[0] == next[0] && peak[1] == next[1])
                //     {
                //         alreadyFound = true;
                //         break;
                //     }
                // }

                // if (alreadyFound) { continue; }

                discoveredPeaks.Add(next);
                currentScore++;
            } else
            {
                FindTrails(ref topographicMap, next, targetHeight + 1, currentTrail, ref discoveredPeaks);
            }
        }
    }

    private List<int> Add(List<int> left, List<int> right)
    {
        return new List<int> { left[0] + right[0], left[1] + right[1] };
    }

    private bool PositionOutOfBounds(int rows, int cols, List<int> position)
    {
        return (position[0] < 0 || position[0] >= rows || position[1] < 0 || position[1] >= cols) ? true : false;
    }
    #endregion
}