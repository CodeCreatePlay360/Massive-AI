using System;
using System.Collections.Generic;
using MassiveAI.Fuzzy.Interfaces;


namespace MassiveAI.Fuzzy.MemberFunctions
{
    public class RightShoulder : IMemberFunction, IHasCentroid
    {
        private readonly double _min;
        private readonly double _max;
        
        public RightShoulder(double min, double max)
        {
            _min = min;
            _max = max;
        }

        public double Evaluate(double x)
        {
            if (!double.IsFinite(x)) return double.NaN;

            if (x <= _min) return 0.0;
            if (x >= _max) return 1.0;
            return (x - _min) / (_max - _min);
        }

        Centroid IHasCentroid.GetCentroid(double y)
        {
            if (y == 0) return new Centroid(0, 0);
            if (y == 1) return new Centroid((_min + _max) / 2, _max - _min);

            List<Centroid> centroids = new();

            // Left part where membership is less than 1
            double leftMax = _min + (_max - _min) * y;
            double leftCenter = (_min + leftMax) / 2;
            double leftWeight = (leftMax - _min) * y / 2;
            centroids.Add(new Centroid(leftCenter, leftWeight));

            // Right part where membership is 1
            double rightCenter = (leftMax + _max) / 2;
            double rightWeight = (_max - leftMax) * y / 2 + (_max - leftMax) * y;
            centroids.Add(new Centroid(rightCenter, rightWeight));

            return centroids.Sum();
        }
    }
}
