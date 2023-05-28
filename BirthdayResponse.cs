using Newtonsoft.Json;

namespace Birthday_Service
{
    public class BirthdayResponse
    {
        [JsonProperty(PropertyName="name")]
        public string Name { get; set; }


        [JsonProperty(PropertyName="birthday")]
        public string Birthday { get; set; }
    }
}
