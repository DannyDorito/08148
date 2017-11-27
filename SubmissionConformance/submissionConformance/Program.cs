using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Ionic.Zip;

namespace submissionConformance {

  /// <remarks>
  /// This program to be used by a student who wishes to submit a program in
  /// the form of a Visual Studio solution.  The student should run this
  /// program to check that his or her solution conforms to the submission requirements.
  /// Two kinds of Exception may be raised by this program.
  /// Exceptions of type ExceptionConformance indicate a problem with the solution 
  /// being checked.  The student must fix these problems.
  /// All other types of Exception indicate a problem with this program.  
  /// Please email full details of such problems to L.Bottaci@hull.ac.uk
  /// This program should not be modified by the student.
  /// </remarks>
  class Program {

    /// <summary>
    /// The name of the main Visual Studio project in the submission.
    /// </summary>
    const String progName = "processFlow";

    /// <summary>
    /// The full pathname to the Visual Studio solution.
    /// Read pathname from program properties or console window.
    /// </summary>
    static String solutionLocation = null;

    /// <summary>
    /// The name of the output file.
    /// </summary>
    const String outputFile = "out.xml";

    /// <summary>
    /// The max number of milliseconds for which student program should execute.
    /// </summary>
    const int timeout = 500;

    /// <summary>
    /// Location of executable within solution.
    /// </summary>
    static String binPath = @"\" + progName + @"\Bin\Debug";

    /// <summary>
    /// Creation time of the dummy report file included in supplied code.
    /// Dummy report file should be overwritten by student report file 
    /// and hence should have a later date.
    /// </summary>
    //static readonly DateTime suppliedCodeTime = new DateTime(2016, 3, 11);

    /// <summary>
    /// Pathname of executable program
    /// </summary>
    static String exePath;

    /// <summary>
    /// First command line argument for progName.
    /// Edit as required, "" if not required.
    /// </summary>
    const String arg1 = "";

    /// <summary>
    /// Second command line argument for progName.
    /// Edit as required, "" if not required.
    /// </summary>
    const String arg2 = "";

    /// <summary>
    /// Third command line argument for progName.
    /// Edit as required, "" if not required.
    /// </summary>
    const String arg3 = "";

      /// <summary>
      /// True if report should be present in solution.
      /// </summary>
    const bool reportRequired = true;

    /// <summary>
    /// Name of report file less the suffix
    /// </summary>
    const string reportRoot = "report";

    /// <summary>
    /// Allowable report file name suffixes.
    /// </summary>
    static List<string> suffixes = new List<string>() { ".docx", ".doc", ".pdf" };

