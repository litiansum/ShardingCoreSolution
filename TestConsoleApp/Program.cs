// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using static System.Net.WebRequestMethods;
var httpClient = new HttpClient();

try
{
    string url = "http://localhost:5240/Test";

    List<string> request = new List<string>()
{
    "9eeb44437ab9420e9018f390a987b432",
    "150502308059869184",
    "150504739355291648",
    "160923754242306048"
};
    int count = int.Parse(Console.ReadLine() ?? "0");
    while (request.Count < count)
    {
        request.Add(Guid.NewGuid().ToString("n"));
    }

    try
    {

        var response = await httpClient.PostAsJsonAsync(url, request);

        Console.WriteLine($"Thread:{Thread.CurrentThread.ManagedThreadId}:" + response.IsSuccessStatusCode);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

    Console.ReadKey();

}
catch (Exception ex)
{

    Console.WriteLine(ex.Message);
}