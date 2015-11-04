namespace BrainSharperTests.Implementations.Algorithms.RuleBasedSystems.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
    using BrainSharper.Abstract.Algorithms.RuleInduction.Processors;
    using BrainSharper.Abstract.Data;
    using BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures;
    using BrainSharper.Implementations.Algorithms.RuleInduction.Processors;

    using NUnit.Framework;

    using TestUtils;

    [TestFixture]
    public class ComplexIntersectorWithSingleValueTest
    {
        private readonly IDataFrame dataFrame = TestDataBuilder.ReadWeatherDataWithCategoricalAttributes();
        private readonly IComplexesIntersector<string> subject = new ComplexIntersectorWithSingleValue<string>();

        [Test]
        public void IntersectComplexWithFeatureDomainsTest_IntersectUniversalComplex()
        {
            // Given
            var featureDomains = subject.PrepareFeatureDomains(
                dataFrame,
                TestDataBuilder.WeatherDataDependentFeatureName);
            var seeds = new List<IComplex<string>> { new Complex<string>() };
            var expectedComplexes = featureDomains.SelectMany(kvp => kvp.Value);

            // When
            var intersectedSeed = subject.IntersectComplexesWithFeatureDomains(
                seeds,
                featureDomains);

            // Then
            CollectionAssert.AreEquivalent(expectedComplexes, intersectedSeed);
        }

        [Test]
        public void IntersectComplexWithFeatresDomains_WithExistingComplexes()
        {
            // Given
            var featureDomains = subject.PrepareFeatureDomains(
                dataFrame,
                TestDataBuilder.WeatherDataDependentFeatureName);
            var seeds = new List<IComplex<string>>
                            {
                                new Complex<string>(
                                    false,
                                    new DisjunctiveSelector<string>("Outlook", "Sunny", "Overcast", "Rainy"), 
                                    new DisjunctiveSelector<string>("Temperature", "Hot", "Cool"))
                            };
            var expectedComplexes = new List<IComplex<string>>
                            {
                                new Complex<string>(
                                    false,
                                    new DisjunctiveSelector<string>("Outlook", "Sunny"),
                                    new DisjunctiveSelector<string>("Temperature", "Hot", "Cool")),
                                new Complex<string>(
                                    false,
                                    new DisjunctiveSelector<string>("Outlook", "Overcast"),
                                    new DisjunctiveSelector<string>("Temperature", "Hot", "Cool")),
                                new Complex<string>(
                                    false,
                                    new DisjunctiveSelector<string>("Outlook", "Rainy"),
                                    new DisjunctiveSelector<string>("Temperature", "Hot", "Cool")),
                                new Complex<string>(
                                    false,
                                    new DisjunctiveSelector<string>("Outlook", "Sunny", "Overcast", "Rainy"),
                                    new DisjunctiveSelector<string>("Temperature", "Hot")),
                                new Complex<string>(
                                    false,
                                    new DisjunctiveSelector<string>("Outlook", "Sunny", "Overcast", "Rainy"),
                                    new DisjunctiveSelector<string>("Temperature", "Cool")),
                                new Complex<string>(
                                    false,
                                    new DisjunctiveSelector<string>("Outlook", "Sunny", "Overcast", "Rainy"),
                                    new DisjunctiveSelector<string>("Temperature", "Hot", "Cool"),
                                    new DisjunctiveSelector<string>("Humidity", "High")),
                                new Complex<string>(
                                    false,
                                    new DisjunctiveSelector<string>("Outlook", "Sunny", "Overcast", "Rainy"),
                                    new DisjunctiveSelector<string>("Temperature", "Hot", "Cool"),
                                    new DisjunctiveSelector<string>("Humidity", "Normal")),
                                new Complex<string>(
                                    false,
                                    new DisjunctiveSelector<string>("Outlook", "Sunny", "Overcast", "Rainy"),
                                    new DisjunctiveSelector<string>("Temperature", "Hot", "Cool"),
                                    new DisjunctiveSelector<string>("Windy", "False")),
                                new Complex<string>(
                                    false,
                                    new DisjunctiveSelector<string>("Outlook", "Sunny", "Overcast", "Rainy"),
                                    new DisjunctiveSelector<string>("Temperature", "Hot", "Cool"),
                                    new DisjunctiveSelector<string>("Windy", "True"))
                            };

            // When
            var actualComplexes = subject.IntersectComplexesWithFeatureDomains(seeds, featureDomains);

            // Then
            CollectionAssert.AreEquivalent(expectedComplexes, actualComplexes);
        }
    }
}
