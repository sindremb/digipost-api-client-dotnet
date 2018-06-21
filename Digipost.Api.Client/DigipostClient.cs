﻿using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Digipost.Api.Client.Api;
using Digipost.Api.Client.Common;
using Digipost.Api.Client.Common.Handlers;
using Digipost.Api.Client.Common.Identify;
using Digipost.Api.Client.Common.Search;
using Digipost.Api.Client.Common.Utilities;
using Digipost.Api.Client.Send;
using Digipost.Api.Client.Shared.Certificate;

namespace Digipost.Api.Client
{
    public class DigipostClient
    {
        private readonly SendMessageApi _api;
        private readonly ClientConfig _clientConfig;
        private readonly RequestHelper _requestHelper;

        public DigipostClient(ClientConfig clientConfig, string thumbprint)
            : this(clientConfig, CertificateUtility.SenderCertificate(thumbprint))
        {
        }

        public DigipostClient(ClientConfig clientConfig, X509Certificate2 businessCertificate)
        {
            _clientConfig = clientConfig;
            _requestHelper = new RequestHelper(GetHttpClient(businessCertificate));
            _api = new SendMessageApi(new SendRequestHelper(_requestHelper));
        }

        private HttpClient GetHttpClient(X509Certificate2 businessCertificate)
        {
            var loggingHandler = new LoggingHandler(
                new HttpClientHandler {AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate},
                _clientConfig
            );

            var authenticationHandler = new AuthenticationHandler(_clientConfig, businessCertificate, loggingHandler);

            var httpClient = new HttpClient(authenticationHandler)
            {
                Timeout = TimeSpan.FromMilliseconds(_clientConfig.TimeoutMilliseconds),
                BaseAddress = new Uri(_clientConfig.Environment.Url.AbsoluteUri)
            };

            return httpClient;
        }

        public Inbox.Inbox GetInbox(Sender senderId)
        {
            return new Inbox.Inbox(senderId, _requestHelper);
        }

        public IIdentificationResult Identify(IIdentification identification)
        {
            return _api.Identify(identification);
        }

        public Task<IIdentificationResult> IdentifyAsync(IIdentification identification)
        {
            return _api.IdentifyAsync(identification);
        }

        public IMessageDeliveryResult SendMessage(IMessage message)
        {
            return _api.SendMessage(message);
        }

        public Task<IMessageDeliveryResult> SendMessageAsync(IMessage message)
        {
            return _api.SendMessageAsync(message);
        }

        public ISearchDetailsResult Search(string query)
        {
            return _api.Search(query);
        }

        public Task<ISearchDetailsResult> SearchAsync(string query)
        {
            return _api.SearchAsync(query);
        }
    }
}
