using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;
using System.Diagnostics.Metrics;

public class OneToFive
{
    /// <summary>
    /// Solves the Day One portion of the AOC 2024 challenge.
    /// https://adventofcode.com/2024/day/1
    /// </summary>
    public void DayOne()
    {
        // The input files
        string[] files = {"assets/AOC2024.1.Input-1.csv", "assets/AOC2024.1.Input-2.csv" };

        // List of lists of ints to use for storage
        List<List<int>> lists = new List<List<int>>();
        lists.Add(new List<int>());
        lists.Add(new List<int>());

        // Iterate over each file and process the data
        for (int i = 0; i < files.Length; i++)
        {
            // Get the file to work with
            using (TextFieldParser parser = new TextFieldParser(files[i]))
            {
                // Parse the text field into comma delimited
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                
                // Iterate over each piece of data until we reach the end
                while (!parser.EndOfData)
                {
                    // Process the text into an array of strings
                    string[] fields = parser.ReadFields();
                    // Iterate over each string
                    foreach (string field in fields)
                    {
                        // Convert it to an int
                        int fieldInt = Int32.Parse(field);
                        // Add the int to the relevant list (i)
                        lists[i].Add(fieldInt);
                    }
                }
            }
            // Sort the list now that we have all values
            lists[i].Sort();
        }

        // Quick validity check to make sure we can troubleshoot a problem
        if (lists[0].Count != lists[1].Count) { Console.WriteLine("List length mismatch!"); }

        // List of distances so we can keep track of our data points
        List<int> distances = new List<int>();
        // The total distance between the sorted list values
        int totalDistance = 0;

        // List of counts that an item in list 1 appears in list 2
        List<int> similarityScores = new List<int>();
        // The total multiplication of this count of this item in list 1 found in list 2
        int totalSimilarity = 0;

        // Group the items by the unique values
        var groupedItems = lists[1].GroupBy(j => j).ToList();
        // Lists to make it super easy to access further down the line
        List<int> groupOrder = new List<int>();
        List<int> groupCounts = new List<int>();

        // Iterate over the grouped items
        foreach (var group in groupedItems)
        {
            // Store the unique value from the list here, we can 
            // then get the index and retrieve the count in the second list
            groupOrder.Add(group.Key);
            // Store the count in this other list
            groupCounts.Add(group.Count());
        }

        // Iterate over all items in list 1
        for (int i = 0; i < lists[0].Count; i ++)
        {
            // Calculate the distance
            int currentDistance = (int)MathF.Abs(lists[0][i] - lists[1][i]);
            // Add it to our list of distances
            distances.Add(currentDistance);
            // Also add it to the running total distance
            totalDistance += currentDistance;

            // Check that this value exists in our grouped list
            if (groupOrder.Contains(lists[0][i]))
            {
                // It does, so now lets get the value from the counts list
                int similarityForItem = groupCounts.ElementAt(groupOrder.IndexOf(lists[0][i]));
                // Add the similarity to our similarity list (just in case)
                similarityScores.Add(lists[0][i] * similarityForItem);
                // Addd it to the cumulative similarity too
                totalSimilarity += (lists[0][i] * similarityForItem);
            }
        }


        // Debug outputs
        Console.WriteLine("List One: " + lists[0].Count);
        Console.WriteLine("List Two: " + lists[1].Count);
        Console.WriteLine("Total Distance: " + totalDistance);
        Console.WriteLine("-------");
        Console.WriteLine("Groups: " + groupOrder.Count);
        Console.WriteLine("Group Counts: " + groupCounts.Count);
        Console.WriteLine("Total Similarity: " + totalSimilarity);
        Console.WriteLine("-------");
        string listOne = "List One: ";
        foreach (int item in lists[0])
        {
            listOne = listOne + item + ", ";
        }
        Console.WriteLine(listOne);
        Console.WriteLine("-------");
        string listTwo = "List Two: ";
        foreach (int item in lists[1])
        {
            listTwo = listTwo + item + ", ";
        }
        Console.WriteLine(listTwo);
        Console.WriteLine("-------");
        string distancesList = "Distances: ";
        foreach (int item in distances)
        {
            distancesList = distancesList + item + ", ";
        }
        Console.WriteLine(distancesList);
    }

    /// <summary>
    /// Solves the Day Two portion of the AOC 2024 challenge.
    /// Also uses <c>CheckReportSafety</c>
    /// https://adventofcode.com/2024/day/2
    /// </summary>
    public void DayTwo()
    {
        // The input files
        string fileName = "assets/AOC2024.2.Input.txt";

        // List of lists where each sub-List is a report containing a list of ints (levels)
        List<List<int>> reports = new List<List<int>>();

        // Read the data in from the text file
        using (StreamReader streamReader = new StreamReader(fileName))
        {
            // Placeholder for the current line of the tile
            string currentLine;
            // currentLine will be null when the StreamReader reaches the end of file
            while((currentLine = streamReader.ReadLine()) != null)
            {
                // Split the current line into separate values, delimited with space
                string[] levelsStrings = currentLine.Split(' ');
                // Create a placeholder list to store the int parsed values
                List<int> levels = new List<int>();

                // Iterate over each value to add it to the list
                foreach (string level in levelsStrings)
                {
                    // Convert it to an int
                    int fieldInt = Int32.Parse(level);
                    // Add the int to the relevant list (i)
                    levels.Add(fieldInt);
                }

                // Add the report to the list of reports
                reports.Add(levels);
            }
        }

        // Create a List to store the results of our checks
        List<bool> reportSafety = new List<bool>();
        // Create a count of the safe reports
        int countOfSafeReports = 0;

        // Iterate over each report
        foreach(List<int> report in reports)
        {
            // Bool which is re-instantiated for each report and set on checking the level safety
            bool isSafeReport = CheckLevelSafety(report);

            // Part two enabled a dampener, so if it's not safe, let's re-check by checking if we remove
            // one level would it be safe
            if (!isSafeReport)
            {
                // We iterate over the report for each level, using the index i, we remove
                // that particular level from the report and re-check, if it's safe, we move on
                for (int i = 0; i < report.Count; i++)
                {
                    // Temporary list so we aren't damaging source data
                    List<int> dampenedReport = report.ToList();
                    // Remove the level in question to enable a re-test
                    dampenedReport.RemoveAt(i);

                    // Run our CheckLevelSafety with the modified report
                    if (CheckLevelSafety(dampenedReport))
                    {
                        // If it's safe, let's skip all further processing and re-mark it as safe
                        isSafeReport = true;
                        break;
                    }
                }
            }

            // Add the result to our list
            reportSafety.Add(isSafeReport);
            // If it was safe, increment out counter
            if (isSafeReport) { countOfSafeReports++; }
        }

        Console.WriteLine("Count of Reports: " + reports.Count);
        Console.WriteLine("Count of Safe Reports: " + countOfSafeReports);
    }

