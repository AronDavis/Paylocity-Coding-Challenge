using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PaylocityCodingChallenge.Code;
using System;
using System.Collections.Generic;

namespace Unit_Tests.Code
{
    [TestClass]
    public class CalculatorTests
    {
        [TestMethod]
        public void ConstructorTests()
        {
            //test null rules
            new Calculator<IUniqueId>(null);

            //test with no rules
            new Calculator<IUniqueId>(new List<ICalculatorRule<IUniqueId>>());

            //test with rules
            Mock<ICalculatorRule<IUniqueId>> mockRule = new Mock<ICalculatorRule<IUniqueId>>();
            new Calculator<IUniqueId>(new List<ICalculatorRule<IUniqueId>>() { mockRule.Object });
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void CalculateSingleModel(int numRules)
        {
            //Arrange
            int prime = 7;
            List<ICalculatorRule<IUniqueId>> rules = new List<ICalculatorRule<IUniqueId>>();
            List<Mock<ICalculatorRule<IUniqueId>>> mockRules = new List<Mock<ICalculatorRule<IUniqueId>>>();

            for(int i = 0; i < numRules; i++)
            {
                //setup each rule to add prime to the total
                Mock<ICalculatorRule<IUniqueId>> mockRule = new Mock<ICalculatorRule<IUniqueId>>();
                mockRule.Setup(r => r.Calculate(
                    It.IsAny<IUniqueId>(),
                    It.IsAny<CalculatorResult>()
                    )
                ).Callback((IUniqueId model, CalculatorResult result) =>
                {
                    result.Total += prime;
                });

                rules.Add(mockRule.Object);
                mockRules.Add(mockRule);
            }

            Calculator<IUniqueId> calculator =  new Calculator<IUniqueId>(rules);

            Mock<IUniqueId> mockModel = new Mock<IUniqueId>();


            //Act
            CalculatorResult result = calculator.Calculate(mockModel.Object);


            //Assert

            //assert total is numRules * prime
            Assert.AreEqual(numRules * prime, result.Total);

            //assert each rule was called only once
            foreach(Mock<ICalculatorRule<IUniqueId>> mockRule in mockRules)
            {
                mockRule.Verify(r => r.Calculate(
                        It.IsAny<IUniqueId>(),
                        It.IsAny<CalculatorResult>()
                    ), 
                    Times.Once()
                );
            }
        }

        [DataTestMethod]
        [DataRow(0, true)]
        [DataRow(1, true)]
        [DataRow(2, true)]
        [DataRow(0, false)]
        [DataRow(1, false)]
        [DataRow(2, false)]
        public void CalculateMultipleModels(int numModels, bool shouldUseExistingResult)
        {
            //Arrange
            int prime = 7;
            List<ICalculatorRule<IUniqueId>> rules = new List<ICalculatorRule<IUniqueId>>();

            //setup one rule to add prime to the total
            Mock<ICalculatorRule<IUniqueId>> mockRule = new Mock<ICalculatorRule<IUniqueId>>();
            mockRule.Setup(r => r.Calculate(
                It.IsAny<IUniqueId>(),
                It.IsAny<CalculatorResult>()
                )
            ).Callback((IUniqueId model, CalculatorResult result) =>
            {
                result.Total += prime;
            });

            rules.Add(mockRule.Object);

            Calculator<IUniqueId> calculator = new Calculator<IUniqueId>(rules);

            List<IUniqueId> models = new List<IUniqueId>();
            List<Mock<IUniqueId>> mockModels = new List<Mock<IUniqueId>>();

            for (int i = 0; i < numModels; i++)
            {
                Mock<IUniqueId> mockModel = new Mock<IUniqueId>();

                //return unique id
                mockModel.Setup(m => m.Id)
                    .Returns(Guid.NewGuid());

                models.Add(mockModel.Object);
                mockModels.Add(mockModel);
            }

            CalculatorResult existingResult = null;

            if (shouldUseExistingResult)
                existingResult = new CalculatorResult();

            //Act
            CalculatorResult result = calculator.Calculate(models, existingResult);


            //Assert

            //assert total is numModels * prime
            Assert.AreEqual(numModels * prime, result.Total);

            //assert one set of SubResults per model
            Assert.AreEqual(numModels, result.SubResults.Count);

            //assert results are empty
            Assert.AreEqual(0, result.Results.Count);

            foreach (Mock<IUniqueId> mockModel in mockModels)
            {
                //assert each rule was called only once per model
                mockRule.Verify(r => r.Calculate(
                        It.Is<IUniqueId>(model => model == mockModel.Object),
                        It.IsAny<CalculatorResult>()
                    ),
                    Times.Once()
                );

                //get sub results exist
                var subResults = result.SubResults[mockModel.Object.Id];

                //assert total = prime
                Assert.AreEqual(prime, subResults.Total);
            }

            //assert numModels is the same as SubResult count
            Assert.AreEqual(numModels, result.SubResults.Count);

            if (shouldUseExistingResult)
                Assert.AreEqual(existingResult, result);
        }
    }
}
