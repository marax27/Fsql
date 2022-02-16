using Fsql.Core.Evaluation;

namespace Fsql.Core.Functions;

public class ArgumentCountException : ApplicationException
{
    public ArgumentCountException(int expectedCount, int actualCount)
        : base($"Function has received a wrong number of arguments. Expected {expectedCount}, received {actualCount}.")
    {}
}

public class ArgumentTypeException : ApplicationException
{
    public ArgumentTypeException(string expectedType, string actualType)
        : base($"Function has received a wrong argument. Expected <{expectedType}>, received <{actualType}>")
    {}
}