    /// <summary>
    /// This checks the safey of a given report based on the criteria of:
    /// - All levels must be either increasing or decreasing
    /// - All levels must not have a difference less than 1 or more than 3
    /// </summary>
    /// <param name="report"></param>
    /// <returns></returns>
    private bool CheckLevelSafety(List<int> report)
    {
        // Find the difference of the 1st & 2nd level
        int firstDifference = report[0] - report[1];

        // If it doesn't increase or decrease, it's unsafe
        if (firstDifference == 0) { return false; }

        // Establish whether the report is increasing or decreasing for checking each level
        bool isIncreasing = firstDifference > 0 ? true : false;

        // Iterate over all the levels to check it matches the criteria
        for (int i = 1; i < report.Count; i++)
        {
            // Find the difference between the previous and current level
            int levelDifference = report[i - 1] - report[i];

            // If this level difference is 0, or is trending opposite, it's unsafe
            if (levelDifference == 0 || levelDifference > 0 && !isIncreasing || levelDifference < 0 && isIncreasing) { return false; }

            // Also identify the absolute difference
            int absoluteDifference = (int)MathF.Abs(levelDifference);
            // If the absolute difference is less than 1 or more than 3, it's unsafe
            if (absoluteDifference < 1 || absoluteDifference > 3) { return false; }
        }

        // The report made it here, so it's safe
        return true;
    }

    public void DayThree()
    {
        // The input file
        string fileName = "assets/AOC2024.3.Input.txt";

        // List of strings of the muls (e.g. mul(X,Y))
        List<string> muls = new List<string>();

        // List of List of ints for the inidividual pairs of X,Y values
        List<List<int>> mulPairs = new List<List<int>>();

        // The total of the pairs multiplied
        int multiplicationTotal = 0;

        // Read the data in from the text file
        using (StreamReader streamReader = new StreamReader(fileName))
        {
            // This regex pattern now looks for don't, do or mul(X,Y) so we can
            // flip a flag and get the correct answer
            string regexPattern = @"(don't|do|mul\(([0-9]{1,3},[0-9]{1,3})\))";
            // Create the regex object for this pattern
            Regex regex = new Regex(regexPattern);

            // Boolean flag that will go to true when "don't" is found, and false when "do"
            // this will allow us to ignore anything that shouldn't be mulled.
            // Default is "false" as any instructions before the first do or don't are
            // considered a "do"
            bool ignoreMuls = false;

            // Placeholder for the current line of the tile
            string currentLine;
            // currentLine will be null when the StreamReader reaches the end of file
            while((currentLine = streamReader.ReadLine()) != null)
            {
                // Find all the mul(X,Y) matches with only 1-3 numbers
                MatchCollection matches = regex.Matches(currentLine);

                // Iterate over every match
                for (int i = 0; i < matches.Count; i++)
                {
                    // If we found a "do" 
                    if (matches[i].Value == "do")
                    {
                        // Make sure we don't ignore instructions
                        ignoreMuls = false;
                    }
                    // We found a "dont'"
                    else if (matches[i].Value == "don't")
                    {
                        // Ignore any further instructions until the next "do"
                        ignoreMuls = true;
                    }
                    else if (!ignoreMuls)
                    {
                        // Create the number matching pattern to run on the matched mul(X,Y)
                        string numberPattern = @"[0-9]{1,3}";
                        // Create the regex object for the number pattern
                        Regex numberRegex = new Regex(numberPattern);

                        // Add the matched string to our list
                        muls.Add(matches[i].Value);

                        // Find all the numbers only in the current pair (should only be 2)
                        MatchCollection numbers = numberRegex.Matches(matches[i].Value);
                        // Create a holding list for the int pair
                        List<int> mulPair = new List<int>();

                        // Get each number, parse it to an Int32 and add it to the holding pair
                        for (int j = 0; j < numbers.Count; j++)
                        {
                            mulPair.Add(Int32.Parse(numbers[j].Value));
                        }

                        // Add this pair to the list of all pairs
                        mulPairs.Add(mulPair);

                        // Multiply the pair and add it to the running total
                        multiplicationTotal += (mulPair[0] * mulPair[1]);
                    }
                }
            }
        }

        Console.WriteLine("Multiplication Total: " + multiplicationTotal);
        Console.WriteLine("-----");

        for (int i = 0; i < muls.Count; i++)
        {
            Console.WriteLine("Mul #" + i + ": " + muls[i]);
            Console.WriteLine("Mul #" + i + ": X=" + mulPairs[i][0] + " | Y=" + mulPairs[i][1]);
        }
    }
}

