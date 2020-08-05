using PaylocityCodingChallenge.Models;
using System;

namespace PaylocityCodingChallenge.Code
{
    public class DependentRule : ICalculatorRule<DependentModel>
    {
        private readonly decimal _dependentCost;
        private readonly decimal _discount;

        public DependentRule(decimal dependentCost, decimal discount)
        {
            if (dependentCost < 0)
                throw new ArgumentOutOfRangeException(nameof(dependentCost), "Dependent cost cannot be negative.");

            if (discount < 0 || discount > 1)
                throw new ArgumentOutOfRangeException(nameof(discount), "Discount cannot be negative or greater than 100%.");

            _dependentCost = dependentCost;
            _discount = discount;
        }

        public void Calculate(DependentModel model, CalculatorResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result), "Result cannot be null.");

            result.Results.Add(new RuleResult(CostCodeEnum.DependentBaseCost, _dependentCost));
            decimal costAfterDiscount = _dependentCost;

            //if the name starts with A, give them a discount
            if (model?.Name?.Length >= 1 && model.Name.ToLower()[0] == 'a')
            {
                decimal appliedDiscount = costAfterDiscount * _discount;
                result.Results.Add(new RuleResult(CostCodeEnum.DependentFirstLetterADiscount, appliedDiscount));
                costAfterDiscount -= appliedDiscount;
            }

            result.Total += costAfterDiscount;
        }
    }
}
