using MassiveAI.Fuzzy.Interfaces;


namespace MassiveAI.Fuzzy
{
    public static class FuzzyRule
    {
        public static IRuleBuilder If(ICondition condition)
        {
            return new FuzzyRuleBuilder(condition);
        }
    }
}
