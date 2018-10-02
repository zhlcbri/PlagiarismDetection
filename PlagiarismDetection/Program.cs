using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PlagiarismDetection
{
    class Program
    {
        private static List<List<string>> GetNTuples(string file, int n)
        {
            List<List<string>> nTuples = new List<List<string>>();

            try
            {
                using (StreamReader input = new StreamReader(file))
                {
                    string line;
                    while ((line = input.ReadLine()) != null)
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

            Console.WriteLine("There are " + nTuples.Count + " tuples in " + file);

            return nTuples;
        }

        private static List<HashSet<string>> GetSynonyms(string file)
        {
            List<HashSet<string>> synonyms = new List<HashSet<string>>();

            try
            {
                using (StreamReader input = new StreamReader(file))
                {
                    string line;
                    while ((line = input.ReadLine()) != null)
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

        private static bool Match(List<string> tuple1, List<string> tuple2, List<HashSet<string>> synonyms)
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

        private static int DetectPlagiarism(List<List<string>> tuples1, List<List<string>> tuples2, List<HashSet<string>> synonyms)
        {
            int count = 0;

            foreach (List<string> t1 in tuples1)
            {
                foreach (List<string> t2 in tuples2)
                {
                    if (Match(t1, t2, synonyms))
                    {
                        //Console.WriteLine("Matching tuples: ");
                        //Console.WriteLine(string.Join(" ", t1));
                        //Console.WriteLine(string.Join(" ", t2));
                        //Console.WriteLine();
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
                        // TODO print usage instructions
                        continue;
                    }

                    string synonymsFile = input[0];
                    string file1 = input[1];
                    string file2 = input[2];

                    int n = 0;
                    if (input.Length == 4) Int32.TryParse(input[3], out n);
                    if (n == 0) n = 3;

                    List<HashSet<string>> synonyms = GetSynonyms(synonymsFile);
                    List<List<string>> tuples1 = GetNTuples(file1, n);
                    List<List<string>> tuples2 = GetNTuples(file2, n);

                    int allTuples = tuples2.Count;
                    int matchingTuples = DetectPlagiarism(tuples1, tuples2, synonyms);
                    double matchingPercentage = (double)matchingTuples / (double)allTuples;

                    Console.WriteLine(matchingPercentage.ToString("P", CultureInfo.InvariantCulture));
                }
            }
        }
    }
}
