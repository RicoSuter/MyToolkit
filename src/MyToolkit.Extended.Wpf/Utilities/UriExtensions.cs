//-----------------------------------------------------------------------
// <copyright file="UriExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace MyToolkit.Utilities
{
    /// <summary>Contains extension methods to work with <see cref="Uri"/>s. </summary>
    public static class UriExtensions
    {
        /// <summary>Finds the certificate of the given <see cref="Uri"/>. </summary>
        /// <param name="uri">The URI. </param>
        /// <returns>The <see cref="X509Certificate2"/> for the given <see cref="Uri"/>, or <c>null</c> if no certificate could be found. </returns>
        /// <exception cref="CryptographicException">The certificate is unreadable. </exception>
        public static X509Certificate2 FindWebCertificate(this Uri uri)
        {
            var request = WebRequest.CreateHttp(uri);
            request.Timeout = 5000;
            request.ServerCertificateValidationCallback = delegate { return true; };
            request.GetResponse();

            var certificate = request.ServicePoint.Certificate;
            if (certificate != null)
                return new X509Certificate2(certificate);

            return null;
        }

        /// <summary>Finds the start and end date of the given <see cref="Uri"/>'s certificate. </summary>
        /// <param name="uri">The URI. </param>
        /// <returns>The expiration range for the given <see cref="Uri"/>, or <c>null</c> if no certificate could be found. </returns>
        /// <exception cref="CryptographicException">The certificate is unreadable. </exception>
        public static CertificateValidityPeriod FindWebCertificateValidityPeriod(this Uri uri)
        {
            var certificate = FindWebCertificate(uri);
            if (certificate != null)
                return new CertificateValidityPeriod(certificate.NotBefore, certificate.NotAfter);
            return null;
        }
    }

    /// <summary>Stores the start and end date of a certificate. </summary>
    public class CertificateValidityPeriod
    {
        /// <summary>Initializes a new instance of the <see cref="CertificateValidityPeriod"/> class.</summary>
        /// <param name="start">The start date.</param>
        /// <param name="end">The end date.</param>
        public CertificateValidityPeriod(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        /// <summary>Gets the start date. </summary>
        public DateTime Start { get; private set; }

        /// <summary>Gets the end date. </summary>
        public DateTime End { get; private set; }

        /// <summary>Checks whether the given time is between <see cref="Start"/> and <see cref="End"/>. </summary>
        /// <param name="time">The time. </param>
        /// <returns><c>true</c> if the <paramref name="time"/> is between <see cref="Start"/> and <see cref="End"/>; otherwise, <c>false</c>. </returns>
        public bool IsValidForDate(DateTime time)
        {
            return time >= Start && time <= End;
        }
    }
}