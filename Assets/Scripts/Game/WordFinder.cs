using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.WordComparison
{
    public class WordFinder
    {
        private readonly int minWordLength;
        private readonly List<List<string>> words = new List<List<string>>();

        public WordFinder(IEnumerable<TextAsset> dictionaries, char separator)
        {
            foreach (var dictionary in dictionaries)
                words.Add(new List<string>(dictionary.text.Split(separator)));

            minWordLength = words.Select(x => x)
                .OrderBy(word => word[0].Length)
                .First()[0].Length;
        }

        public bool FindWord(string word)
        {
            var wordLength = word.Length - minWordLength;

            if (wordLength < 0 || wordLength > words.Count - 1)
                return false;

            word = word.ToLower();

            var searchResult = words[wordLength].Find(x => x == word);
            
            var isWordFound = !string.IsNullOrEmpty(searchResult);
            
            return isWordFound;
        }

        public List<string> GetWordsListWhithLength(int length)
        { 
            var list = new List<string>();
            foreach (var word in words)
            {
                foreach (var w in word)
                {
                    if (w.Length.Equals(length))
                        list.Add(w);
                }
            }
            return list;
        }
    }
}