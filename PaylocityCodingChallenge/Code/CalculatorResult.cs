using System;
using System.Collections.Generic;

namespace PaylocityCodingChallenge.Code
{
    public class CalculatorResult
    {
        public Dictionary<Guid, CalculatorResult> SubResults = new Dictionary<Guid, CalculatorResult>();

        public List<RuleResult> Results = new List<RuleResult>();

        public decimal Total;
    }
}
