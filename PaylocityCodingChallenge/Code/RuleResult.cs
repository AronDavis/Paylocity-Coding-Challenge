namespace PaylocityCodingChallenge.Code
{
    public class RuleResult
    {
        public CostCodeEnum CostCode { get; }
        public decimal Value { get; }

        public RuleResult(CostCodeEnum costCode, decimal value)
        {
            CostCode = costCode;
            Value = value;
        }
    }
}