    /// <summary>
    /// Aim to execute a given program contained within a Visual Studio solution.
    /// Supply the location of the solution.
    /// Edit String constants above to set the name of the program and any command line arguments.
    /// Check for the presence of the executable program in the solution.
    /// Execute the executable with the command line arguments.
    /// Check for the presence of an output file.
    /// The given program is executed in the working directory of this program. 
    /// If no problems detected then this program terminates and Console window closed.  
    /// If Console window does not close then a problem has been found and a message 
    /// is written to the Console window.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args) {
      Console.WindowHeight = 58;
      Console.WindowWidth = 160;
      Console.WindowLeft = 0;
      Console.WindowTop = 0;
      Console.BufferHeight = 400;
      Console.BufferWidth = 800;
      if (solutionLocation == null) {
        // if exactly one arg not supplied, read arg from Console
        if (args.Length == 1) {
          solutionLocation = args[0];
        }
        else {
          Console.WriteLine("Enter full path-name to Visual Studio Solution:");
          solutionLocation = Console.ReadLine();
        }
      }
      try {
        CheckSolutionPathname(solutionLocation);
        DirectoryInfo solutionDI;
        try {
          solutionDI = new DirectoryInfo(solutionLocation);
        }
        catch (Exception e)  {
          Console.WriteLine(e.Message);
          Console.WriteLine("Problem in pathname: " + solutionLocation);
          throw new ExceptionConformance(e.Message);
        }
        binPath = solutionLocation + "\\" + progName + "\\Bin\\Debug";
        exePath = binPath + "\\" + progName + ".exe";
        CheckExecutablePathname(solutionDI, exePath);
        String outFile = binPath + "\\" + outputFile;
        if (RemoveFileIfPresent(outFile)) {
          Console.WriteLine("In preparation for execution, deleted existing file: " + outFile);
        }
        ExecuteProgram(exePath);
        CheckFilePresent(outFile);
        if (reportRequired) {
          // Report file, called "report.(docx | doc | pdf)" must be present in solution directory.
          CheckReportPresent(solutionLocation + "\\" + reportRoot);
        }
        RemoveDirectoryIfPresent(solutionLocation + "\\" + "TestResults");
        RemoveDirectoryIfPresent(solutionLocation + "\\" + ".parasoft.dat");
        RemoveDirectoryIfPresent(solutionLocation + "\\" + ".vs");
        ZipSolution();
        //var options = new ReadOptions { StatusMessageWriter = Console.Out };
        //using (ZipFile zip = ZipFile.Read(ZipFileToCreate, options)) {
        //  // This call to ExtractAll() assumes:
        //  //   - none of the entries are password-protected.
        //  //   - want to extract all entries to current working directory
        //  //   - none of the files in the zip already exist in the directory;
        //  //     if they do, the method will throw.
        //  Directory.CreateDirectory(solutionLocation + "\\tmp");
        //  Directory.SetCurrentDirectory(solutionLocation + "\\tmp");
        //  zip.ExtractAll(ZipFileToCreate);
        //}
      }
      catch (ExceptionConformance e) {
        Console.WriteLine(e.Message);
        Console.WriteLine("There are problems as reported above.");
        Console.WriteLine("These must be fixed before the solution can be submitted.");
        Console.WriteLine("Press any key to exit SubmissionConformance program.");
        Console.ReadKey();
        return;
      }
      catch (Exception e) {
        Console.WriteLine(e.Message);
        Console.WriteLine("There are problems with the SubmissionConformance program.");
        Console.WriteLine("These must be fixed before the solution can be checked.");
        Console.WriteLine("Press any key to exit SubmissionConformance program.");
        Console.ReadKey();
        return;
      }
      Console.WriteLine("No problems found.");
      Console.WriteLine("The solution zip file conforms to submission requirements.");
      Console.WriteLine("Press any key to exit SubmissionConformance program.");
      Console.ReadKey();
    }

    /// <summary>
    /// Check location of the executable file in the main 
    /// project \Bin\Debug directory.  If problem found then 
    /// Console window does not close and message displayed.
    /// </summary>
    /// <param name="solutionDI"></param>
    private static void CheckExecutablePathname(DirectoryInfo solutionDI, String exePath) {
      DirectoryInfo[] projectDIs = solutionDI.GetDirectories();
      if (projectDIs.Length == 0) {
        Console.WriteLine("Problem: no directories in the solution location: " + solutionDI.FullName);
        throw new ExceptionConformance("Exception in CheckExecutablePathname().");
      }
      foreach (DirectoryInfo projectDI in projectDIs) {
        if (projectDI.Name.Equals(progName)) {  // mainProjectDirectory
          if (!File.Exists(exePath)) {
            Console.WriteLine("Problem: no executable file found:");
            Console.WriteLine("  " + exePath);
            Console.WriteLine("Solution must be built before submission.");
            throw new ExceptionConformance("Exception in CheckExecutablePathname().");
          }
          return;
        }
      }
      Console.WriteLine("Problem: no executable file found, expecting:");
      Console.WriteLine("  " + exePath);
      throw new ExceptionConformance("Exception in CheckExecutablePathname().");

    }

