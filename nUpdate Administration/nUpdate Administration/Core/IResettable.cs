﻿namespace nUpdate.Administration.Core
{
    /// <summary>
    ///     Offers methods for resetting data when asynchronous operations failed.
    /// </summary>
    public interface IResettable
    {
        void Reset();
    }
}