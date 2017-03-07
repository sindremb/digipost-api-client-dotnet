﻿using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Digipost.Api.Client.Api;
using Digipost.Api.Client.Domain.Identify;
using Digipost.Api.Client.Domain.Mailbox;
using Digipost.Api.Client.Domain.Search;
using Digipost.Api.Client.Domain.SendMessage;

namespace Digipost.Api.Client
{
    public class DigipostClient
    {
        private readonly DigipostApi _api;

        public DigipostClient(ClientConfig clientConfig, X509Certificate2 businessCertificate)
        {
            var requestHelper = new RequestHelper(clientConfig, businessCertificate);
            _api = new DigipostApi(clientConfig, businessCertificate, requestHelper);
        }

        public DigipostClient(ClientConfig clientConfig, string thumbprint)
        {
            _api = new DigipostApi(clientConfig, thumbprint);
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

        public Mailbox Mailbox(string senderId)
        {
            return new Mailbox(senderId);
        }
    }
}