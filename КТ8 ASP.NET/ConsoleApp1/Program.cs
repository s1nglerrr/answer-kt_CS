using System.Text.Json;

namespace VkDictionaryBot
{
    class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private static Dictionary<string, string> dictionary = new Dictionary<string, string>();

        private static readonly string storageFile = "definitions.json";
        private const string VkApiVersion = "5.199";
        private const string VkAccessToken = "";
        private const long GroupId = 000000000;

        private const string MainKeyboard = @"
        {
            ""one_time"": false,
            ""buttons"": [
                [
                    {
                        ""action"": {
                            ""type"": ""text"",
                            ""label"": ""Дай определение""
                        },
                        ""color"": ""primary""
                    },
                    {
                        ""action"": {
                            ""type"": ""text"",
                            ""label"": ""Запомни определение""
                        },
                        ""color"": ""positive""
                    }
                ]
            ]
        }";

        static async Task Main()
        {
            LoadDefinitions();
            await StartLongPollBot();
        }

        static void LoadDefinitions()
        {
            if (File.Exists(storageFile))
            {
                string json = File.ReadAllText(storageFile);
                dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                             ?? new Dictionary<string, string>();
            }
        }

        static void SaveDefinitions()
        {
            string json = JsonSerializer.Serialize(dictionary);
            File.WriteAllText(storageFile, json);
        }

        static async Task StartLongPollBot()
        {
            string groupsGetLongPollServerUrl = $"https://api.vk.com/method/groups.getLongPollServer?" +
                $"access_token={VkAccessToken}&v={VkApiVersion}&group_id={GroupId}";

            Console.WriteLine(groupsGetLongPollServerUrl);

            string response = await httpClient.GetStringAsync(groupsGetLongPollServerUrl);

            Console.WriteLine(response);

            JsonDocument jsonDoc = JsonDocument.Parse(response);
            JsonElement root = jsonDoc.RootElement;

            if (root.TryGetProperty("response", out JsonElement resp))
            {
                string? server = resp.GetProperty("server").GetString();
                string? key = resp.GetProperty("key").GetString();
                string? ts = resp.GetProperty("ts").GetString();

                Console.WriteLine("Бот запущен и ожидает сообщения...");

                while (true)
                {
                    string longPollUrl = $"{server}?act=a_check&key={key}&ts={ts}&wait=25";

                    try
                    {
                        string longPollResponse = await httpClient.GetStringAsync(longPollUrl);
                        JsonDocument lpDoc = JsonDocument.Parse(longPollResponse);
                        JsonElement lpRoot = lpDoc.RootElement;

                        if (lpRoot.TryGetProperty("ts", out JsonElement newTs))
                            ts = newTs.GetString();

                        if (lpRoot.TryGetProperty("updates", out JsonElement updates))
                        {
                            foreach (JsonElement update in updates.EnumerateArray())
                            {
                                if (update.TryGetProperty("type", out JsonElement type) && type.GetString() == "message_new")
                                {
                                    JsonElement message = update.GetProperty("object").GetProperty("message");
                                    int userId = message.GetProperty("from_id").GetInt32();
                                    string? text = message.GetProperty("text").GetString();

                                    await HandleMessage(userId.ToString(), text);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка Long Poll: {ex.Message}");
                        await Task.Delay(5000);
                    }
                }
            }
        }

        static async Task HandleMessage(string userId, string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            UserSession userState = UserState.GetState(userId);

            if (message == "Дай определение" || message == "Дай определение")
            {
                userState.State = "waiting_term_get";
                userState.TempData = null;
                await SendMessage(userId, "Введите термин, определение которого хотите получить:", MainKeyboard);
            }
            else if (message == "Запомни определение" || message == "Запомни определение")
            {
                userState.State = "waiting_term_save";
                userState.TempData = null;
                await SendMessage(userId, "Введите термин, который хотите сохранить:", MainKeyboard);
            }
            else if (userState.State == "waiting_term_get")
            {
                string term = message.Trim();
                if (dictionary.ContainsKey(term))
                {
                    await SendMessage(userId, $"{term}:\n{dictionary[term]}", MainKeyboard);
                }
                else
                {
                    await SendMessage(userId, $"Ошибка: Определение для термина \"{term}\" не найдено.", MainKeyboard);
                }
                userState.State = null;
            }
            else if (userState.State == "waiting_term_save")
            {
                string term = message.Trim();
                if (dictionary.ContainsKey(term))
                {
                    await SendMessage(userId, $"Ошибка: Термин \"{term}\" уже существует в словаре.", MainKeyboard);
                    userState.State = null;
                }
                else
                {
                    userState.TempData = term;
                    userState.State = "waiting_definition_save";
                    await SendMessage(userId, $"Введите определение для термина \"{term}\":", MainKeyboard);
                }
            }
            else if (userState.State == "waiting_definition_save")
            {
                string term = userState.TempData;
                string definition = message.Trim();

                dictionary[term] = definition;
                SaveDefinitions();

                await SendMessage(userId, $"Определение для термина \"{term}\" успешно сохранено!", MainKeyboard);
                userState.State = null;
                userState.TempData = null;
            }
            else
            {
                await SendMessage(userId,
                    "Привет! Я бот-словарь.\n\n" +
                    "Я умею запоминать определения терминов и выдавать их по запросу.\n" +
                    "Используй кнопки ниже для работы со мной.",
                    MainKeyboard);
            }
        }

        static async Task SendMessage(string userId, string text, string keyboard = null)
        {
            string url = $"https://api.vk.com/method/messages.send?access_token={VkAccessToken}&v={VkApiVersion}";

            Dictionary<string, string> payload = new Dictionary<string, string>
            {
                { "user_id", userId },
                { "message", text },
                { "random_id", new Random().Next().ToString() }
            };

            if (!string.IsNullOrEmpty(keyboard))
            {
                payload.Add("keyboard", keyboard);
            }

            FormUrlEncodedContent content = new FormUrlEncodedContent(payload);
            HttpResponseMessage response = await httpClient.PostAsync(url, content);
            string responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Ответ VK API: {responseString}");
        }
    }
    static class UserState
    {
        private static Dictionary<string, UserSession> sessions = new Dictionary<string, UserSession>();

        public static UserSession GetState(string userId)
        {
            if (!sessions.ContainsKey(userId))
            {
                sessions[userId] = new UserSession();
            }
            return sessions[userId];
        }
    }

    class UserSession
    {
        public string State { get; set; }
        public string TempData { get; set; }
    }
}
