# console-selenium-42-net

Automação de cenários reais no [DuckDuckGo](https://duckduckgo.com) com Selenium WebDriver e MSTest, em .NET 10.

Atividade da disciplina de Garantia da Qualidade de Software — Professor Daniel Henrique Matos de Paiva.

## Estrutura da Solution

- `ConsoleSelenium` — projeto principal (Console Application).
- `ConsoleSelenium.Tests` — projeto de testes automatizados (MSTest + Selenium.WebDriver + Selenium.Support).

## Cenários automatizados

Todos os cenários estão em [`ConsoleSelenium.Tests/BuscaDesafioTest.cs`](ConsoleSelenium.Tests/BuscaDesafioTest.cs):

1. **Exercicio1_PesquisaBasica_ValidaTitulo** — digita um termo na barra de pesquisa, envia com Enter e valida que o título ou a URL da página de resultados contém o termo pesquisado.
2. **Exercicio2_Autocompletar_SugestoesDinamicas** — digita um termo sem pressionar Enter, aguarda via `WebDriverWait` a lista de sugestões de autocompletar, clica na terceira opção e valida o redirecionamento.
3. **Exercicio3_NavegacaoHistorico_VoltarEAvancar** — busca um termo, clica no primeiro resultado orgânico, valida a página externa, usa `Navigation.Back()` para retornar e valida que a página de resultados foi restaurada.

## Como executar

Pré-requisitos: .NET 10 SDK e Google Chrome instalado (o Selenium Manager baixa o driver automaticamente).

```bash
dotnet test
```
