using System;
using UnityEngine.UIElements;

namespace Mane.Unity.DOTween.Editor
{
    [UxmlElement]
    internal partial class LoopCountField : IntegerField
    {
        private const string InfinityLabel = "Infinity";
        private const string NoneLabel = "None";

        protected override string ValueToString(int v) => v == 0 
            ? NoneLabel 
            : v < 0 
                ? InfinityLabel 
                : base.ValueToString(v);

        protected override int StringToValue(string str)
        {
            if (string.Equals(str, InfinityLabel, StringComparison.OrdinalIgnoreCase))
                return -1;

            return base.StringToValue(str);
        }
    }
}
