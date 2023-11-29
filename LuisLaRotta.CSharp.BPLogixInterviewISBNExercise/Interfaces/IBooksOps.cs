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
using System.Threading.Tasks;

namespace LuisLaRotta.CSharp.BPLogixInterviewISBNExercise.Interfaces
{
    internal interface IBooksOps
    {
        Tuple<Boolean, String> launchSelectISBNFileDialog();

        Tuple<Boolean, String, IEnumerable<String>> returnISBNsInFile(String filePath);

        Task<Book> getBookFromWebservice(String requiredIsbn);

        String convertOutputToCsv(List<Book> booksList);

        void showCsvOnConsole(String csvContents);

        void saveCsvOnDisk(string filePath, String csvContents, int tries);

        String mainProcess(String[] args);
    }
}