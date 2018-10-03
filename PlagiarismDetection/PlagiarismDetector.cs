using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlagiarismDetection
{
    class PlagiarismDetector
    {
        const string USAGE = "usage: [path to file of synonyms] [path to file1] [path to file2] (optional; default = 3)[tuple size]";

        /// <summary>
        /// Returns a list of N-Tuples (alphanumeric only) extracted from input file
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

        private static async Task<bool> Match(List<string> tuple1, List<string> tuple2, List<HashSet<string>> synonyms)
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

        private static async Task<int> DetectPlagiarism(List<List<string>> tuples1, List<List<string>> tuples2, List<HashSet<string>> synonyms)
        {
            int count = 0;

            var tuples1Copy = new List<List<string>>(tuples1);
            var tuples2Copy = new List<List<string>>(tuples2);

            foreach (var t1 in tuples1Copy)
            {
                foreach (var t2 in tuples2Copy)
                {
                    if (t1.Count > 0 && t2.Count > 0 && await Match(t1, t2, synonyms))
                    {
                        t1.Clear();
                        t2.Clear();
                        count++;
                    }
                }
            }

            return count;
        }

        public static void Main(string[] args)
        {
            InputConverter.ConvertInput(args);
        }

        public static class InputConverter
        {
            public static void ConvertInput(string[] args)
            {
                string line;
                while ((line = Console.ReadLine()) != null)
                {
                    string[] input = line.Split(' ');
                    if (input.Length < 3 || input.Length > 4)
                    {
                        Console.WriteLine(USAGE);
                        continue;
                    }

                    string synonymsFile = input[0];
                    string file1 = input[1];
                    string file2 = input[2];
                    if (String.IsNullOrWhiteSpace(synonymsFile) 
                        || String.IsNullOrWhiteSpace(file1)
                        || String.IsNullOrWhiteSpace(file2))
                    {
                        Console.WriteLine(USAGE);
                        continue;
                    }

                    int n = 0;
                    if (input.Length == 4) Int32.TryParse(input[3], out n);
                    if (n == 0) n = 3;

                    var synonymTask = Task.Run(async () => await GetSynonymsAsync(synonymsFile));
                    var task1 = Task.Run(async () => await GetNTuplesAsync(file1, n));
                    var task2 = Task.Run(async () => await GetNTuplesAsync(file2, n));

                    Task.WaitAll(new Task[] { synonymTask, task1, task2 });

                    var synonyms = synonymTask.Result;
                    var tuples1 = task1.Result;
                    var tuples2 = task2.Result;

                    var matchingTask = Task.Run(async () => await DetectPlagiarism(tuples1, tuples2, synonyms));
                    matchingTask.Wait();

                    int matchingTuples = matchingTask.Result;
                    int allTuples = tuples2.Count;
                    double matchingPercentage = (double)matchingTuples / (double)allTuples;

                    Console.WriteLine(matchingPercentage.ToString("P", CultureInfo.InvariantCulture));
                }
            }
        }
    }
}
