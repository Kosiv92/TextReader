using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextReader
{
    public static class Programm
    {
        static void Main()
        {
            #region Переменные

            string directory; //рабочая директория (хранит файл источник и в нее будет записан результат)
            
            string pathToFile; //путь к исходному файлу

            string pathToResult; //путь к результату

            bool isFileExist; //проверка существования файла в указанной директории

            string fileName = "\\example.txt"; //имя исходного файла

            char[] punctuationMarks = new char[] { '.', ',', '!', '?', '"', '«', '»', ':', '(', ')', '•', '-',
                                                   '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', ';', '–', '…', ' '  }; // список символов которые будут исключаться при выборке слов из текста

            Dictionary<string, int> uniqueWords = new Dictionary<string, int>(); //словарь для хранения уникальных слов и подсчета их количества

            #endregion

            Console.Write($"Укажите путь к папке, хранящей файл с именем \"{fileName}\" в формате \"D:\\Directory\": ");
            do
            {
                directory = Console.ReadLine();
                pathToFile = directory + fileName;
                isFileExist = File.Exists(pathToFile);
                if (!isFileExist)
                {
                    Console.WriteLine($"Файл \"{fileName}\" отсутствует по указанному пути. Необходимо указать путь к папке с файлом в формате \"D:\\Directory\\\"");
                    Console.Write("Попробуйте снова: ");
                }
                else
                {
                    Console.WriteLine("Файл найден. Начните обработку нажатием любой клавиши");
                    Console.ReadKey();
                }
            } while (!isFileExist);
                        
            
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
            uniqueWords = uniqueWords.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            pathToResult = directory + "\\result.txt";

            if (uniqueWords.Count == 0)
            {
                Console.WriteLine("Отсутствуют найденные слова! Работа приложения завершена!");
                Console.ReadKey();
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(pathToResult, false, System.Text.Encoding.UTF8))
                {
                    foreach (var pair in uniqueWords)
                    {
                        sw.WriteLine($"{pair.Key} - {pair.Value};");
                    }
                }
                Console.WriteLine($"Работа приложения завершена. Найдено {uniqueWords.Count} уникальных слов. Результат сохранены в файле \"result.txt\"");
                Process.Start("notepad", pathToResult); //запускаем файл с результатами
                Console.ReadKey();
            }
        }
    }

}