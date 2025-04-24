using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;
using FluentAssertions;

namespace migrapp_api.Tests.StepDefinitions
{

    public class LoginUserSteps
    {
        private IWebDriver _driver;

        [Given(@"el usuario abre la página de login")]
        public void UsuarioAbreLaPaginaDeLogin()
        {
            _driver = new ChromeDriver();
            _driver.Navigate().GoToUrl("http://localhost:4200/login");
            _driver.Manage().Window.Maximize();
        }

        [When(@"ingresa el email ""(.*)"" y la contraseña ""(.*)""")]
        public void IngresaCredenciales(string email, string password)
        {
            _driver.FindElement(By.Name("email")).SendKeys(email);
            _driver.FindElement(By.Name("password")).SendKeys(password);
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        }

        [When(@"se le solicita un código OTP")]
        public void SeSolicitaOtp()
        {
            Thread.Sleep(1000);
        }

        [When(@"el usuario ingresa el código ""(.*)""")]
        public void UsuarioIngresaOtp(string otp)
        {
            _driver.FindElement(By.Name("otp")).SendKeys(otp);
            _driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        }

        [Then(@"debería ver el mensaje ""(.*)""")]
        public void DeberiaVerElMensaje(string esperado)
        {
            Thread.Sleep(1000);
            var mensaje = _driver.FindElement(By.Id("mensaje")).Text;
            mensaje.Should().Be(esperado);
            _driver.Quit();
        }
    }
}
