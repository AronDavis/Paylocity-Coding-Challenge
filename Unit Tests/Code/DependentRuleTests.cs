using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaylocityCodingChallenge.Code;
using PaylocityCodingChallenge.Models;
using System;

namespace Unit_Tests.Code
{
    [TestClass]
    public class DependentRuleTests
    {
        [TestMethod]
        public void ConstructorTests()
        {
            //test dependent cost can't be negative
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                new DependentRule(-1, 1);
            });

            //test discount can't be negative
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                new DependentRule(0, -1);
            });

            //test discount can't be greater than 1
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                new DependentRule(0, 2);
            });

            //test happy path
            new DependentRule(0, 1);
        }

        [DataTestMethod]
        [DataRow("a")]
        [DataRow("A")]
        [DataRow("aron")]
        [DataRow("Aron")]
        public void DiscountApplied(string dependentName)
        {
            //Arrange
            int dependentCost = 500;
            decimal discount = .1m;

            var rule = new DependentRule(
                dependentCost: dependentCost,
                discount: discount
                );

            DependentModel dependentModel = new DependentModel()
            {
                Name = dependentName
            };

            CalculatorResult result = new CalculatorResult();


            //Act
            rule.Calculate(dependentModel, result);


            //Assert
            decimal expectedTotalCost = dependentCost * (1 - discount);

            Assert.AreEqual(expectedTotalCost, result.Total);

            Assert.AreEqual(2, result.Results.Count);

            Assert.AreEqual(CostCodeEnum.DependentBaseCost, result.Results[0].CostCode);
            Assert.AreEqual(dependentCost, result.Results[0].Value);

            Assert.AreEqual(CostCodeEnum.DependentFirstLetterADiscount, result.Results[1].CostCode);
            Assert.AreEqual(dependentCost * discount, result.Results[1].Value);

            Assert.AreEqual(0, result.SubResults.Count);
        }

        [DataTestMethod]
        [DataRow("b")]
        [DataRow("B")]
        [DataRow("bubba")]
        [DataRow("Bubba")]
        [DataRow("z")]
        [DataRow("Z")]
        [DataRow("zebra")]
        [DataRow("Zebra")]
        [DataRow(null)]
        public void NoDiscountApplied(string dependentName)
        {
            //Arrange
            int dependentCost = 500;
            decimal discount = .1m;

            var rule = new DependentRule(
                dependentCost: dependentCost,
                discount: discount
                );

            DependentModel dependentModel = new DependentModel()
            {
                Name = dependentName
            };

            CalculatorResult result = new CalculatorResult();


            //Act
            rule.Calculate(dependentModel, result);


            //Assert
            decimal expectedTotalCost = dependentCost;

            Assert.AreEqual(dependentCost, result.Total);

            Assert.AreEqual(1, result.Results.Count);

            Assert.AreEqual(CostCodeEnum.DependentBaseCost, result.Results[0].CostCode);
            Assert.AreEqual(dependentCost, result.Results[0].Value);

            Assert.AreEqual(0, result.SubResults.Count);
        }
    }
}
