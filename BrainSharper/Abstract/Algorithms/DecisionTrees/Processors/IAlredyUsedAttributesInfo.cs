namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides information about attributes already used in induction process
    /// </summary>
    public interface IAlredyUsedAttributesInfo
    {
        IList<string> AlreadyUsedAttributesList { get; }
        bool WasAttributeAlreadyUsed(string attributeName);
        bool WasAttributeAlreadyUsedWithValue(string attributeName, object attributeValue);

        void AddAlreadyUsedAttribute(string attributeName, object attrValue = null);
    }
}
