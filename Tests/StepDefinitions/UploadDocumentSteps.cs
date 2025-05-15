using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace migrapp_api.Tests.StepDefinitions
{
    [Binding]
    public class UploadDocumentSteps
    {
        private readonly HttpClient _client;
        private HttpResponseMessage _response;

        private string _filePath;
        private const string BaseUrl = "https://localhost:7045"; // Cambia al URL base de tu API

        public UploadDocumentSteps()
        {
            _client = new HttpClient();
        }

        [Given(@"que tengo el archivo PNG ""(.*)""")]
        public void GivenQueTengoElArchivoPNG(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"El archivo no existe: {filePath}");
            }
            _filePath = filePath;
        }

        [When(@"subo el documento con Name ""(.*)"", Type ""(.*)"" y UserId (.*)")]
        public async Task WhenSuboElDocumentoConNameTypeYUserId(string name, string type, int userId)
        {
            using var content = new MultipartFormDataContent();

            var fileBytes = File.ReadAllBytes(_filePath);
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(type);

            // El nombre del campo debe ser "File" según el DTO
            content.Add(fileContent, "File", name);
            content.Add(new StringContent(name), "Name");
            content.Add(new StringContent(type), "Type");
            content.Add(new StringContent(userId.ToString()), "UserId");

            var url = $"{BaseUrl}/api/Document/upload";

            _response = await _client.PostAsync(url, content);
        }

        [Then(@"la respuesta debe indicar que el documento fue subido con exito")]
        public async Task ThenLaRespuestaDebeIndicarQueElDocumentoFueSubidoConExito()
        {
            _response.EnsureSuccessStatusCode();

            var responseString = await _response.Content.ReadAsStringAsync();
            responseString.Should().Contain("Documento subido con éxito");
        }


        [Given(@"que tengo un archivo con extension ""(.*)"" en la ruta ""(.*)""")]
        public void GivenQueTengoUnArchivoConExtensionEnLaRuta(string extension, string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"El archivo no existe: {filePath}");
            }

            // Opcional: puedes validar que el archivo tiene la extensión indicada
            if (Path.GetExtension(filePath)?.ToLower() != extension.ToLower())
            {
                throw new ArgumentException($"El archivo no tiene la extension esperada: {extension}");
            }

            _filePath = filePath;
        }

        [When(@"intento subir el documento con Name ""(.*)"", Type ""(.*)"" y UserId (.*)")]
        public async Task WhenIntentoSubirElDocumentoConNameTypeYUserId(string name, string type, int userId)
        {
            using var content = new MultipartFormDataContent();

            var fileBytes = File.ReadAllBytes(_filePath);
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse(type);

            content.Add(fileContent, "File", name);
            content.Add(new StringContent(name), "Name");
            content.Add(new StringContent(type), "Type");
            content.Add(new StringContent(userId.ToString()), "UserId");

            var url = $"{BaseUrl}/api/Document/upload";

            _response = await _client.PostAsync(url, content);
        }

        [Then(@"la respuesta debe indicar que la extension del archivo no esta permitida")]
        public async Task ThenLaRespuestaDebeIndicarQueLaExtensionDelArchivoNoEstaPermitida()
        {
            // La API debería responder con un error, por ejemplo BadRequest (400)
            _response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

            var responseString = await _response.Content.ReadAsStringAsync();
            responseString.Should().Contain("La extensión del archivo no está permitida.");
        }

    }
}

