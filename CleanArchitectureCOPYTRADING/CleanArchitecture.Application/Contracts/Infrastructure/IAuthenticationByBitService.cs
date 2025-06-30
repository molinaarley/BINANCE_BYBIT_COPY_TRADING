using System.Security.Cryptography;
using System.Text;
using System;
using CleanArchitecture.Application.Models;
using CleanArchitecture.Domain.Enum;

namespace CleanArchitecture.Application.Contracts.Infrastructure
{
    public interface IAuthenticationByBitService
    {
        Task<long?> ConvertToMilliseconds(DateTime? time);


         Task<DateTime> GetTimestamp();


         Task<string> SignHMACSHA256(string data, string secretKey = "", SignOutputType? outputType = null);


        Task<string> BytesToBase64String(byte[] buff);

        Task<string> BytesToHexString(byte[] buff);
        Task<string?> ConvertToMillisecondsString();


    }
}
