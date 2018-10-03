using System;
using System.Collections.Generic;
using Xunit;
using static PlagiarismDetector.PlagiarismDetector;

namespace PlagiarismDetectorTest
{
    public static class PlagiarismDetectorTest
    {
        [Fact]
        public static void Test1()
        {
            string syns = "Data/syns1.txt";
            string file1 = "Data/file1_1.txt";
            string file2 = "Data/file2_1.txt";

            Assert.Equal((double)1, GetMatchingPercentage(syns, file1, file2));
            Assert.Equal((double)1, GetMatchingPercentage(syns, file1, file2, 2));
        }

        [Fact]
        public static void Test2()
        {
            string syns = "Data/syns1.txt";
            string file1 = "Data/file1_1.txt";
            string file2 = "Data/file2_1_2.txt";

            Assert.Equal((double)0.5, GetMatchingPercentage(syns, file1, file2));
        }

        [Fact]
        public static void Test3()
        {
            string syns = "Data/syns2.txt";
            string file1 = "Data/file1_2.txt";
            string file2 = "Data/file2_2.txt";

            Assert.Equal((double)0.85, Math.Round(GetMatchingPercentage(syns, file1, file2), 2));
        }
    }
}
