//-----------------------------------------------------------------------
// <copyright file="ServiceClientUtilities.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.ServiceModel;

namespace MyToolkit.Utilities
{
    /// <summary>Provides utility methods for WCF service clients. </summary>
    public static class ServiceClientUtilities
    {
        /// <summary>Disposes the service client. </summary>
        /// <param name="service">The service client. </param>
        /// <param name="isDisposed">The reference to a value indicating whether the service is disposed. </param>
        public static void Dispose(ICommunicationObject service, ref bool isDisposed)
        {
            if (isDisposed)
                return;

            try
            {
                if (service.State == CommunicationState.Faulted)
                    service.Abort();
                else
                {
                    try
                    {
                        service.Close();
                    }
                    catch (Exception closeException)
                    {
                        try
                        {
                            service.Abort();
                        }
                        catch (Exception abortException)
                        {
                            throw new AggregateException(closeException, abortException);
                        }
                        throw;
                    }
                }
            }
            finally
            {
                isDisposed = true;
            }
        }
    }
}