using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Birthday_Service
{
    // Birthday class
    // will validate birthdays
    public class Birthday
    {
        [JsonIgnore]
        [JsonProperty(PropertyName = "birthday")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [JsonProperty(PropertyName = "year")]
        public int Year { get; set; }

        [Required]
        [JsonProperty(PropertyName = "month")]
        public int Month { get; set; }

        [Required]
        [JsonProperty(PropertyName = "day")]
        public int Day { get; set; }

        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        // This will eventually come from the user profile
        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [Required]
        [JsonProperty(PropertyName ="firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }


        public bool Validate()
        {
            // greater than or equal to today and less than or equal to 100
            return ((this.DateOfBirth.Year <= DateTime.UtcNow.Year) && (this.DateOfBirth.Year > DateTime.UtcNow.Year - 100));
        }

    }
}
