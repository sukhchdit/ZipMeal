using SwiggyClone.Shared;
using Xunit;

namespace SwiggyClone.UnitTests.Shared;

public class ResultTests
{
    [Fact]
    public void Success_ReturnsSuccessResult()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.ErrorCode);
        Assert.Null(result.ErrorMessage);
    }

    [Fact]
    public void Failure_ReturnsFailureResult()
    {
        var result = Result.Failure("TEST_ERROR", "Something went wrong");

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal("TEST_ERROR", result.ErrorCode);
        Assert.Equal("Something went wrong", result.ErrorMessage);
    }

    [Fact]
    public void GenericSuccess_ReturnsValueOnAccess()
    {
        var result = Result<int>.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void GenericFailure_ThrowsOnValueAccess()
    {
        var result = Result<int>.Failure("ERR", "fail");

        Assert.True(result.IsFailure);
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Map_TransformsSuccessValue()
    {
        var result = Result<int>.Success(10);

        var mapped = result.Map(v => v.ToString());

        Assert.True(mapped.IsSuccess);
        Assert.Equal("10", mapped.Value);
    }

    [Fact]
    public void Map_PreservesFailureState()
    {
        var result = Result<int>.Failure("ERR", "fail");

        var mapped = result.Map(v => v.ToString());

        Assert.True(mapped.IsFailure);
        Assert.Equal("ERR", mapped.ErrorCode);
    }
}
