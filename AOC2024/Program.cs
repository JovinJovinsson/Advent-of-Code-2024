using System.ComponentModel;
using Microsoft.VisualBasic.FileIO;

// The input fields
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

// Iterate over all items in list 1
for (int i = 0; i < lists[0].Count; i ++)
{
    // Calculate the distance
    int currentDistance = (int)MathF.Abs(lists[0][i] - lists[1][i]);
    // Add it to our list of distances
    distances.Add(currentDistance);
    // Also add it to the running total distance
    totalDistance += currentDistance;
}


// Debug outputs
Console.WriteLine("List One: " + lists[0].Count);
Console.WriteLine("List Two: " + lists[1].Count);
Console.WriteLine("Total Distance: " + totalDistance);
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