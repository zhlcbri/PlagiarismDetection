# PlagiarismDetection


This is a command-line program that performs plagiarism detection using a N-tuple comparison algorithm allowing for synonyms in the text.

Input format: [path to file of synonyms] [path to file1] [path to file2] (optional; default = 3)[tuple size]

Output: Percentage of N-tuples in file1 that appear in file2

## Getting Started

## Running the program on Windows

Double-click on PlagiarismDetection\PlagiarismDetection\PlagiarismDetector.exe, and input commands in the format indicated.

## Running the program on Mac

## Running the tests

## Assumptions Made

* Synonym comparison is case-insensitive
* Non-alphanumeric characters are not to be compared
* Numeric characters are not ignored because we could have synonyms like "nine" and "9"


## Built With

* .NET Framework
* Visual Studio 2017
