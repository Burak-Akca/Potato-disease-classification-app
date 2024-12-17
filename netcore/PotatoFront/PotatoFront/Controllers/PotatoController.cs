using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;


namespace PotatoFront.Controllers
{
    public class PotatoController : Controller
    {
        
        private readonly HttpClient _httpClient;

        public PotatoController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

       
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetStringAsync("http://127.0.0.1:8000/");
            ViewBag.ApiData = response;
            return View();
        }  
        
        
        
        public async Task<IActionResult> UploadAndPredict()
        {
            
            return View();
        }




        [HttpPost]
        public async Task<IActionResult> UploadAndPredict(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var formContent = new MultipartFormDataContent();
                var fileContent = new StreamContent(file.OpenReadStream());

                // Resim dosyasının içerik tipini belirliyoruz
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                formContent.Add(fileContent, "file", file.FileName); // 'file' parametre adı FastAPI'deki parametre ile uyumlu olmalı

                // FastAPI'den tahmin isteği gönderme
                var response = await _httpClient.PostAsync("http://127.0.0.1:8000/predict/", formContent);

                if (response.IsSuccessStatusCode)
                {
                    // FastAPI'den gelen JSON yanıtını al
                    var result = await response.Content.ReadAsStringAsync();
                    var predictionResult = JObject.Parse(result);

                    // Tahmin sınıfı ve güven skoru
                    ViewBag.PredictedClass = predictionResult["class"];  // "predicted_class" yerine "class"
                    ViewBag.ConfidenceScore = predictionResult["confidence"];
                }
                else
                {
                    ViewBag.Error = "Predict request failed!";
                }
            }

            return View("UploadAndPredict");
        }

    }
}
