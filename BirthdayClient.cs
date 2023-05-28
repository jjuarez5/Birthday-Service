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

        public async Task<Birthday> PostBirthday( Birthday birthday )
        {
            var result = await this._container.CreateItemAsync<Birthday>(birthday).ConfigureAwait(false);
            return result;
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
                FeedIterator<BirthdayResponse> iterator =
                    this._container.GetItemQueryIterator<BirthdayResponse>(
                        def,
                        continuationToken: contToken,
                        requestOptions: options);
                while (iterator.HasMoreResults)
                {
                    FeedResponse<BirthdayResponse> response = await iterator.ReadNextAsync().ConfigureAwait(false);
                    foreach (BirthdayResponse birthday in response)
                    {
                        birthdays.Add(birthday);
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


            //using (FeedIterator<Birthday> iterator = this._container.GetItemLinqQueryable<Birthday>()
            //    .Where(x => x.UserId == userId).ToFeedIterator())
            //{
            //    if (iterator.HasMoreResults)
            //    {

            //    }
            //}

                if (string.IsNullOrEmpty(result))
                {
                    return false;
                }

            return true;
        }
    }
}