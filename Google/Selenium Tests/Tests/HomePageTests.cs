using AventStack.ExtentReports;
using Google.Custom_Exceptions;
using Google.Selenium_Tests.Pages;
using Google.Test_Data_Classes;
using Google.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Google.Selenium_Tests.Tests
{
    internal class HomePageTests : BasePage
    {
        private HomePage? homePage;
        string? testName;

        [SetUp]
        public void BeforeTest()
        {
            homePage = new HomePage(Driver);
        }
        [Test]
        public void GoogleSearchTest()
        {

            testName = "Google Search Test";
            Log.Information(testName);
            Test = Extent?.CreateTest(testName);

            string? excelFilePath = currdir + "/Test Data/Google Data.xlsx";
            string? sheetName = "Search Data";

            List<SearchData> searchDataList = SearchDataRead.ReadSearchText(excelFilePath, sheetName);
            foreach (var searchData in searchDataList)
            {
                string? searchText = searchData.SearchText;
                LogTestResult(testName, "Info", Test,"Starting the Google search test");
                LogTestResult(testName, "Info", Test, "Opened Google homepage");
                try
                {
                    homePage?.SearchTest(searchText, testName, Test);
                    Test.AddScreenCaptureFromPath(TakeScreenshot(), testName);
                    // Assume the title contains the search text for simplicity
                    Assert.That(Driver.Title, Does.Contain(searchText), "Search results page title does not contain '"+searchText+"'");
                    LogTestResult(testName, "Info", Test, "Google search test completed");
                    LogTestResult(testName, "pass", Test, testName + " - Passed");
                }
                catch (Exception ex)
                {
                    if(ex is ProjectExceptions || ex is AssertionException)
                    {
                        LogTestResult(testName, "fail", Test, testName + " - Failed", ex.Message);
                    }
                    else
                    {
                        //other exceptions or handle them as needed
                    }
                }
                finally
                {
                    Driver.Navigate().GoToUrl(url);
                }
                // Additional assertions or actions if needed
            }
        }

        [TearDown]
        public void AfterTest()
        {
            Driver?.Navigate().GoToUrl(url);
        }

    }
}
