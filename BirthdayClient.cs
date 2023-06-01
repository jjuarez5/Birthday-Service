using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Text;

namespace Birthday_Service

{
    public class BirthdayClient : IBirthdayClient
    {
        private Container _container;

        public BirthdayClient(
            CosmosClient dbClient,
            string databaseName,
            string containerName )
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<BirthdayResponse> PostBirthday( Birthday payload )
        {
            Birthday birthday = new Birthday();
            BirthdayResponse response = new BirthdayResponse();

            // move to client
            try
            {
                birthday.Year = payload.Year;
                birthday.Month = payload.Month;
                birthday.Day = payload.Day;
                birthday.Id = Guid.NewGuid().ToString();
                birthday.UserId = "JJUser";
                birthday.Name = payload.Name;
            }
            catch (Exception)
            {
                throw;
            }

            birthday.DateOfBirth = payload.DateOfBirth;
            var result = await this._container.CreateItemAsync<Birthday>(birthday).ConfigureAwait(false);

            if (result.StatusCode == System.Net.HttpStatusCode.Created)
            {
                response = new()
                {
                    Birthday = birthday.DateOfBirth.ToString("dd/MM/yyyy"),
                    Name = birthday.Name
                };
            }

            
            return response;
        }

        public async Task<List<BirthdayResponse>> GetBirthdays( string userId )
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(userId);
            }

            StringBuilder queryString = new StringBuilder("SELECT c.name, c.birthday FROM " + this._container.Id + " c " + "WHERE c.userId = @userId");
            QueryRequestOptions options = new()
            {
                PartitionKey = new PartitionKey(userId),
            };

            QueryDefinition def = new QueryDefinition(queryString.ToString())
                                                    .WithParameter("@userId", userId);

            string contToken = null;
            List<BirthdayResponse> birthdays = new();

            do
            {
                FeedIterator<Birthday> iterator =
                    this._container.GetItemQueryIterator<Birthday>(
                        def,
                        continuationToken: contToken,
                        requestOptions: options);
                while (iterator.HasMoreResults)
                {
                    FeedResponse<Birthday> response = await iterator.ReadNextAsync().ConfigureAwait(false);
                    foreach (Birthday birthday in response)
                    {
                        BirthdayResponse bdayResponse = new()
                        {
                            Birthday = birthday.DateOfBirth.ToString("dd/MM/yyyy"),
                            Name = birthday.Name,
                        };
                        birthdays.Add(bdayResponse);
                    }
                }
            } while (contToken != null);

            return birthdays;


        }

        public async Task<bool> ValidateUserId( string userId )
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(userId);
            }

            string? result = this._container.GetItemLinqQueryable<Birthday>(true)
                                            .Where(x => x.UserId == userId)
                                            .AsEnumerable()
                                            .FirstOrDefault().UserId;

                if (string.IsNullOrEmpty(result))
                {
                    return false;
                }

            return true;
        }
    }
}