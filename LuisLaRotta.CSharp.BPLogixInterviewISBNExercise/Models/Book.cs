/*
===============================================================================
Software Development exercise for BPLogix

 Description: A program that takes ISBNs from a file and gets the book data
 from openlibrary.org API

 Author:      Luis Felipe La Rotta
 Created:     2023-11-28
===============================================================================
*/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LuisLaRotta.CSharp.BPLogixInterviewISBNExercise.Models
{
    public enum DataRetrievalType
    { Server, Cache, Failed }

    public class Book
    {
        public int rowNumber;
        public DataRetrievalType DataRetrievalType { get; set; }
        public Boolean OnlineRetrieveSuccess { get; set; }
        public string OnlineRetrieveErrorReason { get; set; }

        public string Isbn { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }

        [JsonProperty("authors")]
        public List<Author> Authors { get; set; }

        [JsonProperty("number_of_pages")]
        public int NumberOfPages { get; set; }

        [JsonProperty("publish_date")]
        public string PublishDate { get; set; }
    }

    public class Author
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}