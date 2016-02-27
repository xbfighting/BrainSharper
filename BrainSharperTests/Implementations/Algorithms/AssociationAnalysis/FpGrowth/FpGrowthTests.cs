using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.Common;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.FPGrowth;
using BrainSharperTests.TestUtils;
using NUnit.Framework;
using static BrainSharperTests.Implementations.Algorithms.AssociationAnalysis.AssociationAnalysisTestDataBuilder;


namespace BrainSharperTests.Implementations.Algorithms.AssociationAnalysis.FpGrowth
{
    [TestFixture]
    public class FpGrowthTests
    {
        protected readonly FpGrowthBuilder<IDataItem<object>> Subject = new FpGrowthBuilder<IDataItem<object>>();

        [Test]
        public void TestBuildingFpTree()
        {
            // Given
            var data = new TransactionsSet<IDataItem<object>>(
                MarketBasketDataSet1
                .ToAssociativeTransactionsSet(TranId)
                .TransactionsList
                .OrderBy(tran => tran.TransactionKey)
                ); 

            var miningParams = new FrequentItemsMiningParams(0.3, 0.7);
            Subject.FindFrequentItems(data, miningParams);

        }

    }
}
