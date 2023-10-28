using System;

namespace Bro
{
    public static class ActionExtension
    {
        public static string GetDescription(this Action action)
        {
            if (action != null)
            {
                var methodName = action.Method.Name;
                var target = action.Target.ToString();

                var targetData = target.Split( new string[] { "+<>c" }, StringSplitOptions.None );
                if (targetData.Length > 1)
                {
                    target = targetData[0];
                }
                var isLambda = methodName.Contains("<");
                
                methodName = methodName.Replace("b__0", string.Empty);
                methodName = methodName.Replace("b__1", string.Empty);
                methodName = methodName.Replace("b__2", string.Empty);
                methodName = methodName.Replace("<", string.Empty);
                methodName = methodName.Replace(">", string.Empty);

                return ( isLambda ? "lambda" : "method" ) + "_" + methodName + "_target_" + target;
            }

            return string.Empty;
        }
    }
}