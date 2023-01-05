# PlagiarismDetection

This is a command-line program that performs plagiarism detection using a N-tuple comparison algorithm allowing for synonyms in the text.

Since this program/algorithm may end up being used in other contexts such as a website, asynchronous methods are adapted to ensure UI responsiveness when I/O files are large.

Input: 

```
[path to file of synonyms] [path to file1] [path to file2] (optional; default = 3)[tuple size]
```

Output: 

```
Percentage of N-tuples in file1 that appear in file2
```

Example input:

```
s.txt f1.txt f2.txt 3
```

Example output:

```
100%
```
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

* Each N-tuple can only be marked plagiarized once - that is, once its matching counterpart has been found, we don't search further anymore. 


## Built With

* .NET Core 2.0

* Visual Studio 2017

## Author

* Lichen (Brittany) Zhang