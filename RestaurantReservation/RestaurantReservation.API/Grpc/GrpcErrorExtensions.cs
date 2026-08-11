using Grpc.Core;

namespace RestaurantReservation.API.Grpc;

public static class GrpcErrorExtensions
{
    public static RpcException ToRpcException(this List<Error> errors) =>
        errors.Count == 0
            ? new RpcException(new Status(StatusCode.Internal, "The operation failed."))
            : errors[0].ToRpcException();

    public static RpcException ToRpcException(this Error error) =>
        new(new Status(ToStatusCode(error.Type), error.Description), new Metadata
        {
            { "error-code", error.Code }
        });

    private static StatusCode ToStatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCode.InvalidArgument,
        ErrorType.NotFound => StatusCode.NotFound,
        ErrorType.Conflict => StatusCode.AlreadyExists,
        ErrorType.Unauthorized => StatusCode.Unauthenticated,
        ErrorType.Forbidden => StatusCode.PermissionDenied,
        ErrorType.Unavailable => StatusCode.Unavailable,
        ErrorType.Timeout => StatusCode.DeadlineExceeded,
        _ => StatusCode.Internal,
    };
}
