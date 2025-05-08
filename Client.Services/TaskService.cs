using System.Net.Http.Json;
using ToDoListBlazor.Shared;

namespace ToDoListBlazor.Client.Services
{
    public class TaskService
    {
        private readonly HttpClient _http;
        private readonly string _baseUrl = "http://localhost:8090/api/tasks";

        public TaskService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<TaskItem>> GetTasks()
        {
            var response = await _http.GetAsync(_baseUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<TaskItem>>() ?? new List<TaskItem>();
        }

        public async Task<TaskItem?> GetTask(int id)
        {
            return await _http.GetFromJsonAsync<TaskItem>($"{_baseUrl}/{id}");
        }

        public async Task<bool> AddTask(TaskItem task)
        {
            var response = await _http.PostAsJsonAsync(_baseUrl, task);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateTask(TaskItem task)
        {
            var response = await _http.PutAsJsonAsync($"{_baseUrl}/{task.Id}", task);
            return response.IsSuccessStatusCode;
        }

        public async Task DeleteTask(int id)
        {
            var response = await _http.DeleteAsync($"{_baseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> SendEmail(int taskId, string recipientEmail)
        {
            var url = $"{_baseUrl}/send-email?taskId={taskId}&recipientEmail={Uri.EscapeDataString(recipientEmail)}";
            Console.WriteLine($"Отправка запроса на: {url}");
            var response = await _http.PostAsync(url, null);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Успешная отправка напоминания: {responseBody}");
                return true;
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ошибка при отправке напоминания: {response.StatusCode}, {errorBody}");
                return false;
            }
        }

        public async Task<List<InboxMessage>> CheckInboxImap()
        {
            try
            {
                var response = await _http.GetAsync($"{_baseUrl}/check-inbox-imap");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<InboxMessage>>() ?? new List<InboxMessage>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке IMAP: {ex.Message}");
                throw;
            }
        }

        public async Task<List<InboxMessage>> CheckInboxPop3()
        {
            try
            {
                var response = await _http.GetAsync($"{_baseUrl}/check-inbox-pop3");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<InboxMessage>>() ?? new List<InboxMessage>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке POP3: {ex.Message}");
                throw;
            }
        }
    }
}