using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace PetstoreService
{
    public class PetRepository
    {
        const string BaseUrl = "https://petstore.swagger.io/v2";
        private readonly HttpClient _client;

        public PetRepository(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> CreatePetAsync(Pet pet)
        {
            return await _client.PostAsJsonAsync($"{BaseUrl}/pet", pet);
        }
        public async Task<HttpResponseMessage> UpdateAsync(Pet pet)
        {
            return await _client.PutAsJsonAsync($"{BaseUrl}/pet", pet);
        }
        public async Task<HttpResponseMessage> UpdateWithFormAsync(long id, string name, string status = null)
        {
            Dictionary<string, string> formData = new Dictionary<string, string> { { "name", name }, { "status", status } };
            FormUrlEncodedContent content = new FormUrlEncodedContent(formData); // не уверена, в каком формате вставлять в запрос
            return await _client.PostAsync($"{BaseUrl}/pet/{id}", content);
        }
        public async Task<HttpResponseMessage> GetByIdAsync(long id)
        {
            return await _client.GetAsync($"{BaseUrl}/pet/{id}");
        }
        public async Task<HttpResponseMessage> GetByStatusAsync(string status)
        {
            return await _client.GetAsync($"{BaseUrl}/pet/findByStatus?status={status}");
        }

        public async Task<HttpResponseMessage> DeleteAsync(long id)
        {
            return await _client.DeleteAsync($"{BaseUrl}/pet/{id}");
        }

        public async Task<HttpResponseMessage> UploadImageAsync(long id, byte[] fileBytes, string fileName) // не поняла, как сделать :(
        {
            using var content = new MultipartFormDataContent();
            ByteArrayContent fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
            content.Add(fileContent, "file", fileName);
            return await _client.PostAsync($"{BaseUrl}/pet/{id}/uploadImage", content);
        }

        public async Task<HttpResponseMessage> GetUploadImageAsync(long id) // для негативного теста
        {
            return await _client.GetAsync($"{BaseUrl}/pet/{id}/uploadImage");
        }
    }
}
