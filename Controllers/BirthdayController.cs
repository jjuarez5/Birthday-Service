using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace Birthday_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BirthdayController : ControllerBase
    {
        private readonly IBirthdayClient _birthdayClient;
        // cosmos client
        // dummy cosmos client
        public BirthdayController( IBirthdayClient birthdayClient )
        {
            _birthdayClient = birthdayClient;
        }

        [HttpPost]
        public async Task<IActionResult> PostBirthday(Birthday payload )
        {
            Birthday birthday = new();

            try
            {
                birthday.Year = payload.Year;
                birthday.Month = payload.Month;
                birthday.Day = payload.Day;
                birthday.Id = Guid.NewGuid().ToString();
                birthday.UserId = "JJUser";
            }
            catch (Exception ex)
            {

                return new BadRequestObjectResult(ex);
            }

            birthday.DateOfBirth = new DateTime(birthday.Year, birthday.Month, birthday.Day);

            var result = birthday.Validate();
            string spartitionKey = "1";

            if (result)
            {
                try
                {
                    await _birthdayClient.PostBirthday(birthday).ConfigureAwait(false);
                }
                catch (CosmosException ex)
                {

                    return new BadRequestObjectResult(ex);
                }
                catch (Exception ex)
                {
                    throw;
                }


            }

            return result != false
                ? (ActionResult)new OkObjectResult(birthday.DateOfBirth.ToString("dd/MM/yyyy"))
                : new BadRequestObjectResult("Please pass a valid birthday in the request body");
        }
    }
}
