using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlagiarismDetector
{
    public class PlagiarismDetector
    {
        const string USAGE = "usage: [path to file of synonyms] [path to file1] [path to file2] (optional; default = 3)[tuple size]";

        /// <summary>
        /// Asynchronously returns a list of N-Tuples (alphanumeric only) extracted from input file
        /// </summary>
        /// <param name="file">input file to be parsed</param>
        /// <param name="n">tuple size</param>
        /// <returns></returns>
        private static async Task<List<List<string>>> GetNTuplesAsync(string file, int n)
        {
            List<List<string>> nTuples = new List<List<string>>();

            try
            {
                using (StreamReader input = new StreamReader(file))
                {
                    string line;
                    while ((line = await input.ReadLineAsync()) != null)
                    {
                        Regex regex = new Regex("[^a-zA-Z0-9 ]");
                        line = regex.Replace(line.Trim().ToLower(), "");

                        string[] words = line.Split(' ');
                        if (n > words.Length)
                        {
                            Console.WriteLine("n is too large for " + file);
                            continue;
                        }
                        for (int i = 0; i < words.Length - (n-1); i++)
                        {
                            List<string> tuple = new List<string>();
                            for (int j = 0; j < n; j++)
                            {
                                tuple.Add(words[i+j]);
                            }
                            nTuples.Add(tuple);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return nTuples;
        }

        /// <summary>
        /// Asynchronously returns a list of synonym sets extracted from input file
        /// </summary>
        private static async Task<List<HashSet<string>>> GetSynonymsAsync(string file)
        {
            List<HashSet<string>> synonyms = new List<HashSet<string>>();

            try
            {
                using (StreamReader input = new StreamReader(file))
                {
                    string line;
                    while ((line = await input.ReadLineAsync()) != null)
                    {
                        Regex regex = new Regex("[^a-zA-Z0-9 ]");
                        line = regex.Replace(line.Trim().ToLower(), "");

                        string[] words = line.Split(' ');
                        synonyms.Add(new HashSet<string>(words));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return synonyms;
        }

        /// <summary>
        /// Asynchronous helper method that returns true if tuple1 and tuple2 either have all elements identical,
        /// or have matching synonyms. Returns false otherwise.
        /// </summary>
        private static async Task<bool> MatchAsync(List<string> tuple1, List<string> tuple2, List<HashSet<string>> synonyms)
        {
            if (tuple1 == null || tuple2 == null) return false;
            if (tuple1.SequenceEqual(tuple2)) return true;

            foreach (HashSet<string> set in synonyms) {
                if (tuple1.Intersect(set).Any() && tuple2.Intersect(set).Any())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Takes in two lists of N-tuples and a list of synonyms set
        /// and asynchronously returns the number of matching N-tuples found.
        /// </summary>
        private static async Task<int> DetectPlagiarismAsync(List<List<string>> tuples1, List<List<string>> tuples2, List<HashSet<string>> synonyms)
        {
            int count = 0;

            var tuples1Copy = new List<List<string>>(tuples1);
            var tuples2Copy = new List<List<string>>(tuples2);

            foreach (var t1 in tuples1Copy)
            {
                foreach (var t2 in tuples2Copy)
                {
                    if (t1.Count > 0 && t2.Count > 0 && await MatchAsync(t1, t2, synonyms))
                    {
                        t1.Clear();
                        t2.Clear();
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Returns the percentage of N-tuples in file1 that have matching counterparts in file2
        /// Synchronous wrapper for calling asynchronous methods
        /// </summary>
        public static double GetMatchingPercentage(string synsFile, string file1, string file2, int n = 3)
        {
            double matchingPercentage = double.MinValue;

            var synonymTask = Task.Run(async () => await GetSynonymsAsync(synsFile));
            var task1 = Task.Run(async () => await GetNTuplesAsync(file1, n));
            var task2 = Task.Run(async () => await GetNTuplesAsync(file2, n));

            Task.WaitAll(new Task[] { synonymTask, task1, task2 });

            var synonyms = synonymTask.Result;
            var tuples1 = task1.Result;
            var tuples2 = task2.Result;

            var matchingTask = Task.Run(async () => await DetectPlagiarismAsync(tuples1, tuples2, synonyms));
            matchingTask.Wait();

            int matchingTuples = matchingTask.Result;
            int allTuples = tuples2.Count;
            matchingPercentage = (double)matchingTuples / (double)allTuples;

            return matchingPercentage;
        }

        public static void Main(string[] args)
        {
            double matchingPercentage = double.MinValue;
            string line;

            while ((line = Console.ReadLine()) != null)
            {
                string[] input = line.Split(' ');
                if (input.Length < 3 || input.Length > 4)
                {
                    Console.WriteLine(USAGE);
                    continue;
                }

                string synsFile = input[0];
                string file1 = input[1];
                string file2 = input[2];
                if (String.IsNullOrWhiteSpace(synsFile) 
                    || String.IsNullOrWhiteSpace(file1) || String.IsNullOrWhiteSpace(file2))
                {
                    Console.WriteLine(USAGE);
                    continue;
                }

                int n = 0;
                if (input.Length == 4) Int32.TryParse(input[3], out n);
                if (n == 0) n = 3;

                matchingPercentage = GetMatchingPercentage(synsFile, file1, file2, n);
                if (matchingPercentage != double.MinValue)
                {
                    Console.WriteLine(matchingPercentage.ToString("P", CultureInfo.InvariantCulture));
                }
                else
                {
                    Console.WriteLine(USAGE);
                }
            }
        }
    }
}
