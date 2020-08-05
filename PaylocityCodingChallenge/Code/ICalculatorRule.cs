namespace PaylocityCodingChallenge.Code
{
    public interface ICalculatorRule<T>
    {
        void Calculate(T model, CalculatorResult result);
    }
}
