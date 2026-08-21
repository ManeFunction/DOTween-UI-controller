using System;
using UnityEngine.UIElements;

namespace ManeFunction.DOTweenExtensions.Editor
{
    [UxmlElement]
    internal partial class LoopCountField : IntegerField
    {
        protected override string ValueToString(int v)
        {
            return v < 0 ? "Infinity" : base.ValueToString(v);
        }

        protected override int StringToValue(string str)
        {
            if (string.Equals(str, "Infinity", StringComparison.OrdinalIgnoreCase))
                return -1;

            return base.StringToValue(str);
        }
    }
}
