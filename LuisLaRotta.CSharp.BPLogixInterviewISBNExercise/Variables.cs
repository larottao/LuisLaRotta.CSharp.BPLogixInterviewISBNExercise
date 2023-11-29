/*
===============================================================================
Software Development exercise for BPLogix

 Description: A program that takes ISBNs from a file and gets the book data
 from openlibrary.org API

 Author:      Luis Felipe La Rotta
 Created:     2023-11-28
===============================================================================
*/

using LuisLaRotta.CSharp.BPLogixInterviewISBNExercise.Models;
using System;
using System.Collections.Generic;

namespace LuisLaRotta.CSharp.BPLogixInterviewISBNExercise
{
    public static class Variables
    {
        public const String OPENLIBRARY_URL = "http://openlibrary.org/api/volumes/brief/isbn/";

        public const String ERROR_NO_FILE_SELECTED = "No folder selected by user.";

        public const String ERROR_FILE_DOES_NOT_EXIST = "The file does not exist anymore.";

        public const String ERROR_INVALID_CHARACTERS = "Error: The file contains invalid characters and could not be processed.";

        public const String ERROR_FILE_EMPTY = "Error: No ISBNs were found in the file.";

        public const String ERROR_ISBN_TOO_LONG = "Error: One or more ISBN is the file is longer than the standard 13 characters.";

        public const String ERROR_PROCESSING_ISBN_FILE = "Error processing the ISBN file: ";

        public const String ERROR_EMPTY_RESPONSE = "Error the webservice returned a null or empty response. ";

        public const String ERROR_PARSING_WS_RESPONSE = "Error processing the webservice response. ";

        public const String SUCCESS_SAVING_CSV = "CSV file saved successfully.";

        public const String ERROR_SAVING_CSV = "Unable to save CSV file.";
        public static Dictionary<String, Book> isbnsCache { get; set; } = new Dictionary<String, Book>();
    }
}