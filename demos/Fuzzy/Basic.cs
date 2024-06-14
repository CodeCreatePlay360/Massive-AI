using System.Collections.Generic;
using MassiveAI.Fuzzy.MemberFunctions;


namespace MassiveAI.Fuzzy
{
    public class Basic : UnityEngine.MonoBehaviour
    {		
        private void Start()
        {
			/**
			* This class represents a simple onboard automated fire alarm control system.
			* It alerts operators about potential fire threats on a scale of 0 to 100.
			* This approach is beneficial because there is no single fixed temperature that can trigger a fire.
			* Instead, it provides operators with a threat level, helping them assess and respond to potential fire risks.
			*/

            // Crisp temperature value as received from sensors.
            int temperatureValue = 12;  // this value can range from 0-min to 100-max

            // Define constants representing fuzzy sets for 
            // different levels or statuses.
            const int low = 0;
            const int medium = 1;
            const int high = 2;

            // Convert the crisp input values (i.e. temperatureValue) into fuzzy values,
            // for each of the temperature levels (i.e low, medium, high) using the 
			// membership functions for the input fuzzy sets.
            FuzzyInput temperature = new(() => temperatureValue);
            temperature.Set(low,    new Triangle(0, 25, 50));
            temperature.Set(medium, new Triangle(25, 50, 75));
            temperature.Set(high,   new Triangle(50, 75, 100));
			
            // Create a FuzzyOutput to represent the threat level, for each of the different
			// levels of temperature (i.e low, medium, high)
            FuzzyOutput threatLevel = new();
            threatLevel.Set(low,    new LeftShoulder(0, 0.3));
            threatLevel.Set(medium, new Triangle(0.3, 0.5, 0.7));
            threatLevel.Set(high,   new RightShoulder(0.7, 1.0));

            // Create fuzzy rules to determine threat level based on temperature.
            FuzzyRule.If(temperature.Is(low)).Then(threatLevel.Is(low));
            FuzzyRule.If(temperature.Is(medium)).Then(threatLevel.Is(medium));
            FuzzyRule.If(temperature.Is(high)).Then(threatLevel.Is(high));

            // Evaluate the fuzzy output for the threat level and log the result.
			double threatVal = System.Math.Round(threatLevel.Evaluate(), 3);
            string msg = $"Threat level when temperature is {temperatureValue} = {threatVal}";
            UnityEngine.Debug.Log(msg);
        }		
    }
}
