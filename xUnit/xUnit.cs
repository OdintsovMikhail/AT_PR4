using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.ComponentModel;
using Xunit;

namespace xUnit
{
    public class EhuTests : IDisposable
    {
        private readonly IWebDriver driver;

        public EhuTests()
        {
            var options = new ChromeOptions();
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
        }

        public void Dispose()
        {
            driver.Quit();
        }


        [Theory]
        [InlineData("https://en.ehu.lt/", "https://en.ehu.lt/about/", "About", "About")]
        public void VerifyNavigationToAboutPage(string url, string expectedUrl, string linkText, string expectedTitle)
        {
            driver.Navigate().GoToUrl(url);
            driver.FindElement(By.LinkText(linkText)).Click();

            Assert.Equal(expectedUrl, driver.Url);
            Assert.Equal(expectedTitle, driver.Title);

            var header = driver.FindElement(By.TagName("h1"));
            Assert.Equal("About", header.Text);
        }

        [Theory]
        [InlineData("https://en.ehu.lt/")]
        public void VerifySearch(string url)
        {
            driver.Navigate().GoToUrl(url);

            var searchIcon = driver.FindElement(By.XPath("//div[@class='header-search']"));
            new Actions(driver).MoveToElement(searchIcon).Perform();

            var searchBar = driver.FindElement(By.Name("s"));
            searchBar.SendKeys("study programs");
            searchBar.SendKeys(Keys.Enter);

            Assert.Contains("/?s=study+programs", driver.Url);
        }

        [Theory]
        [InlineData("https://en.ehu.lt/", "https://lt.ehu.lt/")]
        public void VerifyLanguageChange(string url, string expectedUrl)
        {
            driver.Navigate().GoToUrl(url);

            IWebElement langMenu = driver.FindElement(By.ClassName("language-switcher"));
            Actions actions = new Actions(driver);
            actions.MoveToElement(langMenu).Perform();

            IWebElement ltLang = driver.FindElement(By.XPath("//li//a[text()='lt']"));
            actions.Click(ltLang).Perform();

            Assert.Equal(expectedUrl, driver.Url);
        }

        [Theory]
        [InlineData("https://en.ehu.lt/contact/", new string[] { "franciskscarynacr@gmail.com",
            "+370 68 771365", "+375 29 5781488", "Facebook Telegram VK"})]
        public void VerifyContactForm(string url, string[] contacts)
        {
            driver.Navigate().GoToUrl(url);

            IWebElement elem = driver.FindElement(By.XPath("//li[strong='E-mail']"));
            string a = elem.Text;
            Assert.True(contacts.Any(x => elem.Text.Contains(x)));

            elem = driver.FindElement(By.XPath("//li[strong='Phone']"));
            Assert.True(contacts.Any(x => elem.Text.Contains(x)));

            elem = driver.FindElement(By.XPath("//li[strong='Phone (BY']"));
            Assert.True(contacts.Any(x => elem.Text.Contains(x)));

            elem = driver.FindElement(By.XPath("//li[strong='Join us in the social networks:']"));
            Assert.True(contacts.Any(x => elem.Text.Contains(x)));
        }
    }
}
