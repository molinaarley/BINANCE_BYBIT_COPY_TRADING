using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using CleanArchitecture.Application.Contracts.Infrastructure;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Enum;
using EllipticCurve.Utils;
using Ionic.Zip;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using Telegram.Bot;

namespace CleanArchitecture.Infrastructure.Services
{

    public class AuthenticationByBitService : IAuthenticationByBitService
    {
        //*********copiado de bybit.net**************
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private byte[] _sBytes;
        //**********************
        private readonly string _remoteServiceBaseUrl;
        private readonly int _recvWindow=5000;
        
        //private readonly IRestClient _restClient;

        public ILogger<AuthenticationByBitService> _logger { get; }
     
        public AuthenticationByBitService() { }

        public AuthenticationByBitService(ILogger<AuthenticationByBitService> logger)
        {
            _logger = logger;
            _sBytes = Encoding.UTF8.GetBytes("ltEHhA91lPHnPTUpMYiNb15AVFjvkLLXV1mc");

        }

        /*
         DateTime currentTimeUtc = DateTime.UtcNow;
            var timestamp = ((DateTimeOffset)currentTimeUtc).AddMinutes(1).AddSeconds(-3).ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture);

         */

        public async Task<string?> ConvertToMillisecondsString()
        {
            DateTime currentTimeUtc = DateTime.UtcNow;
            var timestamp = ((DateTimeOffset)currentTimeUtc).AddSeconds(77)/*.AddSeconds(-2)*/.ToUnixTimeMilliseconds().ToString(CultureInfo.InvariantCulture);
            return timestamp;
        }

        //**********copiado************

        public async Task<long?>   ConvertToMilliseconds(DateTime? time)
        {
            if (time.HasValue)
            {
                return (long)Math.Round((time.Value - _epoch).TotalMilliseconds);
            }

            return null;
        }

        public async Task<DateTime> GetTimestamp(/*RestApiClient apiClient*/)
        {
            return DateTime.UtcNow.Add( TimeSpan.Zero);
           // var datetime= DateTime.UtcNow.Add(TimeZone.CurrentTimeZone.GetUtcOffset(new DateTime ())  );
            //return datetime;

            /*return DateTime.UtcNow.Add(apiClient.GetTimeOffset() ?? TimeSpan.Zero);*/
        }

        public async Task<string> SignHMACSHA256(string data,string secretKey="", SignOutputType? outputType = null)
        {
            _sBytes = Encoding.UTF8.GetBytes(secretKey);
            using HMACSHA256 hMACSHA = new HMACSHA256(_sBytes);
            byte[] buff = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(data));
            return (outputType == SignOutputType.Base64) ? await BytesToBase64String(buff) :await BytesToHexString(buff);
        }

       /* protected string SignRSASHA256(byte[] data, SignOutputType? outputType = null)
        {
            using RSA rSA = RSA.Create();
            if (_credentials.CredentialType == ApiCredentialsType.RsaPem)
            {
                throw new Exception("Pem format not supported when running from .NetStandard2.0. Convert the private key to xml format.");
            }

            if (_credentials.CredentialType == ApiCredentialsType.RsaXml)
            {
                rSA.FromXmlString(_credentials.Secret.GetString());
                using SHA256 sHA = SHA256.Create();
                byte[] hash = sHA.ComputeHash(data);
                byte[] buff = rSA.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return (outputType == SignOutputType.Base64) ? BytesToBase64String(buff) : BytesToHexString(buff);
            }

            throw new Exception("Invalid credentials type");
        }*/

        public async Task<string> BytesToBase64String(byte[] buff)
        {
            return Convert.ToBase64String(buff);
        }
        public async Task<string> BytesToHexString(byte[] buff)
        {
            string text = string.Empty;
            foreach (byte b in buff)
            {
                text += b.ToString("X2");
            }

            return text;
        }
        //************copiado*********************
    }
}

