using System.Net;
using System.Net.Http.Json;
namespace PetstoreService
{
    public class PetTests
    {
        private HttpClient _httpClient;
        private PetRepository _service;

        [SetUp]
        public void Setup()
        {
            _httpClient = new HttpClient();
            _service = new PetRepository(_httpClient);
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
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

        [Test]
        public async Task PostImagePet_Pos()
        {
            Pet pet = CreateTestPet();
            string filePath = "../../../../dog_image.jpg";
            byte[] imageBytes = await File.ReadAllBytesAsync(filePath);
            string fileName = Path.GetFileName(filePath);

            HttpResponseMessage response = await _service.UploadImageAsync(pet.Id, imageBytes, fileName);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task PostImagePet_Neg()
        {
            Pet pet = CreateTestPet();
            HttpResponseMessage response = await _service.GetUploadImageAsync(pet.Id);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
        }

        [Test]
        public async Task PostPet_Pos()
        {
            Pet pet = CreateTestPet();

            HttpResponseMessage response = await _service.CreatePetAsync(pet);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            HttpResponseMessage getResponse = await _service.GetByIdAsync(pet.Id);
            Pet? fetchedPet = await getResponse.Content.ReadFromJsonAsync<Pet>();

            Assert.That(fetchedPet?.Name, Is.EqualTo(pet.Name));

            await _service.DeleteAsync(pet.Id);
        }

        [Test]
        public async Task PostPet_Neg()
        {
            HttpResponseMessage response = await _service.CreatePetAsync(null!);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
        }

        [Test]
        public async Task PutPet_Pos()
        {
            Pet pet = CreateTestPet();
            await _service.CreatePetAsync(pet);

            pet.Name = "Not Ranmaru";
            HttpResponseMessage response = await _service.UpdateAsync(pet);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            HttpResponseMessage getResponse = await _service.GetByIdAsync(pet.Id);
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Pet? newPet = await getResponse.Content.ReadFromJsonAsync<Pet>();

            Assert.That(newPet?.Name, Is.EqualTo("Not Ranmaru"));
        }

        [Test]
        public async Task PutPet_Neg()
        {
            HttpResponseMessage response = await _service.UpdateAsync(null!);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
        }

        [Test]
        public async Task GetByStatusPet_Pos()
        {
            Pet pet = CreateTestPet();

            HttpResponseMessage response = await _service.GetByStatusAsync(pet.Status);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            List<Pet>? pets = await response.Content.ReadFromJsonAsync<List<Pet>>();
            Assert.That(pets, Is.Not.Null);
            Assert.That(pets, Is.Not.Empty);
        }

        [Test]
        public async Task GetByStatusPet_Neg()
        {
            HttpResponseMessage response = await _service.GetByStatusAsync("new_status");
            List<Pet>? pets = await response.Content.ReadFromJsonAsync<List<Pet>>();

            Assert.That(pets, Is.Not.Null);
            Assert.That(pets, Is.Empty);
        }

        [Test]
        public async Task GetByIdPet_Pos()
        {
            Pet pet = CreateTestPet();
            await _service.CreatePetAsync(pet);

            HttpResponseMessage response = await _service.GetByIdAsync(pet.Id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Pet? fetchedPet = await response.Content.ReadFromJsonAsync<Pet>();
            Assert.That(fetchedPet?.Id, Is.EqualTo(pet.Id));
            Assert.That(fetchedPet?.Name, Is.EqualTo(pet.Name));
        }

        [Test]
        public async Task GetByIdPet_Neg()
        {
            int randomId = Random.Shared.Next(1, int.MaxValue);
            HttpResponseMessage response = await _service.GetByIdAsync(randomId);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task PutFormPet_Pos()
        {
            Pet pet = CreateTestPet();

            await _service.CreatePetAsync(pet);
            HttpResponseMessage response = await _service.UpdateWithFormAsync(pet.Id, "Not Ranmaru", "sold");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            HttpResponseMessage getResponse = await _service.GetByIdAsync(pet.Id);
            Pet? updatedPet = await getResponse.Content.ReadFromJsonAsync<Pet>();

            Assert.That(updatedPet?.Name, Is.EqualTo("Not Ranmaru"));
            Assert.That(updatedPet?.Status, Is.EqualTo("sold"));
        }

        [Test]
        public async Task PutFormPet_Neg()
        {
            int randomId = Random.Shared.Next(1, int.MaxValue);
            HttpResponseMessage response = await _service.UpdateWithFormAsync(randomId, "New");

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeletePet_Pos()
        {
            Pet pet = CreateTestPet();
            long id = pet.Id;
            await _service.CreatePetAsync(pet);

            HttpResponseMessage response = await _service.DeleteAsync(id);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            HttpResponseMessage getResponse = await _service.GetByIdAsync(id);
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeletePet_Neg()
        {
            int randomId = Random.Shared.Next(1, int.MaxValue);
            HttpResponseMessage response = await _service.DeleteAsync(randomId);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }

}
