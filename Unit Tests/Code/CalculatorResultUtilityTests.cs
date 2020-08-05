using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PaylocityCodingChallenge.Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Unit_Tests.Code
{
    [TestClass]
    public class CalculatorResultUtilityTests
    {
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void GetAll(int numLayers)
        {
            CostCodeEnum[] costCodes = Enum.GetValues(typeof(CostCodeEnum))
                .Cast<CostCodeEnum>()
                .ToArray();

            CalculatorResultUtility calculatorResultUtility = new CalculatorResultUtility();

            CalculatorResult calculatorResult = _makeResult(costCodes, numLayers);

            var allResults = calculatorResultUtility.GetAllResults(calculatorResult);
            int expectedCount = _getExpectedCount(numLayers) * costCodes.Length;

            Assert.AreEqual(expectedCount, allResults.Count);

            //assert results have correct values
            for(int i = 0; i < allResults.Count; i++)
            {
                RuleResult result = allResults[i];
                Assert.AreEqual(costCodes[i % costCodes.Length], result.CostCode);
                Assert.AreEqual(i % costCodes.Length, result.Value);
            }
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        public void GetEmployeesOnly(int numLayers)
        {
            CostCodeEnum[] costCodes = Enum.GetValues(typeof(CostCodeEnum)).Cast<CostCodeEnum>()
                .Where(c => c == CostCodeEnum.EmployeeBaseCost || c == CostCodeEnum.EmployeeFirstLetterADiscount)
                .ToArray();

            CalculatorResultUtility calculatorResultUtility = new CalculatorResultUtility();

            CalculatorResult calculatorResult = _makeResult(costCodes, numLayers);

            var allResults = calculatorResultUtility.GetAllResults(calculatorResult, true);
            int expectedCount = _getExpectedCount(numLayers) * costCodes.Length;

            Assert.AreEqual(expectedCount, allResults.Count);

            //assert results have correct values
            for (int i = 0; i < allResults.Count; i++)
            {
                RuleResult result = allResults[i];
                Assert.AreEqual(costCodes[i % costCodes.Length], result.CostCode);
                Assert.AreEqual(i % costCodes.Length, result.Value);
            }
        }

        //Please forgive the overly complicated calculation in here.
        private int _getExpectedCount(int numLayers)
        {
            int result = 1;

            //counting layers so we know how far to go in retrieving a factorial
            int layerCount = 1;
            int currentLayer = numLayers;

            while(currentLayer > 0)
            {
                result += _getFactorial(numLayers, layerCount);
                currentLayer--;
                layerCount++;
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="maxDepth">Should be a non-zero positive number.</param>
        /// <returns></returns>
        private int _getFactorial(int number, int maxDepth)
        {
            int depthCount = 0;

            int result = 1;
            while (number > 0 && depthCount < maxDepth)
            {
                result *= number;
                number--;
                depthCount++;
            }

            return result;
        }

        private CalculatorResult _makeResult(CostCodeEnum[] costCodes, int numLayers)
        {
            CalculatorResult calculatorResult = new CalculatorResult();

            for (int i = 0; i < costCodes.Length; i++)
            {
                calculatorResult.Results.Add(new RuleResult(costCodes[i], i));
            }

            for(int i = 0; i < numLayers; i++)
            {
                calculatorResult.SubResults[Guid.NewGuid()] = _makeResult(costCodes, numLayers - 1);
            }

            return calculatorResult;
        }
    }
}
