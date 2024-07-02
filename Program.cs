using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static List<Student> studentRecords = new List<Student>(); // List to store student records
    static string dataFilePath = "studentRecords.txt"; // File path to store/load student records

    static void Main()
    {
        LoadStudentRecords();  // Load existing records from the file
        while (true)
        {
            menus(); // Display the menu
            string choice = Console.ReadLine(); // Read user input

            switch (choice)
            {
                case "1":
                    CreateANewStudentRecord(); // Option to create a new student record
                    break;

                case "2":
                    EnterMarksForAStudent(); // Option to enter marks for a student
                    break;

                case "3":
                    UpdateAStudentRecord(); // Option to update a student record
                    break;

                case "4":
                    ShowAStudentRecord(); // Option to show a student record
                    break;

                case "5":
                    SaveStudentRecords();  // Save records to the file before exiting
                    Console.WriteLine("The Record saved and Program exit.");
                    return;

                default:
                    Console.WriteLine("Invalid choice. Please enter a valid choice.");
                    break;
            }
        }
    }

    static void SaveStudentRecords()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(dataFilePath))
            {
                foreach (Student student in studentRecords)
                {
                    // Save student information and mark history to the file
                    writer.Write($"{student.StudentNumber}|{student.Name}|");

                    // Save marks, average, and result for each mark history entry
                    foreach (MarkHistoryEntry entry in student.MarkHistory)
                    {
                        writer.Write($"{string.Join(",", entry.Marks)}|{entry.Average}|{entry.Result}|");
                    }

                    writer.WriteLine(); // Move to the next line for the next student
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving student records: " + ex.Message);
        }
    }

    static void LoadStudentRecords()
    {
        if (File.Exists(dataFilePath))
        {
            try
            {
                string[] lines = File.ReadAllLines(dataFilePath);

                foreach (string line in lines)
                {
                    // Parse the stored information to recreate student records
                    string[] parts = line.Split('|');
                    string studentNumber = parts[0];
                    string studentName = parts[1];

                    Student newStudent = new Student(studentNumber, studentName);
                    studentRecords.Add(newStudent);

                    // Load marks, average, and result for each mark history entry
                    for (int i = 2; i < parts.Length; i += 3)
                    {
                        if (i + 2 < parts.Length) // Ensure there are enough elements
                        {
                            int[] marks = parts[i].Split(',').Select(int.Parse).ToArray();
                            double average = double.Parse(parts[i + 1]);
                            string result = parts[i + 2];

                            newStudent.AddMarkHistoryEntry(marks, average, result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading student records: " + ex.Message);
            }
        }
    }

    static void menus()
    {
        // Display the menu options
        Console.WriteLine("Menu Choices:");
        Console.WriteLine("1. Create a new student record");
        Console.WriteLine("2. Enter marks for a student");
        Console.WriteLine("3. Update a student record");
        Console.WriteLine("4. Show a student record");
        Console.WriteLine("5. Save & Quit the program");
        Console.Write("Enter your choice: ");
    }

    static string GenerateStudentNumber()
    {
        // Generate a random student number
        Random random = new Random();
        return random.Next(10000000, 99999999).ToString();
    }

    static void CreateANewStudentRecord()
    {
        // Create a new student record with a generated student number
        string studentNumber = GenerateStudentNumber();
        Console.Write("Enter the student's name: ");
        string studentName = Console.ReadLine();

        Student newStudent = new Student(studentNumber, studentName);
        studentRecords.Add(newStudent);

        Console.WriteLine($"New student record created. Student number: {studentNumber}");
    }

    static void EnterMarksForAStudent()
    {
        // Allow the user to enter marks for a student
        Console.Write("Enter the student number: ");
        string studentNumber = Console.ReadLine();

        Student student = FindStudentRecord(studentNumber);

        if (student != null)
        {
            student.EnterMarks();
        }
    }

    static void UpdateAStudentRecord()
    {
        // Allow the user to update marks for a student
        Console.Write("Enter the student number: ");
        string studentNumber = Console.ReadLine();

        Student student = FindStudentRecord(studentNumber);

        if (student != null)
        {
            student.UpdateMarks();
        }
    }

    static void ShowAStudentRecord()
    {
        // Display details for a specific student
        Console.Write("Enter the student number: ");
        string studentNumber = Console.ReadLine();

        Student student = FindStudentRecord(studentNumber);

        if (student != null)
        {
            student.DisplayDetails();
        }
    }

    static Student FindStudentRecord(string studentNumber)
    {
        // Find a student record based on the provided student number
        Student student = studentRecords.Find(s => s.StudentNumber == studentNumber);

        if (student == null)
        {
            Console.WriteLine("Student record not found.");
        }

        return student;
    }
}

class Student
{
    // Properties for student information
    public string StudentNumber { get; }
    public string Name { get; }
    public List<MarkHistoryEntry> MarkHistory { get; } = new List<MarkHistoryEntry>();

    // Constructor to initialize student properties
    public Student(string studentNumber, string studentName)
    {
        StudentNumber = studentNumber;
        Name = studentName;
    }

    // Method to allow the user to enter marks for a student
    public void EnterMarks()
    {
        Console.WriteLine($"Enter marks for {Name}:");
        int[] marks = new int[6];

        for (int i = 0; i < 6; i++)
        {
            Console.Write($"Enter mark {i + 1}: ");
            while (!int.TryParse(Console.ReadLine(), out marks[i]) || marks[i] < 0 || marks[i] > 100)
            {
                Console.WriteLine("Invalid input. Marks must be between 0 and 100. Try again.");
                Console.Write($"Enter mark {i + 1}: ");
            }
        }

        double average = CalculateAverage(marks);
        Console.WriteLine($"Average marks: {average}");

        string result = (average >= 40) ? "Passed" : "Failed";
        Console.WriteLine($"Result: {result}");

        AddMarkHistoryEntry(marks, average, result);
    }

    // Method to allow the user to update marks for a student
    public void UpdateMarks()
    {
        Console.WriteLine($"Update marks for {Name}:");
        int[] marks = new int[6];

        for (int i = 0; i < 6; i++)
        {
            Console.Write($"Enter updated mark {i + 1}: ");
            while (!int.TryParse(Console.ReadLine(), out marks[i]) || marks[i] < 0 || marks[i] > 100)
            {
                Console.WriteLine("Invalid input. Marks must be between 0 and 100. Try again.");
                Console.Write($"Enter updated mark {i + 1}: ");
            }
        }

        double average = CalculateAverage(marks);
        Console.WriteLine($"Updated average marks: {average}");

        string result = (average >= 40) ? "Passed" : "Failed";
        Console.WriteLine($"Updated result: {result}");

        AddMarkHistoryEntry(marks, average, result);
    }

    // Method to display all details for a student, including mark history
    public void DisplayDetails()
    {
        Console.WriteLine($"Student Details for {Name} (Student Number: {StudentNumber}):");

        foreach (var history in MarkHistory)
        {
            Console.WriteLine($"Marks: {string.Join(", ", history.Marks)}, Average: {history.Average}, Result: {history.Result}");
        }
    }

    // Method to calculate the average of an array of marks
    private double CalculateAverage(int[] marks)
    {
        return marks.Average();
    }

    // Method to add a new mark history entry for a student
    public void AddMarkHistoryEntry(int[] marks, double average, string result)
    {
        MarkHistoryEntry entry = new MarkHistoryEntry
        {
            Marks = marks,
            Average = average,
            Result = result
        };

        MarkHistory.Add(entry);
    }
}

// Class to represent a mark history entry with marks, average, and result
class MarkHistoryEntry
{
    public int[] Marks { get; set; }
    public double Average { get; set; }
    public string Result { get; set; }
}
