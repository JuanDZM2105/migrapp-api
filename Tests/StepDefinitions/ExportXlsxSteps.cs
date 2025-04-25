using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;
using FluentAssertions;
using System.Threading;
using System.IO;
using OfficeOpenXml;

namespace migrapp_api.Tests.StepDefinitions
{
    [Binding]
    public class ExportXlsxSteps
    {
        private IWebDriver _driver;

        [BeforeScenario]
        public void Setup()
        {
            Console.WriteLine("Iniciando el navegador...");
            _driver = new ChromeDriver();
            _driver.Manage().Window.Maximize();
            Console.WriteLine("Navegador iniciado");
        }

        [Given(@"que el archivo XLSX fue generado exitosamente")]
        public void DadoQueElArchivoXlsxFueGeneradoExitosamente()
        {
            var downloadPath = @"C:\Users\Admin\Downloads"; // Ruta de descarga, ajústala si es necesario
            var xlsxFile = Path.Combine(downloadPath, "reporte.xlsx");

            if (!File.Exists(xlsxFile))
            {
                Console.WriteLine($"El archivo no existe: {xlsxFile}");
            }
            // Verifica que el archivo XLSX haya sido generado
            File.Exists(xlsxFile).Should().BeTrue();
        }

        [Then(@"debe contener encabezados claros y los datos organizados por columnas")]
        public void EntoncesDebeContenerEncabezadosClarosYLosDatosOrganizadosPorColumnas()
        {
            var downloadPath = @"C:\Users\Admin\Downloads"; // Ajusta la ruta si es necesario
            var xlsxFile = Path.Combine(downloadPath, "reporte.xlsx");

            // Abrir el archivo XLSX
            using (var package = new ExcelPackage(new FileInfo(xlsxFile)))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Obtenemos la primera hoja

                // Verificar los encabezados (suponiendo que están en la primera fila)
                var header1 = worksheet.Cells[1, 1].Text; // Celda A1
                var header2 = worksheet.Cells[1, 2].Text; // Celda B1
                var header3 = worksheet.Cells[1, 3].Text; // Celda C1
                var header4 = worksheet.Cells[1, 4].Text; // Celda D1
                var header5 = worksheet.Cells[1, 5].Text; // Celda E1

                header1.Should().Be("Name");
                header2.Should().Be("Email");
                header3.Should().Be("Country");
                header4.Should().Be("Phone");
                header5.Should().Be("Account Status");

                // Verificar que haya más de una fila (para asegurarse que hay datos)
                worksheet.Dimension.Rows.Should().BeGreaterThan(1);
            }
        }

        [Then(@"debe poder abrirse correctamente en Excel u otro lector de hojas de calculo")]
        public void EntoncesDebePoderAbrirseCorrectamenteEnExcelUOtroLectorDeHojasDeCalculo()
        {
            var downloadPath = @"C:\Users\Admin\Downloads"; // Ajusta la ruta si es necesario
            var xlsxFile = Path.Combine(downloadPath, "reporte.xlsx");

            if (!File.Exists(xlsxFile))
            {
                throw new FileNotFoundException("El archivo XLSX no fue encontrado en la ruta especificada.", xlsxFile);
            }

            // Intentar abrir el archivo XLSX
            var process = new System.Diagnostics.Process();
            process.StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = xlsxFile,
                UseShellExecute = true
            };
            process.Start();

            // Esperar un momento para verificar que se abrió correctamente
            Thread.Sleep(10000);

            process.HasExited.Should().BeTrue(); // Verifica que el archivo se abrió correctamente en Excel
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _driver.Quit();
        }
    }
}
