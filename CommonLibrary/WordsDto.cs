using Newtonsoft.Json;

namespace CommonLibrary
{
    public class WordsDto
    {
        [JsonProperty("wordsCount")]
        public Dictionary<string, int> wordsCount { get; set; } = default!;
    }
}
