﻿// Author: Dominic Beger (Trade/ProgTrade) 2016

using nUpdate.Core.Win32;

namespace nUpdate.Core
{
    public class ConnectionChecker
    {
        /// <summary>
        ///     Checks if an internet connection is available.
        /// </summary>
        /// <returns>This function returns if a internet connection is available.</returns>
        public static bool IsConnectionAvailable()
        {
            int desc;
            return NativeMethods.InternetGetConnectedState(out desc, 0);
        }
    }
}