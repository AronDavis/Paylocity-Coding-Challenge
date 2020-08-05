using System.Collections.Generic;

namespace PaylocityCodingChallenge.Code
{
    public interface ICalculator<T> where T : IUniqueId
    {
        CalculatorResult Calculate(T model);

        CalculatorResult Calculate(IEnumerable<T> models, CalculatorResult resultSet = null);
    }
}
