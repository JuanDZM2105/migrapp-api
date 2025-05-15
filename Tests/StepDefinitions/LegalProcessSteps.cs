using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow;
using System.Text.Json;
using System.Collections.Generic;

namespace migrapp_api.Tests.StepDefinitions
{
    [Binding]
    public class LegalProcessSteps
    {
        private readonly HttpClient _client;
        private HttpResponseMessage _response;
        private List<LegalProcess> _legalProcesses;

        private const string BaseUrl = "https://localhost:7045";

        public LegalProcessSteps()
        {
            _client = new HttpClient();
        }

        [When(@"consulto los procesos legales del usuario con Id (.*)")]
        public async Task WhenConsultoLosProcesosLegalesDelUsuarioConId(int userId)
        {
            var url = $"{BaseUrl}/Api/legalprocess/user/{userId}";
            _response = await _client.GetAsync(url);
        }

        [Then(@"la respuesta debe ser exitosa")]
        public void ThenLaRespuestaDebeSerExitosa()
        {
            _response.IsSuccessStatusCode.Should().BeTrue();
        }

        [Then(@"la respuesta debe contener una lista con objetos de proceso legal validos")]
        public async Task ThenLaRespuestaDebeContenerUnaListaConObjetosDeProcesoLegalValidos()
        {
            var json = await _response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            _legalProcesses = JsonSerializer.Deserialize<List<LegalProcess>>(json, options);

            // Validar que no esté vacío
            _legalProcesses.Should().NotBeNull();
            _legalProcesses.Should().HaveCountGreaterThan(0);

            // Validar campos de cada objeto
            foreach (var proceso in _legalProcesses)
            {
                proceso.LegalProcessId.Should().BeGreaterThan(0);
                proceso.Type.Should().NotBeNullOrWhiteSpace();
                proceso.Status.Should().NotBeNull();
                proceso.Progress.Should().BeInRange(0, 100);
                proceso.TotalProcedures.Should().BeGreaterOrEqualTo(0);
                proceso.CompletedProcedures.Should().BeGreaterOrEqualTo(0);
            }
        }
    }

    // Modelo para deserializar la respuesta
    public class LegalProcess
    {
        public int LegalProcessId { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public int Progress { get; set; }
        public int TotalProcedures { get; set; }
        public int CompletedProcedures { get; set; }
    }
}
