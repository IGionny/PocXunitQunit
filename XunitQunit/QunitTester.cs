using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;

namespace XunitQunit
{
    public class QunitTester : IDisposable
    {
        private IWebDriver _driver;

        public QunitTester(
            IConfiguration configuration, Browser browser)
        {
            string driverPath = null;
            if (browser == Browser.Firefox)
                driverPath =
                    configuration.GetSection("Selenium:FirefoxDriverPath").Value;
            else if (browser == Browser.Chrome)
                driverPath =
                    configuration.GetSection("Selenium:ChromeDriverPath").Value;

            _driver = WebDriverFactory.CreateWebDriver(
                browser, driverPath, true);
        }


        public void Dispose()
        {
            _driver?.Dispose();
            _driver = null;
        }

        public void LoadQunitTest(string url)
        {
            _driver.LoadPage(TimeSpan.FromSeconds(2), url);
        }

        public QunitResult GetResults()
        {
            var jse = (IJavaScriptExecutor) _driver;
            object r = null;
            var secureCount = 0;
            while (r == null && secureCount < 100)
            {
                r = jse.ExecuteScript("return window.resultQunit");
                if (r == null) Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }

            if (r == null) return null;

            var dict = r as Dictionary<string, object>;
            var result = new QunitResult();
            result.Failed = (int) (long) dict["failed"];
            result.Passed = (int) (long) dict["passed"];
            result.Runtime = (int) (long) dict["runtime"];
            result.Total = (int) (long) dict["total"];
            return result;
        }
    }
}