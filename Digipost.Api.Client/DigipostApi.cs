﻿using Digipost.Api.Client.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Digipost.Api.Client
{
   public class DigipostApi
    {
        private static bool isHttps = true;
        
     //   public static readonly string BaseAddress = isHttps ? "https://qa2.api.digipost.no/" : "http://qa2.api.digipost.no/";
        public static readonly string BaseAddress = isHttps ? "https://api.digipost.no/" : "http://api.digipost.no/";


        public static async Task<string> Send(string technicalSenderId,  Message message)
        {
            var loggingHandler = new LoggingHandler(new HttpClientHandler());

            using (var client = new HttpClient(loggingHandler))
            {
                client.BaseAddress = new Uri(BaseAddress);

                var method = "POST";
                var uri = "messages";
                var date = DateTime.UtcNow.ToString("R");

                var boundary = Guid.NewGuid().ToString();

                client.DefaultRequestHeaders.Add("X-Digipost-UserId", technicalSenderId);
                client.DefaultRequestHeaders.Add("Date", date);
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.digipost-v6+xml");

                using (var content = new MultipartFormDataContent(boundary))
                {
                    var mediaTypeHeaderValue = new MediaTypeHeaderValue("multipart/mixed");
                    mediaTypeHeaderValue.Parameters.Add(new NameValueWithParametersHeaderValue("boundary", boundary));
                    content.Headers.ContentType = mediaTypeHeaderValue;

                    {
                        string xmlMessage="";
                        try
                        {
                            xmlMessage = SerializeUtil.Serialize(message);
                        }
                        catch (Exception e)
                        {
                            int i = 0;
                        }

                        var messageContent = new StringContent(xmlMessage);
                        messageContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.digipost-v6+xml");
                        messageContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = "\"message\""
                        };
                        content.Add(messageContent);
                    }

                    {
                        var documentContent = new ByteArrayContent(message.PrimaryDocument.contentOfDocument);
                        documentContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                        documentContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = message.PrimaryDocument.uuid
                        };
                        content.Add(documentContent);
                    }

                    {
                        foreach (Document attachment in message.Attachment)
                        {
                            var attachmentContent = new ByteArrayContent(attachment.contentOfDocument);
                            attachmentContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                            attachmentContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                            {
                                FileName = attachment.uuid
                            };
                            content.Add(attachmentContent);
                        }
                    }

                    var multipartContent = await content.ReadAsByteArrayAsync();
                    var computeHash = ComputeHash(multipartContent);

                    client.DefaultRequestHeaders.Add("X-Content-SHA256", computeHash);
                    client.DefaultRequestHeaders.Add("X-Digipost-Signature", ComputeSignature(method, uri, date, computeHash, technicalSenderId));

                    try
                    {
                        var result = client.PostAsync(uri, content).Result;
                        Debug.WriteLine(await result.Content.ReadAsStringAsync());
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message, e);
                        int i = 0;
                    }
                }


            }

            return null;
        }

        public static async Task<string> Send(string userId, string forsendelseId, string digipostAdresse, string emne, string xmlMessage, byte[] primaryDocument, string primaryDocumentGuid,Dictionary<string, byte[]> attachmentList)
        {
            var loggingHandler = new LoggingHandler(new HttpClientHandler());

            using (var client = new HttpClient(loggingHandler))
            {
                client.BaseAddress = new Uri(BaseAddress);

                var method = "POST";
                var uri = "messages";
                var date = DateTime.UtcNow.ToString("R");
                                
                var boundary = Guid.NewGuid().ToString();

                client.DefaultRequestHeaders.Add("X-Digipost-UserId", userId);
                client.DefaultRequestHeaders.Add("Date", date);
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.digipost-v6+xml");

                using (var content = new MultipartFormDataContent(boundary))
                {
                    var mediaTypeHeaderValue = new MediaTypeHeaderValue("multipart/mixed");
                    mediaTypeHeaderValue.Parameters.Add(new NameValueWithParametersHeaderValue("boundary", boundary));
                    content.Headers.ContentType = mediaTypeHeaderValue;

                    {
                        var messageContent = new StringContent(xmlMessage);
                        messageContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.digipost-v6+xml");
                        messageContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = "\"message\""
                        };
                        content.Add(messageContent);
                    }

                    {
                        var documentContent = new ByteArrayContent(primaryDocument);
                        documentContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                        documentContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = primaryDocumentGuid
                        };
                        content.Add(documentContent);
                    }

                    {
                        foreach(KeyValuePair<string, byte[]> entry in attachmentList)
                        {
                            var attachmentContent = new ByteArrayContent(entry.Value);
                            attachmentContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                            attachmentContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                            {
                                FileName = entry.Key
                            };
                            content.Add(attachmentContent);
                        }
                    }

                    var multipartContent = await content.ReadAsByteArrayAsync();
                    var computeHash = ComputeHash(multipartContent);

                    client.DefaultRequestHeaders.Add("X-Content-SHA256", computeHash);
                    client.DefaultRequestHeaders.Add("X-Digipost-Signature", ComputeSignature(method, uri, date, computeHash, userId));

                    try
                    {
                        var result = client.PostAsync(uri, content).Result;
                        Debug.WriteLine(await result.Content.ReadAsStringAsync());
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message, e);
                        int i = 0;
                    }
                }


            }

            return null;
        }

        private static string ComputeHash(Byte[] inputBytes)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();
            Byte[] hashedBytes = hashAlgorithm.ComputeHash(inputBytes);

            return Convert.ToBase64String(hashedBytes);
        }

     

        private static string ComputeSignature(string method, string uri, string date, string sha256Hash, string userId)
        {
            var parameters = ""; //HttpUtility.UrlEncode(request.RequestUri.Query).ToLower();

            Debug.WriteLine("Canonical string generated by .NET Client:");
            Debug.WriteLine("===START===");

            var s = method.ToUpper() + "\n" +
                    "/" + uri.ToLower() + "\n" +
                    "date: " + date + "\n" +
                    "x-content-sha256: " + sha256Hash + "\n" +
                    "x-digipost-userid: " + userId + "\n" +
                    parameters + "\n";

            Debug.Write(s);
            Debug.WriteLine("===SLUTT===");


            var rsa = GetCert().PrivateKey as RSACryptoServiceProvider;
            var privateKeyBlob = rsa.ExportCspBlob(true);
            var rsa2 = new RSACryptoServiceProvider();
            rsa2.ImportCspBlob(privateKeyBlob);

            var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
            var signature = rsa2.SignHash(hash, CryptoConfig.MapNameToOID("SHA256"));

            return Convert.ToBase64String(signature);
        }

        private static X509Certificate2 GetCert()
        {
            return
                new X509Certificate2(
                    @"C:\Users\krist\Documents\GitHub\certificate.p12",
                    "Qwer12345", X509KeyStorageFlags.Exportable);
        }
    }
}
