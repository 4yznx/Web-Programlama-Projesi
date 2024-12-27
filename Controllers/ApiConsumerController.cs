using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BarberShop.Models;
using Newtonsoft.Json;
using System.Text;

namespace BarberShop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ApiConsumerController : Controller
    {
        private readonly HttpClient _httpClient;

        public ApiConsumerController()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7085/api/IslemApi/")
            };
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var islemList = JsonConvert.DeserializeObject<List<Islem>>(jsonData);
                return View(islemList);
            }
            return View(new List<Islem>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Islem islem)
        {
            if (ModelState.IsValid)
            {
                var jsonData = JsonConvert.SerializeObject(islem);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(islem);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync(id.ToString());
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var islem = JsonConvert.DeserializeObject<Islem>(jsonData);
                return View(islem);
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Islem islem)
        {
            if (id != islem.IslemID || !ModelState.IsValid)
            {
                return View(islem);
            }

            var jsonData = JsonConvert.SerializeObject(islem);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(id.ToString(), content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(islem);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync(id.ToString());
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "An error occurred while deleting the record.");
            return View("Index");
        }
    }
}