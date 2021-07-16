using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace MOI_Application
{

  class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            Dictionary<string, string> Resutls = new Dictionary<string, string>();
            Resutls = LoadBrowser("https://frs.gov.cz/en/ioff/application-status");
            Console.WriteLine("The execution took " + sw.Elapsed + " seconds");
            Console.WriteLine("Completed");

        }

        private static IWebDriver driver;
        public static string homeURL;

        private static Dictionary<string, string> LoadBrowser(string Link)
        {
            Dictionary<string, string> Status = new Dictionary<string, string>();
            string Application_Number = "";
            string Application_Status = "";

            IWebElement Status_App;
            var DeviceDriver = ChromeDriverService.CreateDefaultService();
            DeviceDriver.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--disable-infobars");
            //options.AddArgument("headless");
            driver = new ChromeDriver(DeviceDriver, options);
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl(Link);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(50);


            var csv = new StringBuilder();

            //Main Part of the Loop
            for (int index=10122; index< 11122; index++)
            {
                Boolean _isOk = false;
                int Progress = ((index - 10121) / 10);
                    while (_isOk == false)
                    {
                        try
                        {
                            //driver.FindElement(By.Id("edit-ioff-application-number")).Click();
                            //Thread.Sleep(1 * 1000); // wait 2 seconds
                            //driver.FindElement(By.Id("edit-ioff-application-number")).Clear();
                            //Thread.Sleep(1 * 1000); // wait 2 seconds
                            driver.FindElement(By.Id("edit-ioff-application-number")).SendKeys(index.ToString());
                            Thread.Sleep(1 * 1000); // wait 2 seconds
                            
                            
                            driver.FindElement(By.Id("edit-ioff-application-code")).SendKeys("TP");
                            Thread.Sleep(1 * 1000); // wait 2 seconds
                            driver.FindElement(By.Id("edit-ioff-application-year")).SendKeys("2021");
                            Thread.Sleep(1 * 1000); // wait 2 seconds
                            driver.FindElement(By.Id("edit-submit-button")).Click();
                            //Thread.Sleep(1 * 1000); // wait 2 seconds
                            Status_App = driver.FindElement(By.XPath("//em[@class='placeholder']"));
                            Application_Number = Status_App.Text;
                            
                            _isOk = true;
                        }
                        catch
                        {
                            Status_App = driver.FindElement(By.XPath("//div[@class='alert alert-block alert-danger messages error']"));
                            string Error = Status_App.Text;
                            string[] error = Error.Split(' ');
                            foreach (string word in error)
                            {
                                try
                                {
                                    int _waitingTime = int.Parse(word);
                                    if (_waitingTime >= 120)
                                    {
                                        Thread.Sleep(135*1000);
                                        break;
                                    }
                                    else
                                    {
                                        Thread.Sleep((int.Parse(word)*5)*1000);
                                        break;
                                    }
    
                                }
                                catch
                                {

                                }
                            
                            }
                        }


                    }

                    try
                    {
                    Status_App = driver.FindElement(By.XPath("//span[@class='alert alert-warning']"));
                    Application_Status = Status_App.Text;
                    }
                    catch
                    {

                    }
                    try
                    {
                        Status_App = driver.FindElement(By.XPath("//span[@class='alert alert-success']"));
                        Application_Status = Status_App.Text;
                    }
                    catch
                    {

                    }

                    var newLine = string.Format(": {0} ; {1}", Application_Number, Application_Status);
                    csv.Append(newLine);
                    string filePath = @".\Status.csv";
                    try
                    {
                    if (!File.Exists(filePath))
                        File.Create(filePath);

                    File.WriteAllLines(filePath, csv.ToString().Split(':'));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("{0} Exception caught.", e.Message);
                    }

                   drawTextProgressBar(Progress, 100);
                if (_isdone)
                    File.Copy(filePath, _finalname);

            }
            return Status;
        }

        /// <summary>
        /// This Method is to Generate a Progress Bar on the CLI for better User Interface
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="total"></param>
        private static void drawTextProgressBar(int progress, int total)
        {

            if (_isfirstcycle)
            {
                _finalname = "";
                _isdone = false;
                Console.WriteLine("Gathering Data from Minister of Interior");
                Console.WriteLine("Parsing 1000 Applications (500 prior and 500 after the requested one)");
                Console.WriteLine("Estimated Completion Time would be : 50 minutes");
                Console.WriteLine("Starting Time: " + DateTime.Now.ToString());
                Console.WriteLine("Finish Time (Estimated): " + DateTime.Now.AddMinutes(50).ToString());
                Console.Write("Progress --> ");
                _isfirstcycle = false;
            }
            if (progress <= 100)
            {
                //draw empty progress bar
                Console.CursorLeft = 13; //0
                Console.Write("["); //start
                Console.CursorLeft = 45;
                Console.Write("]"); //end
                Console.CursorLeft = 14; //1
                float onechunk = 30.0f / total;

                //draw filled part
                int position = 14;
                for (int i = 0; i < onechunk * progress; i++)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.CursorLeft = position++;
                    Console.Write(" ");
                }

                //draw unfilled part
                for (int i = position; i <= 31; i++)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.CursorLeft = position++;
                    Console.Write(" ");
                }

                //draw totals
                Console.CursorLeft = 48; //35
                if (progress < 50)
                    Console.BackgroundColor = ConsoleColor.Red;
                else if (progress < 75)
                    Console.BackgroundColor = ConsoleColor.Yellow;
                else
                    Console.BackgroundColor = ConsoleColor.Green;
                Console.Write(progress.ToString() + " % "); //blanks at the end remove any excess
                Console.BackgroundColor = ConsoleColor.Black;
            }

            if (progress == total)
            {
                Console.WriteLine("");
                Console.WriteLine(DateTime.Now.Hour.ToString("yy"));
                Console.WriteLine(DateTime.Now.Hour.ToString());
                _finalname = "Status_"  + DateTime.Now.Hour.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Hour.ToString() + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + ".csv";
                Console.WriteLine("Saving the File with name: " + _finalname);
                _isdone = true;
            }
        }

        #region Properties

        /// <summary>
        /// This is Boolean to check if its the first cicle of the schedule task
        /// </summary>
        private static bool _isfirstcycle = true;

        /// <summary>
        /// This is a Boolean to conctrol the end of the requested length of check
        /// </summary>
        private static bool _isdone;

        /// <summary>
        /// This is the name of the file that will be saved (with Hour and Minutes of Finalization)
        /// </summary>
        private static string _finalname;

        #endregion
    }
}
