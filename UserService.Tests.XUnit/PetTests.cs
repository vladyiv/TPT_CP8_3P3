using System.Net;
using System.Net.Http.Json;

namespace PetstoreService
{
    public class PetTests
    {
        private readonly HttpClient _httpClient;
        private readonly PetRepository _service;

        public PetTests()
        {
            _httpClient = new HttpClient();
            _service = new PetRepository(_httpClient);
        }

        private static Pet CreateTestPet()
        {
            string name = $"dog_{Random.Shared.Next(1, int.MaxValue)}";
            return new Pet()
            {
                Id = Random.Shared.Next(0, int.MaxValue),
                Category = new Category(1, "dog"),
                Name = name,
                PhotoUrls = [],
                Tags = [new Tag(1, "tag1")],
                Status = "available"
            };
        }

        // не знаю, ставить ли настройки перед и после yield (создание и автоматическое удаление)

        [Fact]
        public async Task PostImagePet_Pos()
        {
            Pet pet = CreateTestPet();
            string filePath = "../../../../dog_image.jpg";
            byte[] imageBytes = await File.ReadAllBytesAsync(filePath);
            string fileName = Path.GetFileName(filePath);
            HttpResponseMessage response = await _service.UploadImageAsync(pet.Id, imageBytes, fileName);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostImagePet_Neg()
        {
            Pet pet = CreateTestPet();
            HttpResponseMessage response = await _service.GetUploadImageAsync(pet.Id);
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }
        [Fact]
        public async Task PostPet_Pos()
        {
            Pet pet = CreateTestPet();
            HttpResponseMessage response = await _service.CreatePetAsync(pet);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            HttpResponseMessage getResponse = await _service.GetByIdAsync(pet.Id);
            Pet fetchedPet = await getResponse.Content.ReadFromJsonAsync<Pet>();
            Assert.Equal(pet.Name, fetchedPet.Name);
        }

        [Fact]
        public async Task PostPet_Neg()
        {
            HttpResponseMessage response = await _service.CreatePetAsync(null);
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Fact]
        public async Task PutPet_Pos()
        {
            Pet pet = CreateTestPet();
            await _service.CreatePetAsync(pet);
            pet.Name = "Not Ranmaru";
            HttpResponseMessage response = await _service.UpdateAsync(pet);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            HttpResponseMessage getResponse = await _service.GetByIdAsync(pet.Id);
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            Pet newPet = await getResponse.Content.ReadFromJsonAsync<Pet>();
            Assert.Equal("Not Ranmaru", newPet.Name);
        }

        [Fact]
        public async Task PutPet_Neg()
        {
            HttpResponseMessage response = await _service.UpdateAsync(null);
            Assert.Equal(response.StatusCode, HttpStatusCode.MethodNotAllowed);
        }

        [Fact]
        public async Task GetByStatusPet_Pos()
        {
            Pet pet = CreateTestPet();
            HttpResponseMessage response = await _service.GetByStatusAsync(pet.Status);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            List<Pet> pets = await response.Content.ReadFromJsonAsync<List<Pet>>();
            Assert.NotEmpty(pets);
        }

        [Fact]
        public async Task GetByStatusPet_Neg()
        {
            HttpResponseMessage response = await _service.GetByStatusAsync("new_status");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            List<Pet> pets = await response.Content.ReadFromJsonAsync<List<Pet>>();
            Assert.Empty(pets);
        }

        [Fact]
        public async Task GetByIdPet_Pos()
        {
            Pet pet = CreateTestPet();
            await _service.CreatePetAsync(pet);
            HttpResponseMessage response = await _service.GetByIdAsync(pet.Id);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Pet petJson = await response.Content.ReadFromJsonAsync<Pet>();
            Assert.Equal(pet.Id, petJson.Id);
            Assert.Equal(pet.Name, petJson.Name);
        }

        [Fact]
        public async Task GetByIdPet_Neg()
        {
            int randomId = Random.Shared.Next(1, int.MaxValue);
            HttpResponseMessage response = await _service.GetByIdAsync(randomId);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PutFormPet_Pos()
        {
            Pet pet = CreateTestPet();
            await _service.CreatePetAsync(pet);
            HttpResponseMessage response = await _service.UpdateWithFormAsync(pet.Id, "Not Ranmaru", "sold");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            HttpResponseMessage getResponse = await _service.GetByIdAsync(pet.Id);
            Pet updatedPet = await getResponse.Content.ReadFromJsonAsync<Pet>();
            Assert.Equal("Not Ranmaru", updatedPet?.Name);
            Assert.Equal("sold", updatedPet?.Status);
        }

        [Fact]
        public async Task PutFormPet_Neg()
        {
            long randomId = Random.Shared.Next(1, int.MaxValue);
            HttpResponseMessage response = await _service.UpdateWithFormAsync(randomId, "New");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeletePet_Pos()
        {
            Pet pet = CreateTestPet();
            long id = pet.Id;
            await _service.CreatePetAsync(pet);
            HttpResponseMessage response = await _service.DeleteAsync(id);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            HttpResponseMessage getResponse = await _service.GetByIdAsync(id);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeletePet_Neg()        {
            int randomId = Random.Shared.Next(1, int.MaxValue);
            HttpResponseMessage response = await _service.DeleteAsync(randomId);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
