using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TaskModel = DevOps.Frontend.Models.Task;

namespace DevOps.Frontend.Controllers
{
    public class TasksController : Controller
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        private readonly string _baseUrl;
        private readonly IHttpClientFactory _httpClientFactory;

        public TasksController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this._httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this._baseUrl = configuration.GetConnectionString("Backend");
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssignedTo,Description,Due,Id")] TaskModel task)
        {
            if (this.ModelState.IsValid)
            {
                var client = this._httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Post, $"{this._baseUrl}/api/Tasks");
                var body = JsonSerializer.Serialize(task, JsonSerializerOptions);
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                await client.SendAsync(request);

                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View(task);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var model = await this.Get(id);

            return this.View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = this._httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{this._baseUrl}/api/Tasks/{id}");
            await client.SendAsync(request);

            return this.RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var model = await this.Get(id);

            return this.View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var model = await this.Get(id);

            return this.View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssignedTo,Description,Due,Id")] TaskModel task)
        {
            if (id != task.Id)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                var client = this._httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Put, $"{this._baseUrl}/api/Tasks/{id}");
                var body = JsonSerializer.Serialize(task, JsonSerializerOptions);
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                await client.SendAsync(request);

                return this.RedirectToAction(nameof(this.Index));
            }
            return this.View(task);
        }

        public async Task<IActionResult> Index()
        {
            var client = this._httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{this._baseUrl}/api/Tasks");
            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<List<TaskModel>>(json, JsonSerializerOptions);

            return this.View(model);
        }

        private async Task<TaskModel> Get(int? id)
        {
            var client = this._httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{this._baseUrl}/api/Tasks/{id}");
            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<TaskModel>(json, JsonSerializerOptions);

            return model;
        }
    }
}