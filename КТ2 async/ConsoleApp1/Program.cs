using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var httpClient = new HttpClient();
            
            var tasks = new Task[10];
            for (int i = 0; i < 10; i++)
            {
                tasks[i] = ProcessTaskAsync(httpClient, i);
            }

            await Task.WhenAll(tasks);
            Console.WriteLine("Все задачи завершены.");
        }

        static async Task ProcessTaskAsync(HttpClient httpClient, int id)
        {
            try
            {
                var uuidResponse = await httpClient.GetStringAsync("https://httpbin.org/uuid");
                var jsonDoc = JsonDocument.Parse(uuidResponse);
                var uuid = Guid.Parse(jsonDoc.RootElement.GetProperty("uuid").GetString());
                
                var bytes = uuid.ToByteArray();
                int seed = BitConverter.ToInt16(bytes, 0);
                
                var byteTasks = new Task<byte[]>[5];
                for (int i = 0; i < 5; i++)
                {
                    int currentSeed = seed + i;
                    byteTasks[i] = httpClient.GetByteArrayAsync($"https://httpbin.org/bytes/{currentSeed}");
                }

                var results = await Task.WhenAll(byteTasks);
                
                var resultStrings = new string[5];
                for (int i = 0; i < 5; i++)
                {
                    resultStrings[i] = BitConverter.ToString(results[i]);
                }
                
                string concatenated = string.Join("-", resultStrings);
                Console.WriteLine($"Задача #{id} : {concatenated}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка в задаче #{id}: {ex.Message}");
            }
        }
    }
}
