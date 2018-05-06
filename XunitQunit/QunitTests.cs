using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace XunitQunit
{
    public class QunitTests
    {
        private readonly IConfiguration _configuration;
        private readonly ITestOutputHelper _output;

        public QunitTests(ITestOutputHelper output)
        {
            _output = output;
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json");
            _configuration = builder.Build();
        }

        [Theory]
        [InlineData(Browser.Firefox)]
        [InlineData(Browser.Chrome)]
        public void Tests(Browser browser)
        {
            //Arrange
            var qunit = new QunitTester(_configuration, browser);
            var basePath = _configuration.GetSection("Selenium:TestUrl").Value;
            var tests = Directory.GetFiles(basePath, "*.html");
            var results = new List<PageQunitResult>();

            //Act
            foreach (var test in tests)
            {
                //var url = _configuration.GetSection("Selenium:TestUrl").Value + test;
                //    "TestUrl": "file:///C:/Dev/PocXunitQunit/QunitTests/"
                var url = "file:///" + test.Replace("\\", "/");

                _output.WriteLine("Starting to load: " + url);
                qunit.LoadQunitTest(url);
                var result = qunit.GetResults();
                Assert.NotNull(result);
                results.Add(new PageQunitResult
                {
                    PageName = test,
                    Failed = result.Failed,
                    Passed = result.Passed,
                    Runtime = result.Runtime,
                    Total = result.Total
                });
            }

            //Assert
            foreach (var result in results)
                _output.WriteLine(
                    $"{result.PageName}: Passed: {result.Passed} Failed: {result.Failed} Total: {result.Total} Runtime: {result.Runtime}");

            qunit.Dispose();
        }

    }
}