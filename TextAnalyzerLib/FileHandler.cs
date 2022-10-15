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
        Dictionary<string, int> _dictionary; //словарь для хранения уникальных слов и подсчета их количества

        ConcurrentDictionary<string, int> _concurrentDictionary; //словарь для хранения уникальных слов и подсчета их количества поддерживающих многопоточную работу

        string[] punctuationMarks; // список символов которые будут исключаться при выборке слов из текста                     

        string line; //строка считанная из файла, слово считанное из строки

        string[] words; //массив слов считанных из строки
                        
        List<string> lines; //коллекция строк распознанных из файла


        public FileHandler()
        {
            punctuationMarks = new string[] { ".", ",", "!", "?", "\"", "«", "»", ":", "(", ")", "•", "-", Environment.NewLine, "\t", "=", "’", "“", "№",
                                                   "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", ";", "–", "…", "…", " ", "  ", "#", "&", "[", "]", "„"};

            _dictionary = new Dictionary<string, int>();

            _concurrentDictionary = new ConcurrentDictionary<string, int>();
        }

        /// <summary>
        /// Подсчет уникальных слов в анализируемом файле
        /// </summary>
        /// <param name="strings">Массив строк содержащих искомые слова</param>
        /// <returns>Словарь с уникальными словами в качестве ключей и количеством их повторений в файле в качестве значений</returns>
        private Dictionary<string, int> CountUniqueWords(string[] strings)
        {            
            for(int i = 0; i < strings.Length; i++)
            {
                words = strings[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int y = 0; y < words.Length; y++)
                {
                    words[y] = words[y].DeleteSupplySymbols(punctuationMarks).ToLower();
                    if (String.IsNullOrEmpty(words[y]) || String.IsNullOrWhiteSpace(words[y])) continue;
                    if (_dictionary.ContainsKey(words[y])) _dictionary[words[y]]++;
                    else _dictionary.Add(words[y], 1);
                }
                Array.Clear(words);
            }                        
            return _dictionary.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Подсчет уникальных слов в анализируемом файле в многопоточном режиме
        /// </summary>
        /// <param name="strings">Массив строк содержащих искомые слова</param>
        /// <returns>Словарь с уникальными словами в качестве ключей и количеством их повторений в файле в качестве значений</returns>
        public Dictionary<string, int> CountUniqueWordsPL(string[] strings)
        {                 
            ParallelLoopResult result = Parallel.ForEach(strings, SaveWordsToConcurrentDictionary);

            return _concurrentDictionary.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);                        
        }

        /// <summary>
        /// Поиск отдельных слов и строке и сохранение их в словарь
        /// </summary>
        /// <param name="str">Строка для анализа наличия слов</param>
        private void SaveWordsToConcurrentDictionary(string str)
        {
            words = str.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                string wordForWork = word;
                wordForWork = wordForWork.DeleteSupplySymbols(punctuationMarks).ToLower();
                if (String.IsNullOrEmpty(wordForWork) || String.IsNullOrWhiteSpace(wordForWork))
                    return;
                _concurrentDictionary.AddOrUpdate(word, 1, (word, u) => u + 1);
            }
        }                
    }
}
