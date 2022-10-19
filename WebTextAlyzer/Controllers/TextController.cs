using Microsoft.AspNetCore.Mvc;
using TextAnalyzerLib;
using CommonLibrary;

namespace WebTextAlyzer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TextController : Controller
    {
        FileHandler _fileHandler;

        List<string> _strings;

        public TextController()
        {
            _fileHandler = new FileHandler();
            _strings = new List<string>();
        }

        /// <summary>
        /// Подчет уникальных слов в массиве строк
        /// </summary>
        /// <param name="requestObject">Объект хранящий массив строк</param>
        /// <returns>Json-файл с результатами подсчета уникальных слов</returns>
        [HttpPost]
        public ActionResult<WordsDto> GetUniqueWords([FromBody] StringsDto requestObject)
        {
            var dictionary = _fileHandler.CountUniqueWordsPL(requestObject.Strings);
            var response = new WordsDto();
            response.wordsCount = dictionary;
            return response;
        }

        /// <summary>
        /// Проверка доступности сервера
        /// </summary>
        /// <returns>Результат get-запроса</returns>
        [HttpGet]
        public ActionResult<String> IsAvailable()
        {            
            return Ok();
        }

    }   

}
