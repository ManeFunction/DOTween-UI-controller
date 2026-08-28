using System;
using UnityEngine.UIElements;

namespace ManeFunction.DOTweenExtensions.Editor
{
    [UxmlElement]
    internal partial class LoopCountField : IntegerField
    {
        private const string InfinityLabel = "Infinity";

        protected override string ValueToString(int v)
        {
            return v < 0 ? InfinityLabel : base.ValueToString(v);
        }

        protected override int StringToValue(string str)
        {
            if (string.Equals(str, InfinityLabel, StringComparison.OrdinalIgnoreCase))
                return -1;

            return base.StringToValue(str);
        }
    }
}
