﻿using BookTracker.Models.BookScouter;
using BookTracker.Models.Options;
using BookTracker.Services.ExternalServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookTracker.Services.Http
{
    public class BookScouterService : IBookScouterService
    {
        private readonly string _baseUri;
        private readonly ILogger<BookScouterService> _logger;

        public BookScouterService(IOptions<BookScouterOptions> options, ILogger<BookScouterService> logger)
        {
            _baseUri = options.Value.BaseUri;
            _logger = logger;
        }

        public async Task<BookScouterResponse> GetBook(string isbn)
        {
            var client = new RestClient(_baseUri);

            var request = new RestRequest("prices/sell/{term}");

            request.AddUrlSegment("term", isbn);

            var response = await client.ExecuteGetTaskAsync(request);

            if (!response.IsSuccessful)
            {
                _logger.LogInformation($"Error trying to get book isbn {isbn}");
                _logger.LogError($"Http status code {response.StatusCode} content -> {response.Content}");
            }

            var data = JsonConvert.DeserializeObject<BookScouterResponse>(response.Content);
            return data;
        }
    }
}