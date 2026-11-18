using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ConsoleSelenium.Tests;

[TestClass]
public class BuscaDesafioTest
{
    private const string UrlBase = "https://duckduckgo.com";
    private IWebDriver _driver = null!;

    [TestInitialize]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        _driver = new ChromeDriver(options);
    }

    [TestCleanup]
    public void Teardown()
    {
        _driver.Quit();
    }

    [TestMethod]
    public void Exercicio1_PesquisaBasica_ValidaTitulo()
    {
        const string termo = "Selenium WebDriver C#";

        _driver.Navigate().GoToUrl(UrlBase);

        var campoPesquisa = _driver.FindElement(By.Name("q"));
        campoPesquisa.SendKeys(termo);
        campoPesquisa.SendKeys(Keys.Enter);

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(d => d.Url.Contains("q=", StringComparison.OrdinalIgnoreCase));

        Assert.IsTrue(
            _driver.Title.Contains("Selenium", StringComparison.OrdinalIgnoreCase)
            || _driver.Url.Contains("Selenium", StringComparison.OrdinalIgnoreCase),
            "O título ou a URL da página de resultados deveria conter o termo pesquisado.");
    }
}
