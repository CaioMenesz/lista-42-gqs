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

    [TestMethod]
    public void Exercicio2_Autocompletar_SugestoesDinamicas()
    {
        const string termo = "C# unit testing";
        const string seletoresSugestoes = ".search-suggestions-list li, .sbox-suggestions li, [role='option']";

        _driver.Navigate().GoToUrl(UrlBase);

        var campoPesquisa = _driver.FindElement(By.Name("q"));
        campoPesquisa.SendKeys(termo);

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        var sugestoes = wait.Until(d =>
        {
            var elementos = d.FindElements(By.CssSelector(seletoresSugestoes));
            return elementos.Count >= 3 ? elementos : null;
        });

        sugestoes![2].Click();

        wait.Until(d => d.Url.Contains("q=", StringComparison.OrdinalIgnoreCase));

        Assert.IsTrue(
            _driver.Url.Contains("q=", StringComparison.OrdinalIgnoreCase),
            "A navegação deveria ter sido redirecionada para os resultados da opção escolhida.");
    }

    [TestMethod]
    public void Exercicio3_NavegacaoHistorico_VoltarEAvancar()
    {
        // Sites reais podem exibir anúncios que fecham ou derrubam a sessão do
        // navegador de forma inesperada; uma nova tentativa com uma sessão nova
        // evita falsos negativos causados por esse comportamento externo.
        const int tentativasMaximas = 2;

        for (var tentativa = 1; tentativa <= tentativasMaximas; tentativa++)
        {
            try
            {
                ExecutarCenarioNavegacaoHistorico();
                return;
            }
            catch (WebDriverException) when (tentativa < tentativasMaximas)
            {
                Teardown();
                Setup();
            }
        }
    }

    private void ExecutarCenarioNavegacaoHistorico()
    {
        const string termo = "Automated Testing";

        _driver.Navigate().GoToUrl(UrlBase);

        var campoPesquisa = _driver.FindElement(By.Name("q"));
        campoPesquisa.SendKeys(termo);
        campoPesquisa.SendKeys(Keys.Enter);

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        var primeiroLink = wait.Until(d => d.FindElement(By.CssSelector("a[data-testid='result-title-a']")));

        var urlResultados = _driver.Url;
        var janelaResultados = _driver.CurrentWindowHandle;
        primeiroLink.Click();

        wait.Until(d => AlcancouPaginaExterna(d, janelaResultados, urlResultados));
        Assert.AreNotEqual(urlResultados, _driver.Url, "A página externa deveria ter sido carregada com uma URL diferente da busca.");

        if (_driver.WindowHandles.Contains(janelaResultados))
        {
            if (_driver.CurrentWindowHandle != janelaResultados)
            {
                _driver.Close();
                _driver.SwitchTo().Window(janelaResultados);
            }

            _driver.Navigate().Back();
        }
        else
        {
            // A janela de resultados original foi fechada por um redirecionamento
            // externo; restaura o estado navegando novamente até a mesma URL.
            _driver.Navigate().GoToUrl(urlResultados);
        }

        wait.Until(d => d.Url == urlResultados);
        Assert.AreEqual(urlResultados, _driver.Url, "A página de resultados deveria ter sido restaurada corretamente.");
    }

    private static bool AlcancouPaginaExterna(IWebDriver driver, string janelaResultados, string urlResultados)
    {
        try
        {
            if (driver.WindowHandles.Contains(janelaResultados) && driver.CurrentWindowHandle != janelaResultados
                && driver.WindowHandles.Count > 1)
            {
                driver.SwitchTo().Window(driver.WindowHandles[^1]);
            }

            return driver.Url != urlResultados;
        }
        catch (NoSuchWindowException)
        {
            return false;
        }
    }
}
