/*
===============================================================================
Software Development exercise for BPLogix

 Description: A program that takes ISBNs from a file and gets the book data
 from openlibrary.org API

 Author:      Luis Felipe La Rotta
 Created:     2023-11-28
===============================================================================
*/

using LuisLaRotta.CSharp.BPLogixInterviewISBNExercise.Interfaces;
using LuisLaRotta.CSharp.BPLogixInterviewISBNExercise.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuisLaRotta.CSharp.BPLogixInterviewISBNExercise.Services
{
    public class BookOpsService : IBooksOps
    {
        public String mainProcess(String[] args)
        {
            //**************************************************
            //Check if a file is passed as an argument, if not,
            //launch the Select File dialog
            //**************************************************

            String selectedFile = "";

            if (args.Length == 0)
            {
                Tuple<Boolean, String> dialogResult = launchSelectISBNFileDialog();

                if (!dialogResult.Item1)
                {
                    return dialogResult.Item2;
                }

                Console.WriteLine("Selected file: " + dialogResult.Item2);

                selectedFile = dialogResult.Item2;
            }
            else
            {
                selectedFile = args[0];
            }

            //**************************************************
            //Try to extract ISBNs contained in file
            //**************************************************

            Tuple<bool, string, IEnumerable<string>> fileExtractResult = returnISBNsInFile(selectedFile);

            if (!fileExtractResult.Item1)
            {
                return fileExtractResult.Item2;
            }

            //**************************************************
            //Given the required ISBNs, look fot them locally
            //first, if not found try with the web service
            //**************************************************

            int rowNumber = 1;

            Variables.isbnsCache.Clear();

            List<Book> resultsListForUser = new List<Book>();

            foreach (String isbnCode in fileExtractResult.Item3)
            {
                //**************************************************
                // Try to get from local cache
                //**************************************************

                if (Variables.isbnsCache.TryGetValue(isbnCode, out Book bookOnCache))
                {
                    //**************************************************
                    // Create a new instance to avoid overwriting
                    // the existing books on the user view
                    //**************************************************

                    Book cachedBook = new Book
                    {
                        Isbn = bookOnCache.Isbn,
                        Title = bookOnCache.Title,
                        Subtitle = bookOnCache.Subtitle,
                        Authors = bookOnCache.Authors,
                        NumberOfPages = bookOnCache.NumberOfPages,
                        PublishDate = bookOnCache.PublishDate,
                        rowNumber = rowNumber,
                        DataRetrievalType = DataRetrievalType.Cache,
                        OnlineRetrieveSuccess = bookOnCache.OnlineRetrieveSuccess,
                        OnlineRetrieveErrorReason = bookOnCache.OnlineRetrieveErrorReason
                    };

                    resultsListForUser.Add(cachedBook);
                }
                //**************************************************
                // If local cache failed, try to get info from webservice
                //**************************************************
                else
                {
                    Book bookOnline = getBookFromWebservice(isbnCode).Result;
                    bookOnline.rowNumber = rowNumber;
                    bookOnline.Isbn = isbnCode;
                    bookOnline.DataRetrievalType = DataRetrievalType.Server;
                    resultsListForUser.Add(bookOnline);

                    //**************************************************
                    // If webservice was successful, store
                    // the retrieved book in local cache
                    //**************************************************

                    if (bookOnline.OnlineRetrieveSuccess)
                    {
                        Variables.isbnsCache.Add(isbnCode, bookOnline);
                    }
                }

                rowNumber++;
            }

            //**************************************************
            //After finishing show results on Console and save
            //**************************************************

            String resultsAsCsv = convertOutputToCsv(resultsListForUser);

            showCsvOnConsole(resultsAsCsv);

            saveCsvOnDisk(selectedFile, resultsAsCsv, 0);

            return resultsAsCsv;
        }

        public Tuple<bool, string> launchSelectISBNFileDialog()
        {
            try
            {
                string selectedFileName = "";

                var t = new Thread((ThreadStart)(() =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();

                    openFileDialog.ShowDialog((new Form() { TopMost = true }));

                    selectedFileName = openFileDialog.FileName;
                }));

                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                t.Join();

                if (String.IsNullOrEmpty(selectedFileName))
                {
                    return new Tuple<Boolean, String>(false, Variables.ERROR_NO_FILE_SELECTED);
                }

                if (!File.Exists(selectedFileName))
                {
                    return new Tuple<Boolean, String>(false, Variables.ERROR_FILE_DOES_NOT_EXIST);
                }

                return new Tuple<Boolean, String>(true, selectedFileName);
            }
            catch (Exception ex)
            {
                return new Tuple<Boolean, String>(false, "Unable to launch select folder dialog: " + ex.ToString());
            }
        }

        public Tuple<bool, string, IEnumerable<string>> returnISBNsInFile(String filePath)
        {
            string fileContent = File.ReadAllText(filePath);

            if (Regex.IsMatch(fileContent, "^[a-zA-Z0-9\n]*$"))
            {
                return new Tuple<Boolean, String, IEnumerable<String>>(false, Variables.ERROR_INVALID_CHARACTERS, new List<String>());
            }

            try
            {
                IEnumerable<String> detectedIsbnsList = fileContent.Split(new[] { ',', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(value => value.Trim());

                if (!detectedIsbnsList.Any())
                {
                    return new Tuple<Boolean, String, IEnumerable<String>>(false, Variables.ERROR_FILE_EMPTY, new List<String>());
                }

                if (detectedIsbnsList.Any(isbn => isbn.Length > 13))
                {
                    return new Tuple<Boolean, String, IEnumerable<String>>(false, Variables.ERROR_ISBN_TOO_LONG, new List<String>());
                }

                foreach (String isbn in detectedIsbnsList)
                {
                    Console.WriteLine("ISBN found on file: " + isbn);
                }

                return new Tuple<Boolean, String, IEnumerable<String>>(true, detectedIsbnsList.Count() + " ISBNs found", detectedIsbnsList);
            }
            catch (Exception ex)
            {
                return new Tuple<Boolean, String, IEnumerable<String>>(false, Variables.ERROR_PROCESSING_ISBN_FILE + " " + ex, new List<String>());
            }
        }

        public async Task<Book> getBookFromWebservice(string requiredIsbn)
        {
            Book book = new Book();

            book.Isbn = requiredIsbn;

            string apiUrl = Variables.OPENLIBRARY_URL + requiredIsbn + ".json";

            Tuple<Boolean, String> wsResponse = await new GetJsonFromWebservice().get(apiUrl);

            if (!wsResponse.Item1)
            {
                book.OnlineRetrieveSuccess = false;
                book.OnlineRetrieveErrorReason = wsResponse.Item2;
                return book;
            }

            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(wsResponse.Item2);

            if (jsonObject == null)
            {
                book.OnlineRetrieveSuccess = false;
                book.OnlineRetrieveErrorReason = Variables.ERROR_PARSING_WS_RESPONSE;
                return book;
            }

            //**************************************************
            //Get rid of the useless (and anways changing)
            // json header
            //**************************************************

            var record = jsonObject["records"].First();

            //**************************************************
            //Data sanitation
            //**************************************************

            string dataSection = record.First["data"].ToString().Replace("\r", "").Replace("\n", "");

            Book parsedBook = JsonConvert.DeserializeObject<Book>(dataSection);

            if (parsedBook == null)
            {
                book.OnlineRetrieveSuccess = false;
                book.OnlineRetrieveErrorReason = Variables.ERROR_PARSING_WS_RESPONSE;
                return book;
            }

            book = parsedBook;
            book.OnlineRetrieveSuccess = true;
            book.OnlineRetrieveErrorReason = "N/A";

            return book;
        }

        public string convertOutputToCsv(List<Book> booksList)
        {
            StringBuilder sb = new StringBuilder();

            //**************************************************
            //Prepare and clean data with csv specific
            //sanitation
            //**************************************************

            foreach (Book element in booksList)
            {
                String concatenatedAuthors = "N/A";

                try
                {
                    if (element.Authors != null && element.Authors.Any())
                    {
                        concatenatedAuthors = string.Join("; ", element.Authors.Select(author => author.Name));
                    }
                }
                catch
                {
                    //Unable to concatenate author names
                }

                sb.Append($"{element.rowNumber}|{element.DataRetrievalType}|{element.Isbn}|{element.Title}|{element.Subtitle}|{concatenatedAuthors}|{element.NumberOfPages}|{element.PublishDate}" + Environment.NewLine);
            }

            String resultWithoutEmptySpaces = sb.ToString().Replace("||", "|N/A|").Replace("|\n", "|N/A\n");

            return resultWithoutEmptySpaces;
        }

        public void showCsvOnConsole(string csvContents)
        {
            Console.WriteLine("");
            Console.WriteLine("RESULTS --------------------------------------------------------");
            Console.WriteLine("");

            Console.WriteLine(csvContents);

            Console.WriteLine("END OF RESULTS -------------------------------------------------");

            Console.WriteLine("");
        }

        public void saveCsvOnDisk(string filePath, string csvContents, int tries)
        {
            if (tries == 0)
            {
                Console.WriteLine("Press any key to export this view as CSV file...");
                Console.ReadKey();
            }

            try
            {
                string directory = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                string modifiedFileName = $"{fileName}_extraction.csv";

                string modifiedFilePath = Path.Combine(directory, modifiedFileName);

                File.WriteAllText(modifiedFilePath, csvContents);
                Console.WriteLine(Variables.SUCCESS_SAVING_CSV + " " + modifiedFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Variables.ERROR_SAVING_CSV + ex);

                if (tries < 1)
                {
                    Console.WriteLine("Press any key to try saving again (one last time)...");
                    saveCsvOnDisk(filePath, csvContents, 1);
                }
            }
        }
    }
}