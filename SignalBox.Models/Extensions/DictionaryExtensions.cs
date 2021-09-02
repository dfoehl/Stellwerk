using System;
using System.Collections.Generic;
using System.Text;

namespace SignalBox.Models.Extensions
{
    public static class DictionaryExtensions
    {
        public static void Replace<TTargetKey, TTargetValue, TSourceKey, TSourceValue>(this IDictionary<TTargetKey, TTargetValue> target, IDictionary<TSourceKey, TSourceValue> source) 
        {
            target.Clear();
            foreach (var item in source)
            {
                //target.Add((TTargetKey)item.Key, (TSourceKey)item.Value);
            }
        }
    }
}
