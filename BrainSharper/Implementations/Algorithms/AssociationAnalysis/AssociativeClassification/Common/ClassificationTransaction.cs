using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.Common
{
    public class ClassificationTransaction<TValue> : ITransaction<IDataItem<TValue>>, IClassificationTransaction<TValue>
    {
        private readonly IDictionary<string, TValue> _featureValues;

        public ClassificationTransaction(
            object transactionKey,
            IEnumerable<IDataItem<TValue>> dataItems,
            string decisiveFeatureName
            )
        {
            TransactionKey = transactionKey;
            TransactionItems = dataItems.ToList();
            DecisiveFeature = decisiveFeatureName;
            _featureValues = new Dictionary<string, TValue>();
            Features = new List<string>(TransactionItems.Count());
            Values = new List<TValue>(TransactionItems.Count());
            foreach (var item in TransactionItems)
            {
                if (item.FeatureName.Equals(decisiveFeatureName))
                {
                    ClassLabelValue = item.FeatureValue;
                }
                _featureValues.Add(item.FeatureName, item.FeatureValue);
                Features.Add(item.FeatureName);
                Values.Add(item.FeatureValue);
            }
        }

        public ClassificationTransaction(
            ITransaction<IDataItem<TValue>> tran,
            string dependentFeatureName
            ) : this(tran.TransactionKey, tran.TransactionItems, dependentFeatureName)
        { }

        public object TransactionKey { get; }
        public IEnumerable<IDataItem<TValue>> TransactionItems { get; } 
        public TValue ClassLabelValue { get; }
        public string DecisiveFeature { get; }
        public IList<string> Features { get; }
        public IList<TValue> Values { get; }
        public bool ContainsFeature(string feature)
        {
            return _featureValues.ContainsKey(feature);
        }

        public TValue GetValueForFeature(string feature)
        {
            return _featureValues[feature];
        }
    }
}
