using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using TextAnalyzerLib;
using System.Reflection;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Net.Http.Json;

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

            #endregion

            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:5140");

            //var result = client.GetStringAsync("http://localhost:5140/api/Text");

            ChooseFile();

            stopWatch.Start();

            string[] stringsResult = File.ReadAllLines(pathToFile, UnicodeEncoding.UTF8);

            var request = new HttpRequestObject();
            request.TextStrings = stringsResult;

            string jsonString = JsonSerializer.Serialize(request);
            #region oldMethods

            //string allText = File.ReadAllText(pathToFile, UnicodeEncoding.UTF8);

            //uniqueWords = GetPrivateMethod(stringResult); //однопоточное выполнение

            //uniqueWords = GetPublicMethod(stringResult); //многопоточное выполнение

            //StringContent stringContent = new StringContent(allText, UnicodeEncoding.UTF8);

            #endregion

            string response = await HttpPost("http://localhost:5140/api/Text", jsonString);

            var httpResponseObject = JsonSerializer.Deserialize<HttpResponseObject>(response);

            uniqueWords = httpResponseObject.WordsCountDto;

            pathToResult = directory + "\\result.txt";


            if (uniqueWords.Count == 0)
            {
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

            async Task<String> HttpPost(string url, string content, string mediaType = "application/json")
            {
                HttpContent httrContent = new StringContent(content, UnicodeEncoding.UTF8, mediaType);
                var client = new HttpClient();
                using var response = await client.PostAsync(url, httrContent);
                var responseContent = await response.Content.ReadAsStringAsync();
                var httpResponseObject = JsonSerializer.Deserialize<HttpResponseObject>(responseContent);

                //var result = await client.GetFromJsonAsync<HttpResponseObject>(url, );
                                
                
                Console.WriteLine(httpResponseObject);

                return "End";
            }

            void ChooseFile()
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

            void writeToFile(ICollection<KeyValuePair<string, int>> collection, string path)
            {
                using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.UTF8))
                {
                    foreach (var pair in collection)
                    {
                        sw.WriteLine($"{pair.Key} - {pair.Value};");
                    }
                }
            }
        }
    }

    
    public class HttpRequestObject
    {
        public string[] TextStrings { get; set; }
    }

    
    public class HttpResponseObject
    {
        public Dictionary<string, int> WordsCountDto { get; set; }
    }

}