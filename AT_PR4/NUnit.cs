using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using OpenQA.Selenium.BiDi.Modules.Network;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Linq;

namespace AT_PR4
{
    [TestFixture]
    [Parallelizable(scope: ParallelScope.All)]
    public class NUnit
    {
        private static ThreadLocal<IWebDriver> driver = new ThreadLocal<IWebDriver>();

        [SetUp]
        public void SetUp()
        {
            driver.Value = new ChromeDriver();
            driver.Value.Manage().Window.Maximize();
        }

        [TearDown]
        public void Teardown()
        {
            driver.Value.Quit();
        }

        [Test]
        [Category("Cat1")]
        [TestCase("https://en.ehu.lt/", "https://en.ehu.lt/about/", "About", "About")]
        public void VerifyNavigationToAboutPage(string url, string expectedUrl, string linkText, string expectedTitle)
        {
            driver.Value.Navigate().GoToUrl(url);

            IWebElement aboutLink = driver.Value.FindElement(By.LinkText(linkText));
            aboutLink.Click();

            ClassicAssert.AreEqual(expectedUrl, driver.Value.Url, "URL страницы не соответствует ожидаемому");

            ClassicAssert.AreEqual(expectedTitle, driver.Value.Title, "Заголовок страницы не соответствует ожидаемому");

            IWebElement header = driver.Value.FindElement(By.TagName("h1"));
            ClassicAssert.AreEqual("About", header.Text, "Заголовок контента не совпадает");
        }

        [Test]
        [Category("Cat1")]
        [TestCase("https://en.ehu.lt/")]
        public void VerifySearch(string url)
        {
            driver.Value.Navigate().GoToUrl(url);

            IWebElement searchIcon = driver.Value.FindElement(By.XPath("//div[@class='header-search']"));

            Actions actions = new Actions(driver.Value);
            actions.MoveToElement(searchIcon).Perform();

            IWebElement searchBar = driver.Value.FindElement(By.Name("s"));

            searchBar.SendKeys("study programs");
            searchBar.SendKeys(Keys.Enter);

            Assert.That(driver.Value.Url, Does.Contain("/?s=study+programs"));
        }

        [Test]
        [Category("Cat2")]
        [TestCase("https://en.ehu.lt/", "https://lt.ehu.lt/")]
        public void VerifyLanguageChange(string url, string expectedUrl)
        {
            driver.Value.Navigate().GoToUrl(url);

            IWebElement langMenu = driver.Value.FindElement(By.ClassName("language-switcher"));
            Actions actions = new Actions(driver.Value);
            actions.MoveToElement(langMenu).Perform();

            IWebElement ltLang = driver.Value.FindElement(By.XPath("//li//a[text()='lt']"));
            actions.Click(ltLang).Perform();

            ClassicAssert.AreEqual(expectedUrl, driver.Value.Url, "URL страницы не соответствует ожидаемому");
        }

        [Test]
        [Category("Cat2")]
        [TestCase("https://en.ehu.lt/contact/", new string[] { "franciskscarynacr@gmail.com", 
            "+370 68 771365", "+375 29 5781488", "Facebook Telegram VK"})]
        public void VerifyContactForm(string url, string[] contacts)
        {
            driver.Value.Navigate().GoToUrl(url);

            IWebElement elem = driver.Value.FindElement(By.XPath("//li[strong='E-mail']"));
            string a = elem.Text;
            Assert.That(contacts.Any(x => elem.Text.Contains(x)), Is.True);

            elem = driver.Value.FindElement(By.XPath("//li[strong='Phone']"));
            Assert.That(contacts.Any(x => elem.Text.Contains(x)), Is.True);

            elem = driver.Value.FindElement(By.XPath("//li[strong='Phone (BY']"));
            Assert.That(contacts.Any(x => elem.Text.Contains(x)), Is.True);

            elem = driver.Value.FindElement(By.XPath("//li[strong='Join us in the social networks:']"));
            Assert.That(contacts.Any(x => elem.Text.Contains(x)), Is.True);
        }
    }
}
