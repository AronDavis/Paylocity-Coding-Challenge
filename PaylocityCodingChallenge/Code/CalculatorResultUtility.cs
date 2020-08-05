using System.Collections.Generic;

namespace PaylocityCodingChallenge.Code
{
    public class CalculatorResultUtility
    {
        public List<RuleResult> GetAllResults(CalculatorResult result, bool getEmployeesOnly = false)
        {
            List<RuleResult> allResults = new List<RuleResult>();

            if (result == null)
                return allResults;

            _addAllResults(result, allResults, getEmployeesOnly);

            return allResults;
        }

        private void _addAllResults(CalculatorResult result, List<RuleResult> allResults, bool getEmployeesOnly)
        {
            if (result.Results != null)
            {
                foreach (RuleResult ruleResult in result.Results)
                {
                    if (getEmployeesOnly)
                    {
                        if(ruleResult.CostCode == CostCodeEnum.EmployeeBaseCost || ruleResult.CostCode == CostCodeEnum.EmployeeFirstLetterADiscount)
                            allResults.Add(ruleResult);

                        continue;
                    }

                    allResults.Add(ruleResult);
                }
            }

            if (result.SubResults == null)
                return;

            foreach (CalculatorResult subResult in result.SubResults.Values)
            {
                _addAllResults(subResult, allResults, getEmployeesOnly);
            }
        }
    }
}
