using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalyzerLib
{
    public class FileHandler
    {
        string[] punctuationMarks; // список символов которые будут исключаться при выборке слов из текста                     

        string line; //строка считанная из файла, слово считанное из строки
        string[] words; //массив слов считанных из строки   
        List<string> lines; //коллекция строк распознанных из файла

        public FileHandler()
        {
            punctuationMarks = new string[] { ".", ",", "!", "?", "\"", "«", "»", ":", "(", ")", "•", "-", Environment.NewLine, "\t", "=", "’", "“", "№",
                                                   "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", ";", "–", "…", "…", " ", "  ", "#", "&", "[", "]", "„"};
        }

        /// <summary>
        /// Подсчет уникальных слов в анализируемом файле
        /// </summary>
        /// <param name="pathToFile">Путь к файлу формата .txt</param>
        /// <returns>Словарь с уникальными словами в качестве ключей и количеством их повторений в файле в качестве значений</returns>
        private Dictionary<string, int> CountUniqueWords(string pathToFile)
        {
            Dictionary<string, int> uniqueWords = new Dictionary<string, int>(); ; //словарь для хранения уникальных слов и подсчета их количества

            using (StreamReader sr = new StreamReader(pathToFile, UnicodeEncoding.UTF8))
            {
                while (sr.Peek() > 0)
                {
                    line = sr.ReadLine();
                    words = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < words.Length - 1; i++)
                    {
                        words[i] = words[i].DeleteSupplySymbols(punctuationMarks).ToLower();
                        if (String.IsNullOrEmpty(words[i]) || String.IsNullOrWhiteSpace(words[i])) continue;
                        if (uniqueWords.ContainsKey(words[i])) uniqueWords[words[i]]++;
                        else uniqueWords.Add(words[i], 1);
                    }
                    Array.Clear(words);
                }
            }
            return uniqueWords.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        public Dictionary<string, int> CountUniqueWordsPL(string pathToFile)
        {
            ConcurrentDictionary<string, int> uniqueWords = new ConcurrentDictionary<string, int>();
                        
            string[] stringResult = File.ReadAllLines(pathToFile, UnicodeEncoding.UTF8);

            ParallelLoopResult result = Parallel.ForEach(stringResult, str =>
            {
                words = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach(string word in words)
                {
                    string wordForWork = word;
                    wordForWork = wordForWork.DeleteSupplySymbols(punctuationMarks).ToLower();
                    if (String.IsNullOrEmpty(wordForWork) || String.IsNullOrWhiteSpace(wordForWork))
                        return;
                    uniqueWords.AddOrUpdate(wordForWork, 1, (wordForWork, u) => u + 1);
                }                
            });

            return uniqueWords.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);                        
        }
    }
}
