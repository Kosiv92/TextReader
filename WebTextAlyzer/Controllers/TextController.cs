using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TextAnalyzerLib;

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

        [HttpPost]
        public HttpResponseObject GetUniqueWords([FromBody] HttpRequestObject requestObject)
        {
            //var rm = JsonSerializer.Deserialize<HttpRequestMessage>(message);
            //var stringContent = rm.Content;
            //string content = stringContent.ReadAsStringAsync().Result;
            //var request = JsonSerializer.Deserialize<HttpRequestObject>(content);

            var dictionary = _fileHandler.CountUniqueWordsPL(requestObject.TextStrings);
            var response = new HttpResponseObject();
            response.WordsCountDto = dictionary;
            return response;
        }

        [HttpGet]
        public HttpResponseObject IsAvailable()
        {
            var ok = new HttpResponseObject();
            ok.WordsCountDto.Add("hello", 2);
            ok.WordsCountDto.Add("bye", 5);


            return ok;


        }


    }

    
    public class HttpRequestObject
    {
        public string[] TextStrings { get; set; }
    }

    
    public class HttpResponseObject
    {
        public HttpResponseObject()
        {
            WordsCountDto = new Dictionary<string, int>();
        }
        public Dictionary<string, int> WordsCountDto { get; set; }
    }
}
