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


// Debug outputs
Console.WriteLine("List One: " + lists[0].Count);
Console.WriteLine("List Two: " + lists[1].Count);
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