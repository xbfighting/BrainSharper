namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Abstract.Algorithms.DecisionTrees.Processors;

    public class AlreadyUsedAttributesInfo : IAlredyUsedAttributesInfo
    {
        private readonly ConcurrentDictionary<string, ISet<object>> attributesInfo;

        public AlreadyUsedAttributesInfo()
        {
            attributesInfo = new ConcurrentDictionary<string, ISet<object>>();
        }

        public IList<string> AlreadyUsedAttributesList => attributesInfo.Keys.ToList();

        public bool WasAttributeAlreadyUsed(string attributeName)
        {
            return attributesInfo.ContainsKey(attributeName);
        }

        public bool WasAttributeAlreadyUsedWithValue(string attributeName, object attributeValue)
        {

            if (WasAttributeAlreadyUsed(attributeName))
            {
                return attributesInfo[attributeName].Contains(attributeValue);
            }

            return false;
        }

        public void AddAlreadyUsedAttribute(string attributeName, object attrValue = null)
        {

            if (!attributesInfo.ContainsKey(attributeName))
            {
                attributesInfo.TryAdd(attributeName, new HashSet<object>());
            }
            if (attrValue != null)
            {
                attributesInfo[attributeName].Add(attrValue);
            }
        }

    }
}