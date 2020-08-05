using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PaylocityCodingChallenge.Code;
using PaylocityCodingChallenge.Models;
using System;
using System.Collections.Generic;

namespace Unit_Tests.Code
{
    [TestClass]
    public class EmployeeRuleTests
    {
        [TestMethod]
        public void ConstructorTests()
        {
            //test employee cost can't be negative
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                new EmployeeRule(-1, 1, new Mock<ICalculator<DependentModel>>().Object);
            });

            //test discount can't be negative
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                new EmployeeRule(0, -1, new Mock<ICalculator<DependentModel>>().Object);
            });

            //test discount can't be greater than 1
            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
            {
                new EmployeeRule(0, 2, new Mock<ICalculator<DependentModel>>().Object);
            });

            //test calculator can't be null
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
                new EmployeeRule(0, 1, null);
            });

            //test happy path
            new EmployeeRule(0, 1, new Mock<ICalculator<DependentModel>>().Object);
        }

        [DataTestMethod]
        [DataRow("a", 0)]
        [DataRow("A", 1)]
        [DataRow("aron", 2)]
        [DataRow("Aron", 3)]
        public void DiscountApplied(string employeeName, int numDependents)
        {
            //Arrange
            int employeeCost = 1000;
            decimal discount = .1m;
            decimal dependentCost = 10;

            Mock<ICalculatorRule<DependentModel>> mockDependentRule = new Mock<ICalculatorRule<DependentModel>>();
            mockDependentRule.Setup(c =>
                c.Calculate(
                    It.IsAny<DependentModel>(),
                    It.IsAny<CalculatorResult>()
                )
            ).Callback((DependentModel dependentModel, CalculatorResult result) =>
            {
                result.Results.Add(new RuleResult(CostCodeEnum.DependentBaseCost, dependentCost));
                result.Total += dependentCost;
            });

            List<ICalculatorRule<DependentModel>> dependentCalculatorRules = new List<ICalculatorRule<DependentModel>>()
            {
                mockDependentRule.Object
            };

            ICalculator<DependentModel> dependentCalculator = new Calculator<DependentModel>(dependentCalculatorRules);

            var rule = new EmployeeRule(
                employeeCost: employeeCost,
                discount: discount,
                dependentCalculator: dependentCalculator
                );

            EmployeeModel employeeModel = new EmployeeModel()
            {
                Name = employeeName
            };

            for(int i = 0; i < numDependents; i++)
            {
                employeeModel.Dependents.Add(new DependentModel());
            }

            CalculatorResult result = new CalculatorResult();


            //Act
            rule.Calculate(employeeModel, result);


            //Assert
            decimal expectedTotalCost = employeeCost * (1 - discount);
            expectedTotalCost += numDependents * dependentCost;

            Assert.AreEqual(expectedTotalCost, result.Total);

            Assert.AreEqual(2, result.Results.Count);

            Assert.AreEqual(CostCodeEnum.EmployeeBaseCost, result.Results[0].CostCode);
            Assert.AreEqual(employeeCost, result.Results[0].Value);

            Assert.AreEqual(CostCodeEnum.EmployeeFirstLetterADiscount, result.Results[1].CostCode);
            Assert.AreEqual(employeeCost * discount, result.Results[1].Value);

            Assert.AreEqual(numDependents, result.SubResults.Count);

            for(int i = 0; i < numDependents; i++)
            {
                var dependentModel = employeeModel.Dependents[i];

                var dependentResults = result.SubResults[dependentModel.Id];
                Assert.AreEqual(1, dependentResults.Results.Count);

                Assert.AreEqual(CostCodeEnum.DependentBaseCost, dependentResults.Results[0].CostCode);
                Assert.AreEqual(dependentCost, dependentResults.Results[0].Value);

                Assert.AreEqual(dependentCost, dependentResults.Total);

                Assert.AreEqual(0, dependentResults.SubResults.Count);
            }
        }

        [DataTestMethod]
        [DataRow("b", 0)]
        [DataRow("B", 1)]
        [DataRow("bubba", 2)]
        [DataRow("Bubba", 3)]
        [DataRow("z", 4)]
        [DataRow("Z", 5)]
        [DataRow("zebra", 6)]
        [DataRow("Zebra", 7)]
        [DataRow(null, 8)]
        public void NoDiscountApplied(string employeeName, int numDependents)
        {
            //Arrange
            int employeeCost = 1000;
            decimal discount = .1m;
            decimal dependentCost = 10;

            Mock<ICalculatorRule<DependentModel>> mockDependentRule = new Mock<ICalculatorRule<DependentModel>>();
            mockDependentRule.Setup(c =>
                c.Calculate(
                    It.IsAny<DependentModel>(),
                    It.IsAny<CalculatorResult>()
                )
            ).Callback((DependentModel dependentModel, CalculatorResult result) =>
            {
                result.Results.Add(new RuleResult(CostCodeEnum.DependentBaseCost, dependentCost));
                result.Total += dependentCost;
            });

            List<ICalculatorRule<DependentModel>> dependentCalculatorRules = new List<ICalculatorRule<DependentModel>>()
            {
                mockDependentRule.Object
            };

            ICalculator<DependentModel> dependentCalculator = new Calculator<DependentModel>(dependentCalculatorRules);

            var rule = new EmployeeRule(
                employeeCost: employeeCost,
                discount: discount,
                dependentCalculator: dependentCalculator
                );

            EmployeeModel employeeModel = new EmployeeModel()
            {
                Name = employeeName
            };

            for (int i = 0; i < numDependents; i++)
            {
                employeeModel.Dependents.Add(new DependentModel());
            }

            CalculatorResult result = new CalculatorResult();


            //Act
            rule.Calculate(employeeModel, result);


            //Assert
            decimal expectedTotalCost = employeeCost;
            expectedTotalCost += numDependents * dependentCost;

            Assert.AreEqual(expectedTotalCost, result.Total);

            Assert.AreEqual(1, result.Results.Count);

            Assert.AreEqual(CostCodeEnum.EmployeeBaseCost, result.Results[0].CostCode);
            Assert.AreEqual(employeeCost, result.Results[0].Value);

            Assert.AreEqual(numDependents, result.SubResults.Count);

            for (int i = 0; i < numDependents; i++)
            {
                var dependentModel = employeeModel.Dependents[i];

                var dependentResults = result.SubResults[dependentModel.Id];
                Assert.AreEqual(1, dependentResults.Results.Count);

                Assert.AreEqual(CostCodeEnum.DependentBaseCost, dependentResults.Results[0].CostCode);
                Assert.AreEqual(dependentCost, dependentResults.Results[0].Value);

                Assert.AreEqual(dependentCost, dependentResults.Total);

                Assert.AreEqual(0, dependentResults.SubResults.Count);
            }
        }
    }
}
