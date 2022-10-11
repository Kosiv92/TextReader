using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAnalyzerLib
{
    public class FileHandler
    {
        char[] punctuationMarks; // список символов которые будут исключаться при выборке слов из текста

        Dictionary<string, int> uniqueWords; //словарь для хранения уникальных слов и подсчета их количества

        public FileHandler()
        {
            punctuationMarks = new char[] { '.', ',', '!', '?', '"', '«', '»', ':', '(', ')', '•', '-',
                                                   '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', ';', '–', '…', ' '  };

            uniqueWords = new Dictionary<string, int>();
        }

        /// <summary>
        /// Подсчет уникальных слов в анализируемом файле
        /// </summary>
        /// <param name="pathToFile">Путь к файлу формата .txt</param>
        /// <returns>Словарь с уникальными словами в качестве ключей и количеством их повторений в файле в качестве значений</returns>
        private Dictionary<string, int> CountUniqueWords(string pathToFile)
        {
            using (StreamReader sr = new StreamReader(pathToFile, UnicodeEncoding.UTF8))
            {
                string line; //строка считанная из файла, слово считанное из строки
                string[] words; //массив слов считанных из строки                

                while (sr.Peek() > 0)
                {
                    line = sr.ReadLine();
                    words = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < words.Length; i++)
                    {
                        words[i] = words[i].Trim(punctuationMarks).ToLower();
                        if (String.IsNullOrEmpty(words[i])) continue;
                        if (uniqueWords.ContainsKey(words[i])) uniqueWords[words[i]]++;
                        else uniqueWords.Add(words[i], 1);
                    }
                    Array.Clear(words);
                }
            }
            
            return uniqueWords = uniqueWords.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            
        }
    }
}
