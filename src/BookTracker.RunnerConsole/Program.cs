﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BookTracker.RunnerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string _isbn = "9781433805615";
            for (int i = 0; i < args.Length; i++)
            {
                if (string.Compare(args[i], "-isbn", true) == 0)
                {
                    _isbn = args[i + 1];
                    break;
                }
            }

            IWebDriver driver = Helpers.CreateWebDriver();

            string linkTitle = RunDriveInSafeMode(ref driver, ExecuteScrapyList, _isbn);
            Console.WriteLine($"Link title {linkTitle}");

            RunDriveInSafeMode(ref driver, ExecuteScrapyDetail, "https://keepa.com/#!product/1-1433805618");

            Console.WriteLine("Press enter to continue");
            Console.ReadKey();

            driver.Quit();
            driver.Dispose();
        }

        static string ExecuteScrapyList(IWebDriver driver, string isbn)
        {
            driver.Navigate().GoToUrl(new Uri($"https://keepa.com/#!search/1-{isbn}"));

            Console.WriteLine("Press enter to continue");
            Console.ReadKey();

            string linkTitleUri = string.Empty;

            var bookElement = driver.FindElement(By.CssSelector("div.ag-body-viewport-wrapper > div.ag-body-viewport > div.ag-body-container > div[row-index='0']"));
            if (bookElement != null)
            {
                var img = bookElement.FindElement(By.CssSelector("div[col-id=\"imagesCSV\"] > span > img"));
                _thumbnail = img.GetAttribute("src");

                var linkTitle = bookElement.FindElement(By.CssSelector("div[col-id=\"title\"] > span > a"));
                linkTitleUri = linkTitle.GetAttribute("href");

                var spanTitle = linkTitle.FindElement(By.TagName("span"));
                _title = spanTitle.GetAttribute("title");

                var amazonElem = bookElement.FindElement(By.CssSelector("div[col-id=\"AMAZON_current\"] > span > span"));
                _amazonPrice = amazonElem.Text;

                var currentPriceElem = bookElement.FindElement(By.CssSelector("div[col-id=\"NEW_current\"] > span > span"));
                _newPrice = currentPriceElem.Text;

                var usedPriceElem = bookElement.FindElement(By.CssSelector("div[col-id=\"USED_current\"] > span > span"));
                _usedPrice = usedPriceElem.Text;

                var editionElem = bookElement.FindElement(By.CssSelector("div[col-id=\"edition\"] > span > span"));
                _edition = editionElem.Text;
            }
            return linkTitleUri;
        }

        static void ExecuteScrapyDetail(IWebDriver driver, string bookUri = "https://keepa.com/#!product/1-1433805618")
        {
            //Navigate to google page
            driver.Navigate().GoToUrl(new Uri(bookUri));

            Console.WriteLine("Presse enter to continue");
            Console.ReadKey();

            var baseElement = driver.FindElement(By.Id(_productBox));
            var titleElement = baseElement.FindElement(By.ClassName("productTableDescriptionTitle"));
            Console.WriteLine(titleElement.Text);


            var figBaseElement = driver.FindElement(By.Id("productTableImageBoxThumbs"));
            var figs = figBaseElement.FindElements(By.ClassName("productThumb"));
            if (figs != null && figs.Count > 0)
            {
                foreach (var fig in figs)
                {
                    _imageLst.Add(fig.GetAttribute("styles"));
                }
            }

            string title = (string)driver.Scripts().ExecuteScript("return document.title");
            Console.WriteLine($"Document title {title}");

            string data = "{path:\"user/settings\",type:\"getSettings\",version:3,id:21688}";
            dynamic r = driver.Scripts().ExecuteScript($"return pako.deflate(JSON.stringify({data}))");

            Console.WriteLine(r);
        }

        static void RunDriveInSafeMode(ref IWebDriver driver, Action<IWebDriver> action)
        {
            try
            {
                action(driver);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error executing generic action");

                Exception _ex = ex;
                while (_ex != null)
                {
                    Trace.TraceError("Exception --> " + _ex.Message);
                    _ex = _ex.InnerException;
                }
            }
            finally
            {
                driver.Quit();
                driver.Dispose();
            }
        }

        static TOut RunDriveInSafeMode<TIn, TOut>(ref IWebDriver driver, Func<IWebDriver, TIn, TOut> function, TIn functionArgument)
            where TOut : class
        {
            try
            {
                return function(driver, functionArgument);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error executing generic action");

                Exception _ex = ex;
                while (_ex != null)
                {
                    Trace.TraceError("Exception --> " + _ex.Message);
                    _ex = _ex.InnerException;
                }
                driver.Quit();
                driver.Dispose();

                driver = Helpers.CreateWebDriver();
            }
            return null;
        }

        static void RunDriveInSafeMode<TIn>(ref IWebDriver driver, Action<IWebDriver, TIn> action, TIn functionArgument)
        {
            try
            {
                action(driver, functionArgument);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error executing generic action");

                Exception _ex = ex;
                while (_ex != null)
                {
                    Trace.TraceError("Exception --> " + _ex.Message);
                    _ex = _ex.InnerException;
                }
                driver.Quit();
                driver.Dispose();

                driver = Helpers.CreateWebDriver();
            }
        }
    }
}
