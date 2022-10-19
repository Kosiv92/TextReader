using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Net.Http.Json;
using CommonLibrary;

namespace TextReader
{
    public static class Programm
    {
        static async Task Main()
        {
            #region Переменные

            string directory; //рабочая директория (хранит файл источник и в нее будет записан результат)

            string pathToFile; //путь к исходному файлу

            string pathToResult; //путь к результату

            bool isFileExist; //проверка существования файла в указанной директории

            string fileName = "\\example.txt"; //имя исходного файла

            Stopwatch stopWatch = new Stopwatch(); //таймер на время выполнения программы

            TimeSpan ts; //время выполнения программы

            string elapsedTime; //строковое представление времени выполнения программы

            ICollection<KeyValuePair<string, int>> uniqueWords; //словарь для хранения уникальных слов и подсчета их количества
                                                                //
            string uri = "http://localhost:5140/api/Text"; //адрес сервера для направления запросов

            #endregion

            if (IsServerAvailable().Result) Console.WriteLine("Server is available"); //проверка доступности web-api
            else
            {
                Console.WriteLine("Server is not available. Application will be closed");
                Console.ReadKey();
                Environment.Exit(0);
            }
            

            ChooseFile(); //выбираем файл

            stopWatch.Start();

            string[] stringsResult = File.ReadAllLines(pathToFile, UnicodeEncoding.UTF8); //получаем масси строк из файла

            var request = new StringsDto(); //формируем библиотечный объект для отправки запроса на сервер
            request.Strings = stringsResult; //помещаем в объект данные которые необходимо обработать
            string jsonString = JsonSerializer.Serialize(request); //сериализуем объект в json-строку

            string response = await HttpPost(uri, jsonString); //получаес от сервера обработанные данные через post-запрос

            var httpResponseObject = JsonSerializer.Deserialize<WordsDto>(response); //десериализуем данные

            uniqueWords = httpResponseObject.wordsCount;

            pathToResult = directory + "\\result.txt";


            if (uniqueWords.Count == 0)
            {
                stopWatch.Stop();
                Console.WriteLine("Отсутствуют найденные слова! Работа приложения завершена!");
                Console.ReadKey();
            }
            else
            {
                writeToFile(uniqueWords, pathToResult);
                stopWatch.Stop();
                ts = stopWatch.Elapsed;
                elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
                Console.WriteLine($"Работа приложения завершена. Найдено {uniqueWords.Count} уникальных слов. Результат сохранены в файле \"result.txt\"\n" +
                    $"Время работы приложения {elapsedTime}");
                Process.Start("notepad", pathToResult); //запускаем файл с результатами
                Console.ReadKey();
            }


            async Task<String> HttpPost(string url, string content, string mediaType = "application/json") //post-запрос на сервер и получение данных в видн json-строки
            {
                HttpContent httrContent = new StringContent(content, UnicodeEncoding.UTF8, mediaType);
                var client = new HttpClient();
                using var response = await client.PostAsync(url, httrContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }


            void ChooseFile() //выбор файла для считывания текста
            {
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
            }


            void writeToFile(ICollection<KeyValuePair<string, int>> collection, string path) //запись словаря в файл
            {
                using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.UTF8))
                {
                    foreach (var pair in collection)
                    {
                        sw.WriteLine($"{pair.Key} - {pair.Value};");
                    }
                }
            }


            async Task<bool> IsServerAvailable() //проверка доступности сервера
            {
                var testConnectionResult = await new HttpClient().GetAsync(uri);

                if (testConnectionResult.StatusCode == System.Net.HttpStatusCode.OK) return true;
                else return false;
            }
        }
    }

}