﻿using Microsoft.AspNetCore.Http;
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

            var result = payload.Validate();
            BirthdayResponse bday = new();

            if (result)
            {
                try
                {
                    bday = await _birthdayClient.PostBirthday(payload).ConfigureAwait(false);
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
                ? (ActionResult)new OkObjectResult(bday)
                : new BadRequestObjectResult("Please pass a valid birthday in the request body");
        }

        [HttpGet]
        public async Task<ActionResult> GetBirthday([FromQuery] string userId )
        {
            // check if userId in db
            bool isValid = false;
            try
            {
                isValid = await _birthdayClient.ValidateUserId(userId);
            }
            catch (Exception)
            {

                throw;
            }


            if (isValid)
            {
                var result = await _birthdayClient.GetBirthdays(userId).ConfigureAwait(false);

                return result.Count > 0
                    ? (ActionResult)new OkObjectResult(result)
                    : new BadRequestResult();

            }

            return NotFound();

        }
    }
}
