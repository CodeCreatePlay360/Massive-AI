using UnityEngine;
using System.Collections.Generic;
using MassiveAI.Fuzzy.MemberFunctions;


namespace MassiveAI.Fuzzy
{
    /// <summary>
    /// Basic class demonstrates the use of fuzzy logic in game AI.
    /// It evaluates the health status of a character and determines
	/// whether the character should flee. The fuzzy logic system uses
	/// linguistic variables to map health levels and make decisions.
    /// </summary>
    public class FuzzyBasic : UnityEngine.MonoBehaviour
    {
        // Public field to set the health value via the Unity Inspector
        [Range(0, 100)]
        public int healthValue = 0;

        // Private fields for fuzzy input and output
        private FuzzyInput healthStatus;  // Fuzzy input for health status
        private FuzzyOutput shouldFlee;   // Fuzzy output for flee decision

        // Constants representing different health levels
		// (fuzzy linguistic variables)
        private const int low = 0;
        private const int medium = 1;
        private const int high = 2;

        // Cache
        private int lastHealthVal;


        public void Start()
        {
            // Initialize fuzzy input and output
            healthStatus = new FuzzyInput(() => healthValue);
            shouldFlee   = new FuzzyOutput();

            // Map health levels to fuzzy sets (membership functions)
            healthStatus.Set(low,    new LeftShoulder(0, 15, 30));
            healthStatus.Set(medium, new Triangle(15, 45, 60));
            healthStatus.Set(high,   new RightShoulder(45, 70, 100));

            // Map flee decision levels to fuzzy sets
            shouldFlee.Set(low,    new LeftShoulder(-0.5, 0.0, 0.5));
            shouldFlee.Set(medium, new Trapezoid(0, 0.3, 0.7, 1));
            shouldFlee.Set(high,   new RightShoulder(0.5, 1, 1.5));

            // Create fuzzy rules for decision making
            FuzzyRule.If(healthStatus.Is(high)).Then(shouldFlee.Is(low));
            FuzzyRule.If(healthStatus.Is(medium)).Then(shouldFlee.Is(medium));
            FuzzyRule.If(healthStatus.Is(low)).Then(shouldFlee.Is(high));

            // Cache the initial health value
            lastHealthVal = healthValue;
        }

        private void Update()
        {
            // Check if the health value has changed
            if (lastHealthVal != healthValue)
            {
                UnityEngine.Debug.Log($"Flee(Health: {healthStatus.Value}) = {shouldFlee.Evaluate()}");
				
                // Update the cached health value
                lastHealthVal = healthValue;
            }
        }
    }
}
