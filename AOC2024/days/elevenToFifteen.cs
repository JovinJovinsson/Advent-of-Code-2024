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

        List<string> stoneLabels = new List<string>();

        // Read the data in from the text file
        using (StreamReader streamReader = new StreamReader(fileName))
        {
            // Placeholder for the current line of the tile
            string currentLine;

            // currentLine will be null when the StreamReader reaches the end of file
            while((currentLine = streamReader.ReadLine()) != null)
            {
                // Split the string with spaces and add it to our list
                stoneLabels = currentLine.Split(" ").ToList();
            }
        }

        OutputStones(ref stoneLabels, "Initial arrangement:");

        // Number of times we need to iterate, the test sample is much smaller
        // int numberOfBlinks = usingTestInput ? 6 : 25;
        int numberOfBlinks = 75;

        // Let's repeat our blinks to the required number
        // for (int i = 0; i < numberOfBlinks; i++)
        // {
            // Update out list by blinking
            // Blink(ref stoneLabels);

            long count = 0;

            foreach (string stone in stoneLabels)
            {
                Blink(stone, numberOfBlinks, 0, ref count);
            }

            // OutputStones(ref stoneLabels, "After " + (i + 1) + " blinks:");

            Console.WriteLine("=============================");
            Console.WriteLine("=== Number of Stones: {0} ===", count);
            Console.WriteLine("=============================");
        // }
    }

    /// <summary>
    /// Simple function to output the lsit of stones
    /// </summary>
    /// <param name="stoneLabels"></param>
    /// <param name="title"></param>
    private void OutputStones(ref List<string> stoneLabels, string title)
    {
        Console.WriteLine("\n" + title);
        Console.WriteLine(string.Join(" ", stoneLabels.ToArray()));
    }

    /// <summary>
    /// Processes the blinks based on the rules set out.
    /// </summary>
    /// <param name="stoneLabels">The list of stones</param>
    private void Blink(ref List<string> stoneLabels)
    {
        // A new list to keep the order correct, we'll replace the ref list with this
        List<string> afterBlink = new List<string>();

        // Iterate over each stone
        for (int i = 0; i < stoneLabels.Count; i++)
        {
            // Grab the current stone for ease
            string stone = stoneLabels[i];

            // Get the string length
            int length = stone.Length;

            
            if (stone == "0")
            {
                // If it's a 0, add a 1 in it's place in the new list
                afterBlink.Add("1");
            } else if (stone.Length % 2 == 0) // It's even in number of digits, let's split it
            {
                // Get the midPoint of the stone, splits use length rather than indexes
                int midPoint = stone.Length / 2;

                // First stone, should be the first half of the number (0 - midPoint)
                string firstString = stone.Substring(0, midPoint);
                // Convert to a long which allows dropping of any leading 0's
                long firstLong = long.Parse(firstString);
                // Parse it to a string and add it to our new list
                afterBlink.Add(firstLong.ToString());

                // Repeat the above, but use the midPoint to the end to get the substring
                string secondString = stone.Substring(midPoint, (length - midPoint));
                long secondLong = long.Parse(secondString);
                afterBlink.Add(secondLong.ToString());
            } else 
            {
                // It didn't match the other rules, so lets multiply by 2024
                long newStone = long.Parse(stone) * 2024;

                afterBlink.Add(newStone.ToString());
            }
        }

        // Replace the PBR list with our new list
        stoneLabels = afterBlink;
    }

    private void Blink(string stone, int numberOfBlinks, int currentBlink, ref long countOfStones)
    {
        currentBlink++;

        if (currentBlink > numberOfBlinks) { countOfStones++; return; }

        if (stone == "0")
        {
            Blink("1", numberOfBlinks, currentBlink, ref countOfStones);            
        } else if (stone.Length % 2 == 0) // It's even in number of digits, let's split it
        {
            int length = stone.Length;
            int midPoint = length / 2;
            
            Blink(long.Parse(stone.Substring(0, midPoint)).ToString(), numberOfBlinks, currentBlink, ref countOfStones);
            Blink(long.Parse(stone.Substring(midPoint, (length - midPoint))).ToString(), numberOfBlinks, currentBlink, ref countOfStones);
        } else 
        {
            // It didn't match the other rules, so lets multiply by 2024
            long newStone = long.Parse(stone) * 2024;

            Blink(newStone.ToString(), numberOfBlinks, currentBlink, ref countOfStones);
        }
    }
    #endregion
}