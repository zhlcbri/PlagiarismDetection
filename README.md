# PlagiarismDetection


This is a command-line program that performs plagiarism detection using a N-tuple comparison algorithm allowing for synonyms in the text.

Input format: [path to file of synonyms] [path to file1] [path to file2] (optional; default = 3)[tuple size]

Example:
```

```

Output: Percentage of N-tuples in file1 that appear in file2

## Getting Started

## Running the program on Windows

Double-click on PlagiarismDetection\PlagiarismDetection\PlagiarismDetector.exe, and input commands in the format indicated above.

## Running the program on Mac

Install [Mono](https://www.mono-project.com/) to compile C#, either from this [download page](https://www.mono-project.com/download/stable/) or using [Homebrew](https://brew.sh/):

```
brew install mono
```

## Running the tests

## Assumptions Made

* Synonym comparison is case-insensitive
* Non-alphanumeric characters are not to be compared
* Numeric characters are not ignored because we could have synonyms like "nine" and "9"


## Built With

* .NET Core 2.0
* Visual Studio 2017

