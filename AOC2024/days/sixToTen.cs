using System.IO.MemoryMappedFiles;

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

        // List of lists to map out the room and the guard positions
        // The List<List> portion contains the Y traversing (up/down)
        // The List<char> postion contains the X traversing (left/right)
        List<List<char>> map = new List<List<char>>();

        // The initial starting point of the guard across both dimensions
        int guardRow = -1;
        int guardColumn = -1;
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

                if (guardColumn == -1)
                {
                    foreach (char facing in validFacings)
                    {
                        if (guardColumn == -1 && currentLine.Contains(facing))
                        {
                            guardColumn = currentLine.IndexOf(facing);
                            guardFacing = facing;
                        }
                    }

                    guardRow++;
                }
            }
        }

        bool guardIsOnMap = true;

        do
        {
            int testRow = guardRow;
            int testColumn = guardColumn;

            switch (guardFacing)
            {
                case '<': testColumn--; break;
                case '^': testRow--; break;
                case '>': testColumn++; break;
                case 'v': testRow++; break;
                default: break;
            }

            if ((testRow >= 0 && testRow < map.Count) && (testColumn >= 0 && testColumn < map[0].Count))
            {
                if (map[testRow][testColumn] == '#')
                {
                    int newFacingIndex = (validFacings.IndexOf(guardFacing) + 1) % validFacings.Count;

                    // if (newFacingIndex >= validFacings.Count) { newFacingIndex = 0; }

                    guardFacing = validFacings[newFacingIndex];
                } else
                {
                    if (map[guardRow][guardColumn] != 'X')
                    {
                        map[guardRow][guardColumn] = 'X';
                        countOfPositions++;
                    }
                    
                    guardRow = testRow;
                    guardColumn = testColumn;

                    // map[guardRow][guardColumn] = guardFacing;
                }
            } else
            {
                guardIsOnMap = false;
                countOfPositions++;
            }

        } while (guardIsOnMap);

        Console.WriteLine("Count of Locations Travelled by Guard: {0}", countOfPositions);
        Console.WriteLine("--- Final Map ---");

        foreach (List<char> row in map)
        {
            Console.WriteLine(string.Join("", row.ToArray()));
        }
    }
}