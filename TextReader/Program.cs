using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAnalyzerLib;
using System.Reflection;

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

            uniqueWords = GetPrivateMethod(pathToFile);

            Dictionary<string, int> GetPrivateMethod(string path)
            {
                var fileHandler = new FileHandler();
                
                var type = fileHandler.GetType();

                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);

                var method = methods.Where(m => m.Name.Contains("Count")).FirstOrDefault();

                return (Dictionary<string, int>)method.Invoke(fileHandler, new object[] { path });
            }
                                    
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