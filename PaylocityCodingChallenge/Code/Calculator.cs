using System.Collections.Generic;

namespace PaylocityCodingChallenge.Code
{
    public class Calculator<T> : ICalculator<T> where T : IUniqueId
    {
        private List<ICalculatorRule<T>> _rules;

        public Calculator(IEnumerable<ICalculatorRule<T>> rules)
        {
            _rules = new List<ICalculatorRule<T>>();

            if (rules != null)
                _rules.AddRange(rules);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model">The model the calculation pertains to.</param>
        /// <returns></returns>
        public CalculatorResult Calculate(T model)
        {
            CalculatorResult result = new CalculatorResult();

            foreach (var rule in _rules)
            {
                rule.Calculate(model, result);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="models">The models the calculation pertains to.</param>
        /// <param name="resultSet">An optional result set to add on to.</param>
        /// <returns>If a result set is passed in, it will return that result set with updated values.  Otherwise a new result set will be returned.</returns>
        public CalculatorResult Calculate(IEnumerable<T> models, CalculatorResult resultSet = null)
        {
            if (resultSet == null)
                resultSet = new CalculatorResult();

            foreach (var model in models)
            {
                CalculatorResult result = Calculate(model);

                resultSet.Total += result.Total;

                //add to sub result
                resultSet.SubResults[model.Id] = result;
            }

            return resultSet;
        }
    }
}
