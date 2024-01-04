using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Google.Selenium_Tests.Pages
{
    public class BasePage
    {
        protected IWebDriver? Driver;

        protected ExtentReports? Extent;
        private ExtentSparkReporter? SparkReporter;
        protected ExtentTest? Test;

        private Dictionary<string, string>? Properties;
        protected string? currdir;
        protected string? url;

        //overloaded constructors
        protected BasePage()
        {
            currdir = Directory.GetParent(@"../../../")?.FullName; //getting the working directory
        }
        public BasePage(IWebDriver driver)
        {
            Driver = driver;
        }
        private void ReadConfigSettings()
        {
            Properties = new Dictionary<string, string>();               //declaring the dictionary
            string filename = currdir + "/ConfigSettings/config.properties"; //taking the file from working directory
            string[] lines = File.ReadAllLines(filename);

            foreach (string line in lines)                               //for geting the file data even if there are whitespaces
            {
                if (!string.IsNullOrWhiteSpace(line) && line.Contains("="))
                {
                    string[] parts = line.Split('=');
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    Properties[key] = value;
                }
            }
        }
        protected static void ScrollIntoView(IWebDriver driver, IWebElement element)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }
        protected void LogTestResult(string testName, string type, ExtentTest Test, string result, string? errorMessage = null)
        {
            if(type.ToLower().Equals("info"))
            {
                Log.Information(result);
                Test?.Info(result);
            }
            else if(type.ToLower().Equals("pass")&& errorMessage == null)
            {
                Log.Information(testName + "passed");
                Test?.Pass(result);
            }
            else
            {
                Log.Error($"Test failed for {testName} \n Exception: \n{errorMessage}");
                Test.AddScreenCaptureFromPath(TakeScreenshot(), testName);
                Test?.Fail(result);
            }
        }
        protected string TakeScreenshot()
        {
            ITakesScreenshot? its = (ITakesScreenshot?)Driver;
            Screenshot? screenshot = its?.GetScreenshot();
            string filePath = currdir + "/Screenshots/ss_" + DateTime.Now.ToString("yyyy.mm.dd_HH.mm.ss") + ".png";
            screenshot?.SaveAsFile(filePath);
            Console.WriteLine("taken screenshot");
            return filePath;
        }
        protected void InitializeDriver()
        {
            ReadConfigSettings();
            if (Properties?["browser"].ToLower() == "chrome")
            {
                Driver = new ChromeDriver();
            }
            else if (Properties?["browser"].ToLower() == "edge")
            {
                Driver = new EdgeDriver();
            }
            url = Properties?["baseUrl"];
            Driver.Url = url;
            Driver?.Manage().Window.Maximize();
        }

        protected static void WaitForElementToBeClickable(IWebElement element, string elementName)
        {
            if(element != null)
            {
                DefaultWait<IWebElement> fluentWait = new DefaultWait<IWebElement>(element);
                fluentWait.Timeout = TimeSpan.FromSeconds(5);
                fluentWait.PollingInterval = TimeSpan.FromMilliseconds(50);
                fluentWait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                fluentWait.Message = "Element - " + elementName + " - not found or not clickable";
                fluentWait.Until(x => x != null && x.Displayed && x.Enabled);
            }
        }

        [OneTimeSetUp]
        public void Setup()
        {
            InitializeDriver();
            //configure Serilog
            string? logfilePath = currdir + "/Logs/log_" + DateTime.Now.ToString("yyyy.mm.dd_HH.mm.ss") + ".txt";
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(logfilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
            //configure Extent Report
            Extent = new ExtentReports();
            SparkReporter = new ExtentSparkReporter(currdir + "/Reports/report_" + DateTime.Now.ToString("yyyy.MM.dd_HH.mm.ss") + ".html");
            Extent.AttachReporter(SparkReporter);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Driver?.Quit();
            Extent?.Flush();
            Log.CloseAndFlush();
        }
    }
}
