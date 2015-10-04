namespace BrainSharperTests.Implementations.Algorithms.RandomForests
{
    using System;
    using System.Linq;

    using BrainSharper.Abstract.Algorithms.DecisionTrees;
    using BrainSharper.Abstract.MathUtils.ImpurityMeasures;
    using BrainSharper.General.DataQuality;
    using BrainSharper.General.DataUtils;
    using BrainSharper.Implementations.Algorithms.DecisionTrees;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;
    using BrainSharper.Implementations.Algorithms.RandomForests;
    using BrainSharper.Implementations.MathUtils.ImpurityMeasures;

    using TestUtils;

    using NUnit.Framework;

    [TestFixture]
    public class RandomForestsTests
    {
        private readonly IImpurityMeasure<string> shannonEntropy = new ShannonEntropy<string>();
        private readonly IDecisionTreeModelBuilder binaryTreeBuilder, multiValueTreeBuilder, multiValueTreeBuilderWithBetterNumercValsHandler;
        private readonly IDecisionTreeModelBuilderParams modelBuilderParams = new DecisionTreeModelBuilderParams(false);

        public RandomForestsTests()
        {
            binaryTreeBuilder = new BinaryDecisionTreeModelBuilder(
                new InformationGainRatioCalculator<string>(shannonEntropy, shannonEntropy as ICategoricalImpurityMeasure<string>),
                new BinarySplitSelectorForCategoricalOutcome(new BinaryDiscreteDataSplitter(), new BinaryNumericDataSplitter(), new ClassBreakpointsNumericSplitFinder()),
                new CategoricalDecisionTreeLeafBuilder());
            multiValueTreeBuilder = new MultiSplitDecisionTreeModelBuilder(
                new InformationGainRatioCalculator<string>(shannonEntropy, shannonEntropy as ICategoricalImpurityMeasure<string>),
                new MultiValueSplitSelectorForCategoricalOutcome(new MultiValueDiscreteDataSplitter(), new BinaryNumericDataSplitter(), new ClassBreakpointsNumericSplitFinder()),
                new CategoricalDecisionTreeLeafBuilder());
            multiValueTreeBuilderWithBetterNumercValsHandler = new MultiSplitDecisionTreeModelBuilder(
                new InformationGainRatioCalculator<string>(shannonEntropy, shannonEntropy as ICategoricalImpurityMeasure<string>),
                new MultiValueSplitSelectorForCategoricalOutcome(new MultiValueDiscreteDataSplitter(), new BinaryNumericDataSplitter(), new DynamicProgrammingNumericSplitFinder()),
                new CategoricalDecisionTreeLeafBuilder());
        }

        [Test]
        public void DiscreteClassification_NumericFeatures_MultiValuesSplits_AdultCensusData()
        {
            // Given
            var randomForestBuilder = new RandomForestModelBuilder<object>(
                multiValueTreeBuilderWithBetterNumercValsHandler,
                new DecisionTreePredictor<object>(),
                new ConfusionMatrixBuilder<object>(),
                i => (int)Math.Round(Math.Sqrt(i), MidpointRounding.AwayFromZero),
                () => new DecisionTreeModelBuilderParams(false));
            var randomForestPredictor = new RandomForestPredictor<object>(new DecisionTreePredictor<object>(), true);
            var testData = TestDataBuilder.ReadAdultCensusDataFrame();
            var crossValidator = new CrossValidator<object>();


            // When
            var accuracy = crossValidator.CrossValidate(
                randomForestBuilder,
                new RandomForestParams(10, 100),
                randomForestPredictor,
                new ConfusionMatrixBuilder<object>(),
                testData,
                "income",
                0.8,
                1).First();

            // Then
            Assert.IsTrue(accuracy.Accuracy >= 0.8);
        }

        [Test]
        public void DiscreteClassification_DiscreteFeatures_MultiValuesSplits_CongressVoting()
        {
            // Given
            var randomForestBuilder = new RandomForestModelBuilder<object>(
                multiValueTreeBuilderWithBetterNumercValsHandler,
                new DecisionTreePredictor<object>(),
                new ConfusionMatrixBuilder<object>(),
                i => (int)Math.Round(Math.Sqrt(i), MidpointRounding.AwayFromZero),
                () => new DecisionTreeModelBuilderParams(false));
            var randomForestPredictor = new RandomForestPredictor<object>(new DecisionTreePredictor<object>(), true);
            var testData = TestDataBuilder.ReadCongressData();
            var crossValidator = new CrossValidator<object>();


            // When
            var accuracy = crossValidator.CrossValidate(
                randomForestBuilder,
                new RandomForestParams(100, 10),
                randomForestPredictor,
                new ConfusionMatrixBuilder<object>(),
                testData,
                "party",
                0.7,
                1).First();

            // Then
            Assert.IsTrue(accuracy.Accuracy >= 0.9);
        }

        [Test]
        public void DiscreteClassification_MixedFeatures_MultiValueSplits_CleanedTitanicData()
        {
            // Given
            var randomForestBuilder = new RandomForestModelBuilder<object>(
                multiValueTreeBuilderWithBetterNumercValsHandler,
                new DecisionTreePredictor<object>(),
                new ConfusionMatrixBuilder<object>(),
                i => (int)Math.Round(Math.Sqrt(i), MidpointRounding.AwayFromZero),
                () => new DecisionTreeModelBuilderParams(false, true));
            var randomForestPredictor = new RandomForestPredictor<object>(new DecisionTreePredictor<object>());
            var baseData = TestDataBuilder.ReadTitanicData();
            baseData = baseData.GetSubsetByColumns(baseData.ColumnNames.Except(new[] { "FarePerPerson", "PassengerId", "FamilySize" }).ToList());
            var crossValidator = new CrossValidator<object>();

            // When
            var accuracy = crossValidator.CrossValidate(
                randomForestBuilder,
                new RandomForestParams(200, 10),
                randomForestPredictor,
                new ConfusionMatrixBuilder<object>(),
                baseData,
                "Survived",
                0.75,
                1);

            // Then
            Assert.IsTrue(accuracy.Select(acc => acc.Accuracy).Average() >= 0.75);

            /*
            var qualityMeasure = new ConfusionMatrixBuilder<object>();
            IPredictionModel bestModel = null;
            double accuracy = Double.NegativeInfinity;
            var percetnagOfTrainData = 0.8;

            var trainingDataCount = (int)Math.Round(percetnagOfTrainData * baseData.RowCount);
            var testDataCount = baseData.RowCount - trainingDataCount;
            for (var i = 0; i < 10; i++)
            {
                var shuffledAllIndices = baseData.RowIndices.Shuffle(new Random());
                var trainingIndices = shuffledAllIndices.Take(trainingDataCount).ToList();
                var trainingData = baseData.GetSubsetByRows(trainingIndices);

                var testIndices = shuffledAllIndices.Except(trainingIndices).ToList();
                var testData = baseData.GetSubsetByRows(testIndices);
                IPredictionModel model = randomForestBuilder.BuildModel(trainingData, "Survived", new RandomForestParams(250, 10));
                IList<object> evalPredictions = randomForestPredictor.Predict(testData, model, "Survived");
                IList<object> expected = testData.GetColumnVector<object>("Survived");
                IDataQualityReport<object> qualityReport = qualityMeasure.GetReport(expected, evalPredictions);
                if (qualityReport.Accuracy > accuracy)
                {
                    accuracy = qualityReport.Accuracy;
                    bestModel = model;
                }
            }

            var queryData = TestDataBuilder.ReadTitanicQuery();
            var predictions = randomForestPredictor.Predict(queryData, bestModel, "Survived").Select(elem => (double)Convert.ChangeType(elem, typeof(double))).ToList();
            var passengerIds = queryData.GetNumericColumnVector("PassengerId");

            var matrix = Matrix.Build.DenseOfColumns(new List<IEnumerable<double>>() { passengerIds, predictions });
            DelimitedWriter.Write(@"c:\Users\Filip\Downloads\prediction.csv", matrix, ",");
            Assert.IsTrue(true);
            */
        }
    }
}