    /// <summary>
    /// Execute the student program for no more than timeout milliseconds.
    /// </summary>
    /// <param name="progFile"></param>
    private static void ExecuteProgram(String progFile) {
      ProcessStartInfo procInfo = new ProcessStartInfo();
      procInfo.FileName = progFile;
      procInfo.WorkingDirectory = Directory.GetParent(progFile).FullName;
      procInfo.UseShellExecute = false;
      procInfo.Arguments = arg1 + " " + arg2 + " " + arg3;
      using (Process proc = new Process()) {
        proc.StartInfo = procInfo;
        try {
          proc.Start();
          if (!proc.WaitForExit(timeout)) {  // WaitForExit(t) true if exits before t
            // process takes longer than timeout milliseconds
            Console.WriteLine("Program execution exceeded " + timeout + " millisecs, is the program waiting for user input?");
            throw new ExceptionConformance("Exception in ExecuteProgram(), timeout.");
          }
        }
        catch (ExceptionConformance e) {
          throw new ExceptionConformance("Exception in ExecuteProgram().");
        }
        catch (Exception e) {
          String message = "Cannot start process: " + procInfo.FileName + " "
                            + procInfo.Arguments + "\n  " + e.Message;
          Console.WriteLine(message);
          throw new ExceptionConformance("Exception in ExecuteProgram().");
        }
      }
    }

    /// <summary>
    /// Check no spaces in path name to solution.
    /// </summary>
    /// <param name="solutionLocation"></param>
    private static void CheckSolutionPathname(string solutionLocation) {
      String[] solnParts = solutionLocation.Split(new char[] { ' ' });
      if (solnParts.Length > 1) {
          Console.WriteLine("Pathname to solution location must not contain spaces: " + solutionLocation);
          Console.WriteLine("Move solution to another location.");
          throw new ExceptionConformance("Exception in CheckSolutionPathname().");
      }
    }

    private static bool CheckFilePresent(String file) {
      if (!File.Exists(file)) {
        Console.WriteLine("File not found: " + file);
        throw new ExceptionConformance("Exception in CheckFilePresent().");
      }
      return true;
    }

    /// <summary>
    /// Report file, called "report.(docx | doc | pdf)" must be present in solution directory.
    /// </summary>
    /// <param name="file"></param>
    private static void CheckReportPresent(String file) {
      bool fileFound = false;
      foreach (string suffix in suffixes) {
        if (File.Exists(file + suffix)) {
          fileFound = true;
          break;
        }
      }
      if (!fileFound) {
        Console.WriteLine("Report not found: " + file);
        throw new ExceptionConformance("Exception in CheckReportPresent().");
      }
      //if (DateTime.Compare(fileInfo.CreationTime, suppliedCodeTime) < 1) {
      //  Console.WriteLine("Report file date too early, check your report file replaces existing file.");
      //  throw new ExceptionConformance("Exception in CheckReportPresent().");
      //}
      return;
    }

    /// <summary>
    /// True if file removed.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    private static bool RemoveFileIfPresent(String file) {
      if (File.Exists(file)) {
        try {
          File.Delete(file);
          return true;
        }
        catch (Exception e) {
          Console.WriteLine("Not able to delete: " + file + ". " + e.Message);
          throw new ExceptionConformance("Exception in RemoveFileIfPresent().");
        }
      }
      return false;
    }

    /// <summary>
    /// True if directory removed.
    /// </summary>
    /// <param name="directory"></param>
    /// <returns></returns>
    private static bool RemoveDirectoryIfPresent(String directory) {
      if (Directory.Exists(directory)) {
        try {
          Directory.Delete(directory, true);
          return true;
        }
        catch (Exception e) {
          Console.WriteLine("Not able to delete: " + directory + ". " + e.Message);
          throw new ExceptionConformance("Exception in RemoveDirectoryIfPresent().");
        }
      }
      return false;
    }

    /// <summary>
    /// Compress student VS submission.
    /// Assumes zip file does not already exist.
    /// </summary>
    private static void ZipSolution() {
      String ZipFileToCreate = solutionLocation + ".zip";
      if (File.Exists(ZipFileToCreate)) {
        Console.WriteLine("Zipfile: " + ZipFileToCreate + " already exists! Please remove.");
        throw new ExceptionConformance("Exception in ZipSolution().");
      }
      else {
        using (ZipFile zip = new ZipFile()) {
          zip.StatusMessageTextWriter = System.Console.Out;
          zip.AddDirectory(solutionLocation);
          zip.Save(ZipFileToCreate);
          Console.WriteLine("Zipfile created: " + ZipFileToCreate);
        }
      }
    }
  }

}
