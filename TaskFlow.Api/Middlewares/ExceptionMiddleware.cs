using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace TaskFlow.Api.Middleware;

public class ExceptionMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<ExceptionMiddleware> _logger;

  public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task Invoke(HttpContext context)
  {
    try
    {
      await _next(context);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unhandled exception occurred");

      context.Response.ContentType = "application/json";

      var response = context.Response;
      var error = new ErrorResponse();

      switch (ex)
      {
        case UnauthorizedAccessException:
          response.StatusCode = (int)HttpStatusCode.Unauthorized;
          error.Title = "Unauthorized";
          error.Detail = ex.Message;
          break;

        case DbUpdateException:
          response.StatusCode = (int)HttpStatusCode.BadRequest;
          error.Title = "Database update failed";
          error.Detail = ex.InnerException?.Message ?? ex.Message;
          break;

        case KeyNotFoundException:
          response.StatusCode = (int)HttpStatusCode.NotFound;
          error.Title = "Not Found";
          error.Detail = ex.Message;
          break;

        default:
          response.StatusCode = (int)HttpStatusCode.InternalServerError;
          error.Title = "Internal Server Error";
          error.Detail = ex.Message;
          break;
      }

      error.Status = response.StatusCode;

      var json = JsonSerializer.Serialize(error);
      await context.Response.WriteAsync(json);
    }
  }

  private class ErrorResponse
  {
    public int Status { get; set; }
    public string Title { get; set; } = "Error";
    public string Detail { get; set; } = "An error occurred.";
  }
}
