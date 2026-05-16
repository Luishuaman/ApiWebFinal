using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrudProductos.Middlewares;

public class LoggerMiddleware
{
    
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggerMiddleware> _logger;

    public LoggerMiddleware(RequestDelegate next, ILogger<LoggerMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // ── Antes de la petición ──
        _logger.LogInformation("➡️  [{Method}] {Path} - {Time}",
            context.Request.Method,
            context.Request.Path,
            DateTime.UtcNow.ToString("HH:mm:ss"));

        var sw = System.Diagnostics.Stopwatch.StartNew();

        await _next(context);   // ejecuta el siguiente middleware / endpoint

        sw.Stop();

        // ── Después de la petición ──
        _logger.LogInformation("⬅️  [{StatusCode}] {Path} - {Ms}ms",
            context.Response.StatusCode,
            context.Request.Path,
            sw.ElapsedMilliseconds);
    }
}

// Extensión para registrarlo limpiamente
public static class LoggerMiddlewareExtensions
{
    public static IApplicationBuilder UseLoggerMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<LoggerMiddleware>();
}