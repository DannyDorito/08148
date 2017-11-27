This program to be used by a student who wishes to submit a program in
the form of a Visual Studio solution.  The student should run this
program to check that his or her solution conforms to the submission
requirements.

This program is a Console application with a single command line argument,
i.e. the pathname to the student Visual Studio solution that is to be checked.
Two ways to supply pathname argument: 
  1. Start the program and enter pathname at console window when prompted,
  2. Edit properties of this solution, select the Debug tab of the 
     submissionConformance project properties.  In the Command line 
     arguments text box paste the pathname and save the properties.
     
Two kinds of Exception may be raised by this program.
1. Exceptions of type ExceptionConformance indicate a problem with the 
   solution being checked.  The student must fix these problems.
2. All other types of Exception indicate a problem with this program.  
   Please email details of the problem to L.Bottaci@hull.ac.uk

See Program.cs for more details.  The source code of this program is made available for 
educational purposes only with no warranty expressed or implied.  
This program should not be modified by the student.
