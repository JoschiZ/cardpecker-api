
using Cadpekcer.Api.LoadTester;



using var client = new HttpClient();



var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(10));
while (await timer.WaitForNextTickAsync().ConfigureAwait(false))
{
        var randomIndex = Random.Shared.Next(Bulk.Cards.Length);
        var id =  Bulk.Cards[randomIndex];

        await client.GetAsync($"http://localhost:8002/mtg/cards/{id}/prices").ConfigureAwait(false);
}



