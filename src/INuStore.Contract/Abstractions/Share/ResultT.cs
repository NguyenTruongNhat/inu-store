using System.Diagnostics.CodeAnalysis;

namespace INuStore.Contract.Abstractions.Share;

public class Result<TValue> : Result
{
    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error) =>
        Value = value;

    [AllowNull]
    public TValue Value => IsSuccess
        ? field!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    public static implicit operator Result<TValue>(TValue? value) => Create(value);
}
