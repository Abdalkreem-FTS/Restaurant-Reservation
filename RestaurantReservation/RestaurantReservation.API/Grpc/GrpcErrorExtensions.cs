using System.Globalization;
using Grpc.Core;

namespace RestaurantReservation.API.Grpc;

public static class GrpcErrorExtensions
{
    private const string ContentType = "application/grpc";

    private const string ErrorCodeKey = "error-code";

    public static RpcException ToRpcException(this List<Error> errors) =>
        errors.Count == 0
            ? new RpcException(new Status(StatusCode.Internal, "The operation failed."))
            : errors[0].ToRpcException();

    public static RpcException ToRpcException(this Error error) =>
        new(new Status(ToStatusCode(error.Type), error.Description), new Metadata
        {
            { ErrorCodeKey, error.Code }
        });

    extension(HttpContext context)
    {
        public bool IsGrpcRequest() =>
            context.Request.ContentType?.StartsWith(ContentType, StringComparison.OrdinalIgnoreCase) is true;

        public void WriteRpcStatus(Error error)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            context.Response.ContentType = ContentType;

            context.Response.Headers["grpc-status"] = ((int)ToStatusCode(error.Type)).ToString(CultureInfo.InvariantCulture);
            context.Response.Headers["grpc-message"] = Uri.EscapeDataString(error.Description);
            context.Response.Headers[ErrorCodeKey] = error.Code;
        }
    }

    private static StatusCode ToStatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation or ErrorType.Failure => StatusCode.InvalidArgument,
        ErrorType.NotFound => StatusCode.NotFound,
        ErrorType.Conflict => StatusCode.AlreadyExists,
        ErrorType.Unauthorized => StatusCode.Unauthenticated,
        ErrorType.Forbidden => StatusCode.PermissionDenied,
        ErrorType.Unavailable => StatusCode.Unavailable,
        ErrorType.Timeout => StatusCode.DeadlineExceeded,
        _ => StatusCode.Internal,
    };
}
