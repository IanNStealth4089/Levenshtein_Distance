// Ian Nachreiner Period 2
// Semester 2, Project 1
// Levenshtein Distance - Find Paths between words
// I decided to tackle levenshtein distance by first developing
// a function which finds all the neigbors of a word. It goes
// through a list of all available letters, and all letter placements
// and checks if the constructed word is part of the dictionary.
// The algorithm begins by finding all the neigbors of word 1.
// All the neigbors which are found are added to a list, and their
// parent's index is tracked. I figured the best way to advance would
// be to choose some of these words and explore all its neighbors
// I could explore every word, but I think it would take way too long
// so I make my algorithm only choose the best words which haven't been
// chosen yet. I made a distance function to score the best words to
// use. The distance function first finds the length difference, before
// finding all the letters that could move to the correct place in
// the other word. The score is the length difference - the letters
// within moving distance. The function works as a edit distance tracker
// The 5 words that have the best distance score are chosen to be
// explored. Once the target word has been reached, the algorithm
// goes to each word, then its parent, and prints out the path.


using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Levenshtein
{
    internal class Program
    {
        static List<string> charList = new List<string>{"a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z",""};
        static List<string> Neighbors(string word1, Dictionary dictionary, ref List<string> pastWords, ref List<int> parent, int parentIndex)
        {
            List<string> neighbors = new List<string>();
            for(int i = 0; i < word1.Length*2+1; i++)
            {
                for(int j = 0; j < 27; j++)
                {
                    string tempWord = word1.Substring(0, i / 2) + charList[j] + word1.Substring((i + 1) / 2, word1.Length - ((i + 1) / 2));
                    if (dictionary.Contains(tempWord) && !pastWords.Contains(tempWord))
                    {
                        neighbors.Add(tempWord);
                        pastWords.Add(tempWord);
                        parent.Add(parentIndex);
                        //Console.WriteLine(tempWord);

                    }
                }
            }
            return neighbors;
        }
        static int Distance(string word1, string word2)
        {
            int score = Math.Max(word1.Length, word2.Length);
            int maxCharTravel = Math.Abs(word1.Length - word2.Length);
            List<int> travelDistance = new List<int>();
            string shortWord = word1;
            string longWord = word2;
            if (word1.Length > word2.Length)
            {
                shortWord = word2;
                longWord = word1;
            }
            for(int i = 0; i < shortWord.Length; i++)
            {
                for(int j = i; j <= Math.Min(i + maxCharTravel, longWord.Length-1); j++)
                {
                    if (shortWord[i] == longWord[j])
                    {
                        travelDistance.Add(j - i);
                        break;
                    }
                }
            }
            maxCharTravel = 0;
            for(int i = 0; i < travelDistance.Count; i++)
            {
                if (travelDistance[i] >= maxCharTravel)
                {
                    maxCharTravel = travelDistance[i];
                    score--;
                }
            }
            return score;
        }
        static int activeWordsMax = 5;

        static int chooseIndex(List<int> usedWordsScore, List<int> unusedWordsIndex)
        {
            bool goodEnough = false;
            Random rng = new Random();
            int index = 0;
            int min = int.MaxValue;
            for(int i = 0; i < usedWordsScore.Count; i++)
            {
                int val = usedWordsScore[i];
                if (val < min && unusedWordsIndex.Contains(i))
                {
                    min = val;
                    index = i;
                }
            }
            //double avg = usedWordsScore.Sum() / (double)usedWordsScore.Count();
            /*while (!goodEnough)
            {
                index = rng.Next(usedWordsScore.Count);
                if (Math.Pow(1-((usedWordsScore[unusedWordsIndex[index]]-min)/(double)(max-min)),10) > rng.NextDouble())
                {
                    goodEnough = true;
                }
            }*/
            //Console.WriteLine(usedWordsScore[index]);
            return index;
        }
        static List<string> usedWords;
        static List<string> activeWords;
        static List<int> checkedWordsIndex;
        static List<int> activeWordsIndex;
        static List<int> usedWordsScore;
        static List<int> usedWordsParent;
        static Dictionary<int,int> unusedWordsScore;
        static List<int> unusedWordsIndex;
        static void LevenshteinDistance(string word1, string word2)
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(word1 + " to " + word2);
            drawProgressBar(0, 1);
            int distance = 0;
            int startDistance = Distance(word1, word2);
            int word2Index = 0;
            Dictionary dictionary = new Dictionary();
            usedWords = new List<string>{word1};
            activeWords = new List<string>();
            checkedWordsIndex = new List<int> ();
            activeWordsIndex = new List<int>();
            usedWordsScore = new List<int> { Distance(word1, word2)};
            usedWordsParent = new List<int> { 0 };
            unusedWordsIndex = new List<int>();
            unusedWordsScore = new Dictionary<int,int>();
            Neighbors(word1, dictionary, ref usedWords, ref usedWordsParent,0);
            while (!usedWords.Contains(word2))
            {
                for (int i = usedWordsScore.Count; i < usedWords.Count; i++)
                {
                    usedWordsScore.Add(Distance(usedWords[i], word2));
                    unusedWordsScore.Add(i,Distance(usedWords[i], word2));
                    unusedWordsIndex.Add(i);
                }
                activeWords = new List<string>();
                activeWordsIndex = new List<int>();
                for (int i = 0; i < activeWordsMax; i++)
                {
                    int index = chooseIndex(usedWordsScore, unusedWordsIndex);
                    checkedWordsIndex.Add(index);
                    activeWords.Add(usedWords[index]);
                    unusedWordsScore.Remove(index);
                    unusedWordsIndex.Remove(index);
                    activeWordsIndex.Add(index);
                }
                for (int i = 0; i < activeWords.Count; i++)
                {
                    Neighbors(activeWords[i], dictionary, ref usedWords, ref usedWordsParent, activeWordsIndex[i]);
                }
                drawProgressBar(startDistance -usedWordsScore.Min(), startDistance);
            }
            for(int i = usedWordsScore.Count; i < usedWords.Count; i++)
            {
                if (usedWords[i] == word2)
                {
                    word2Index = i;
                }
            }
            drawProgressBar(1, 1);
            while(word2Index != 0)
            {
                Console.WriteLine(usedWords[word2Index]);
                word2Index = usedWordsParent[word2Index];
                distance++;
            }
            Console.WriteLine(word1);
            Console.WriteLine(distance);
        }
        class Dictionary
        {
            List<string> words = new List<string>();
            public Dictionary()
            {
                StreamReader reader = new StreamReader("../../../dictionarySorted.txt");
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    //if (line.Count() == 3) {
                        words.Add(line);
                    //}
                }
                reader.Close();
            }
            public bool Contains(string word)
            {
                return words.Contains(word);
            }
        }
        static void drawProgressBar(int progress, int size)
        {
            Console.SetCursorPosition(0, 1);
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("                    ");
            Console.SetCursorPosition(0, 1);
            Console.BackgroundColor = ConsoleColor.Green;
            for(int i = 0;i<progress*20/size; i++)
            {
                Console.Write(" ");
            }
            Console.SetCursorPosition(21, 1);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine((int)(progress * 100 / size) + "%   ");
        }
        static void Main(string[] args)
        {
            string word1 = "cat";
            string word2 = "dog";
            LevenshteinDistance(word1, word2);
            word1 = "dog";
            word2 = "cat";
            LevenshteinDistance(word1, word2);
            word1 = "puppy";
            word2 = "dog";
            LevenshteinDistance(word1, word2);
            /*for (int i = 0; i < 100; i++)
            {
                Console.WriteLine(chooseIndex(new List<int> { 0, 1, 2, 3, 4, 5 }));
            }*/
            /*List<string> pastWords = new List<string> { word1 };
            List<string> activeWords = new List<string>();
            List<string> nextWords = new List<string>();
            nextWords.AddRange(Neighbors(word1, dictionary, ref pastWords));
            activeWords = nextWords;
            nextWords = new List<string>();
            for (int i = 0; i < activeWords.Count; i++)
            {
                nextWords.AddRange(Neighbors(activeWords[i], dictionary, ref pastWords));
                Console.WriteLine(activeWords[i]);
            }
            activeWords = nextWords;
            for (int i = 0; i < activeWords.Count; i++)
            {
                Console.WriteLine(activeWords[i]);
            }*/
                    /*static int CorrelationScore(string word1, string word2)
        {
            int combinedLength = Math.Min(word1.Length, word2.Length);
            int score = Math.Max(word1.Length, word2.Length) - combinedLength;
            // Score is by default the difference in letter counts
            for (int i = 0; i < combinedLength; i++)
            {
                if (word1[i] != word2[i]){
                    score = score + 1;
                }
            }
            return score;
        }
        static void LevenshteinDistance(ref string word1, string word2, List<string> dictionary)
        {
            int minScore = int.MaxValue;
            int minI = 0;
            int minJ = 0;
            List<string> bestWords = new List<string>();
            Random rng = new Random();
            for (int i = 0; i < word1.Length*2+1; i++)
            {
                for (int j = 0; j < 27; j++)
                {
                    string tempWord = word1.Substring(0, i / 2) + charList[j] + word1.Substring((i + 1) / 2, word1.Length - ((i + 1) / 2));
                    /*if (dictionary.Contains(tempWord))
                    {
                        Console.WriteLine(tempWord);
                    }
                    if(CorrelationScore(tempWord, word2) < minScore && dictionary.Contains(tempWord) && word1 != tempWord)
                    {
                        bestWords.Add(tempWord);
                        minScore = CorrelationScore(tempWord, word2);
                    }
                }
            }
            word1 = bestWords[rng.Next(bestWords.Count)];
        }*/
                    /*static void NeighborList(List<string> dictionary)
        {
            List<List<int>> dictionaryNeighbors = new List<List<int>>();
            for (int i = 0; i < dictionary.Count; i++)
            {
                List<int> neighborList = new List<int>();
                for (int j = 0; j < dictionary[i].Length; j++)
                {
                    for (int k = 0; k < 27; k++)
                    {
                        string tempWord = dictionary[i].Substring(0, j / 2) + charList[k] + dictionary[i].Substring((j + 1) / 2, dictionary[i].Length - ((j + 1) / 2));
                        for (int l = 0; l < dictionary.Count; l++)
                        {
                            if (dictionary[l] == tempWord)
                            {
                                neighborList.Add(l);
                            }
                        }
                    }
                }
                dictionaryNeighbors.Add(neighborList);
                Console.WriteLine(i);
            }
        }*/
        }
    }
}
