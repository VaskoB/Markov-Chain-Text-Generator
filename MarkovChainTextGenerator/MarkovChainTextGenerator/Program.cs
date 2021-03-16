using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkovChainTextGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Markov("file.txt", 2, 50));
        }

        static string Join(string a, string b)
        {
            return a + " " + b;
        }

        static string Markov(string filePath, int prefixSize, int textSize)
        {
            if(prefixSize < 1)
            {
                throw new ArgumentException("Prefix size cannot be < 1.");
            }

            string text;
            using(StreamReader reader = new StreamReader(filePath))
            {
                text = reader.ReadToEnd();
            }
            var words = text.Split();

            if(textSize < prefixSize || words.Length < textSize)
            {
                throw new ArgumentException("Text size is out of range");
            }

            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
            for(int i = 0; i < words.Length - prefixSize; i++)
            {
                var prefix = words.Skip(i).Take(prefixSize).Aggregate(Join);
                string suffix;

                if(i + prefixSize < words.Length)
                {
                    suffix = words[i + prefixSize];
                }
                else
                {
                    suffix = "";
                }

                if(dictionary.ContainsKey(prefix))
                {
                    dictionary[prefix].Add(suffix);
                }
                else
                {
                    dictionary.Add(prefix, new List<string>());
                    dictionary[prefix].Add(suffix);
                }
            }

            List<string> result = new List<string>();
            int n = 0;
            Random random = new Random();
            int roll = random.Next(dictionary.Count);
            string randomPrefix = dictionary.Keys.Skip(roll).Take(1).Single();
            result.AddRange(randomPrefix.Split());

            while(true)
            {
                var suffix = dictionary[randomPrefix];

                if(suffix.Count == 1)
                {
                    if(suffix[0] == "")
                    {
                        return result.Aggregate(Join);
                    }
                    result.Add(suffix[0]);
                }
                else
                {
                    roll = random.Next(suffix.Count);
                    result.Add(suffix[roll]);
                }
                if(result.Count >= textSize)
                {
                    return result.Take(textSize).Aggregate(Join);
                }
                n++;
                randomPrefix = result.Skip(n).Take(prefixSize).Aggregate(Join);
            }
        }
    }
}
