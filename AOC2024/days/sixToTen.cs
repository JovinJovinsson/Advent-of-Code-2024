using System.IO.MemoryMappedFiles;
using System.Reflection.Metadata.Ecma335;

public class SixToTen
{
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
}