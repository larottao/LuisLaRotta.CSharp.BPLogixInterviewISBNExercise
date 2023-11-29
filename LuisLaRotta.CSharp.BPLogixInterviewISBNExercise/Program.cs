/*
===============================================================================
Software Development exercise for BPLogix

 Description: A program that takes ISBNs from a file and gets the book data
 from openlibrary.org API

 Author:      Luis Felipe La Rotta
 Created:     2023-11-28
===============================================================================
*/

using LuisLaRotta.CSharp.BPLogixInterviewISBNExercise.Services;
using System;

namespace LuisLaRotta.CSharp.BPLogixInterviewISBNExercise
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(new BookOpsService().mainProcess(args));

            Console.WriteLine("Press any key to exit...");

            Console.ReadKey();
        }
    }
}