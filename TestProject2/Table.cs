using System.Collections.ObjectModel;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TestProject2
{
    [TestFixture]
    public class WorkingWithWebTable
    {
        IWebDriver driver;

        [SetUp]
        public void SetUp()
        {
            ChromeOptions options = new ChromeOptions();
            // Ensure Chrome runs in headless mode
            options.AddArguments("headless");
            // Bypass OS security model
            options.AddArguments("no-sandbox");
            // Overcome limited resource problems
            options.AddArguments("disable-dev-shm-usage");
            // Applicable to Windows OS only
            options.AddArguments("disable-gpu");
            // Set window size to ensure elements are visible
            options.AddArguments("window-size=1920x1080");
            // Disable extensions
            options.AddArguments("disable-extensions");
            // Remote debugging port
            options.AddArguments("remote-debugging-port=9222");
            driver = new ChromeDriver(options);

            // Add implicit wait
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [Test]
        public void TestExtractProductInformation()
        {
            // Launch Chrome browser with the given URL
            driver.Url = "http://practice.bpbonline.com/";

            // Identify the web table
            IWebElement productTable = driver.FindElement(By.XPath("//*[@id='bodyContent']/div/div[2]/table"));

            // Find the number of rows
            ReadOnlyCollection<IWebElement> tableRows = productTable.FindElements(By.XPath("//tbody/tr"));

            // Path to save the CSV file
            string path = System.IO.Directory.GetCurrentDirectory() + "/productinformation.csv";

            // If the file exists in the location, delete it
                        if (File.Exists(path))
                File.Delete(path);

            // Traverse through table rows to find the table columns
               foreach (IWebElement trow in tableRows)
            {
                ReadOnlyCollection<IWebElement> tableCols = trow.FindElements(By.XPath("td"));
                foreach (IWebElement tcol in tableCols)
                {
                    // Extract product name and cost
                    String data = tcol.Text;
                    String[] productinfo = data.Split('\n');
                    String printProductinfo = productinfo[0].Trim() + "," + productinfo[1].Trim() + "\n";

                    // Write product information extracted to the file
                    File.AppendAllText(path, printProductinfo);
                }
            }

            // Verify the file was created and has content
            Assert.IsTrue(File.Exists(path), "CSV file was not created");
            Assert.IsTrue(new FileInfo(path).Length > 0, "CSV file is empty");
        }

        [TearDown]
        public void TearDown()
        {
            // Quit the driver
            driver.Quit();
        }
    }
}