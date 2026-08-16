using UserApi.Dtos;
using UserApi.Services;

namespace UserApi.Endpoints;

public static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/", (IUserService service) => Results.Ok(service.GetAll()));

        group.MapGet("/{id:guid}", (Guid id, IUserService service) =>
        {
            var result = service.GetById(id);
            return result.Success ? Results.Ok(result.Value) : ToProblem(result.ErrorType, result.Error!);
        });

        group.MapPost("/", (CreateUserRequest request, IUserService service) =>
        {
            var result = service.Create(request);
            return result.Success
                ? Results.Created($"/users/{result.Value!.Id}", result.Value)
                : ToProblem(result.ErrorType, result.Error!);
        });

        group.MapPut("/{id:guid}", (Guid id, UpdateUserRequest request, IUserService service) =>
        {
            var result = service.Update(id, request);
            return result.Success ? Results.Ok(result.Value) : ToProblem(result.ErrorType, result.Error!);
        });

        group.MapDelete("/{id:guid}", (Guid id, IUserService service) =>
        {
            var result = service.Delete(id);
            return result.Success ? Results.NoContent() : ToProblem(result.ErrorType, result.Error!);
        });

        return group;
    }

    private static IResult ToProblem(UserServiceErrorType errorType, string error) => errorType switch
    {
        UserServiceErrorType.NotFound => Results.NotFound(new { error }),
        UserServiceErrorType.Validation => Results.BadRequest(new { error }),
        UserServiceErrorType.Conflict => Results.Conflict(new { error }),
        _ => Results.Problem(error)
    };
}
