﻿
namespace eCommerce.Products.Availability.Core.Helpers.Log
{
    public interface ILogHelper
    {
        Task RegisterLog(string? message, params object?[] args);
    }
}
