# PlagiarismDetection

This is a command-line program that performs plagiarism detection using a N-tuple comparison algorithm allowing for synonyms in the text.

Since this program/algorithm may end up being used in other contexts such as a website, asynchronous methods are adapted to ensure UI responsiveness when I/O files are large.

Input format: 

```
[path to file of synonyms] [path to file1] [path to file2] (optional; default = 3)[tuple size]
```

Output: 

```
Percentage of N-tuples in file1 that appear in file2
```

Example input:

```
Data\syns1.txt Data\file1_1.txt Data\file2_1.txt 3
```

Example output:

```
100%
```

Sample test cases can be found in PlagiarismDetectorTest\bin\Debug\netcoreapp2.0\Data.

## Running the program on Windows

Double-click on PlagiarismDetection\PlagiarismDetector.exe, and input commands in the format indicated above.

## Running the program on Mac

Install [Mono](https://www.mono-project.com/) to compile C#, either from this [download page](https://www.mono-project.com/download/stable/) or using [Homebrew](https://brew.sh/):

```
brew install mono
```

In the source file directory:

```
mcs PlagiarismDetector.cs // compile
mono PlagiarismDetector.exe // run
```

## Assumptions

* Synonym comparison is case-insensitive

* Non-alphanumeric characters are not to be compared

* Numeric characters are not ignored because we could have synonyms like "nine" and "9"


## Built With

* .NET Core 2.0

* Visual Studio 2017

## Author

* Lichen (Brittany) Zhang