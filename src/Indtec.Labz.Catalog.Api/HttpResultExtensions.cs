using Indtec.Labz.Catalog.Domain.Results;

namespace Indtec.Labz.Catalog.Api;

public static class HttpResultExtensions
{
    public static IResult ToProblem(this Result result, int statusCode = StatusCodes.Status400BadRequest)
        => Results.Problem(statusCode: statusCode, title: "Domain validation failed", extensions: new Dictionary<string, object?>
        {
            ["errors"] = result.Errors.Select(x => new { x.Code, x.Message }).ToArray()
        });
}