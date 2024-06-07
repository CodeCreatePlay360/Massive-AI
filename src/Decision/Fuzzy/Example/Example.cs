using UnityEngine;
using MassiveAI.Fuzzy.MemberFunctions;

namespace MassiveAI.Fuzzy
{
    public class Example : MonoBehaviour
    {
        private void Start()
        {
            // Initialize a constant for the current amount of ammunition
            const int ammoAmount = 55;

            // Define constants representing fuzzy sets for ammunition levels
            const int low = 0;
            const int medium = 1;
            const int high = 2;

            // Create a FuzzyInput to represent the ammunition status
            FuzzyInput ammoStatus = new(() => ammoAmount);
            ammoStatus.Set(low, new Triangle(0, 25, 50));
            ammoStatus.Set(medium, new Triangle(25, 50, 75));
            ammoStatus.Set(high, new Triangle(50, 75, 100));

            // Create a FuzzyOutput to represent the reload necessity
            FuzzyOutput shouldReload = new();
            shouldReload.Set(low, new Triangle(-0.5, 0, 0.5));
            shouldReload.Set(medium, new Triangle(0, 0.5, 1));
            shouldReload.Set(high, new Triangle(0.5, 1, 1.5));

            // Define fuzzy rules to determine the reload necessity based on ammo status
            FuzzyRule.If(ammoStatus.Is(high)).Then(shouldReload.Is(low));
            FuzzyRule.If(ammoStatus.Is(medium)).Then(shouldReload.Is(medium));
            FuzzyRule.If(ammoStatus.Is(low)).Then(shouldReload.Is(high));

            // Evaluate the fuzzy output for the reload necessity and log the result
			string result = $"Reload desirability when (ammoStatus: {ammoStatus.Value}) = {shouldReload.Evaluate()}";
            UnityEngine.Debug.Log(result);
        }
    }
}
