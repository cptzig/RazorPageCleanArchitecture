// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Razor.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace CleanArchitecture.Razor.Infrastructure.Extensions;

internal static class MiddlewareExtensions
{
    public static IApplicationBuilder UseMiddlewares(this IApplicationBuilder app)
    {
        //app.UseMiddleware<LocalizationCookiesMiddleware>();
        //app.UseMiddleware<ExceptionHandlerMiddleware>();
        return app;
    }
}




