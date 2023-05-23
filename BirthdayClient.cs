using Microsoft.Azure.Cosmos;

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
            PartitionKey pk = new ("/"+birthday.UserId);
            var result = await this._container.CreateItemAsync<Birthday>(birthday).ConfigureAwait(false);
            return result;
        }
    }
}