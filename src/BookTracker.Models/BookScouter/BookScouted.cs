﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookTracker.Models.BookScouter
{
    public class BookScouted
    {
        public string Isbn10 { get; set; }

        public string Isbn13 { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public string Published { get; set; }

        public int AmazonSalesRank { get; set; }

        public int AmazonCount { get; set; }

        public float AmazonLowestPrice { get; set; }

        public int AmazonTradeInPrice { get; set; }

        public string AmazonUrl { get; set; }

        public string Slug { get; set; }

        public string FormattedAuthor { get; set; }

        public int Expected { get; set; }

        public int Fetched { get; set; }

        public Price[] Prices { get; set; }
    }
}