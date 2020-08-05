using PaylocityCodingChallenge.Models;
using System;

namespace PaylocityCodingChallenge.Code
{
    public class EmployeeRule : ICalculatorRule<EmployeeModel>
    {
        private readonly decimal _employeeCost;
        private readonly decimal _discount;
        private readonly ICalculator<DependentModel> _dependentCalculator;

        public EmployeeRule(decimal employeeCost, decimal discount, ICalculator<DependentModel> dependentCalculator)
        {
            if (employeeCost < 0)
                throw new ArgumentOutOfRangeException(nameof(employeeCost), "Employee cost cannot be negative.");

            if (discount < 0 || discount > 1)
                throw new ArgumentOutOfRangeException(nameof(discount), "Discount cannot be negative or greater than 100%.");

            if (dependentCalculator == null)
                throw new ArgumentNullException(nameof(dependentCalculator), "Dependent calculator must have a value.");

            _employeeCost = employeeCost;
            _discount = discount;
            _dependentCalculator = dependentCalculator;
        }

        private void _calculateEmployeeCost(EmployeeModel model, CalculatorResult result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result), "Result cannot be null.");

            result.Results.Add(new RuleResult(CostCodeEnum.EmployeeBaseCost, _employeeCost));
            decimal costAfterDiscount = _employeeCost;

            //if the name starts with A, give them a discount
            if (model?.Name?.Length >= 1 && model.Name.ToLower()[0] == 'a')
            {
                decimal appliedDiscount = costAfterDiscount * _discount;
                result.Results.Add(new RuleResult(CostCodeEnum.EmployeeFirstLetterADiscount, appliedDiscount));
                costAfterDiscount -= appliedDiscount;
            }

            result.Total += costAfterDiscount;
        }

        public void Calculate(EmployeeModel employeeModel, CalculatorResult result)
        {
            _calculateEmployeeCost(employeeModel, result);

            _dependentCalculator.Calculate(employeeModel.Dependents, result);
        }
    }
}
