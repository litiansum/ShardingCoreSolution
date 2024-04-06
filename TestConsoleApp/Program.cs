// See https://aka.ms/new-console-template for more information

using static System.Net.WebRequestMethods;
var httpClient = new HttpClient();

string url = "http://localhost:5000/Test?referenceId=9eeb44437ab9420e9018f390a987b432";

List<Task> tasks = new List<Task>();
var count = Environment.ProcessorCount * 5;
for (int i = 0; i < count; i++)
{
    var task = Task.Run(async () =>
    {
        while (true)
        {
            var response = await httpClient.GetAsync(url);

            Console.WriteLine($"Thread:{Thread.CurrentThread.ManagedThreadId}:" + response.IsSuccessStatusCode);
        }
    });

    tasks.Add(task);
}

await Task.WhenAll(tasks);

Console.ReadKey();
