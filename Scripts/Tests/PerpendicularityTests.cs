using NUnit.Framework;
using UnityEngine;
using static SplineTools.Tests.CommonValues;
using static SplineTools.CatmullRomSpline;

namespace SplineTools.Tests {
    public class PerpendicularityTests {
        private bool ArePerpendicular(Vector3 tangent, Vector3 normal, out float value) {
            return Mathf.Abs(value = Vector3.Dot(tangent, normal)) < float.Epsilon;
        }

        [Test]
        public void AllBinormalsArePerpendicularToTheNormal() {
            SampleCatmullRomBinormals(in Points, Points.Length, true, out Vector3[] binormals);

            for (int i = 0; i < binormals.Length; i++) {
                Assert.IsTrue(ArePerpendicular(binormals[i], Vector3.up, out var value),
                    $"Perpendicular points' dot products must be 0, but was {value} for {i}, {binormals[i]} and {Vector3.up}!");
            }
        }

        [Test]
        public void AllBinormalsArePerpendicularToTheirTangents() {
            SampleCatmullRomBinormals(in Points, Points.Length, true, out Vector3[] binormals);
            SampleCatmullRomSplineTangents(in Points, Points.Length, true, out Vector3[] tangents);

            var size = binormals.Length;
            for (int i = 0; i < size; i++) {
                Assert.IsTrue(ArePerpendicular(binormals[i], tangents[i], out var value),
                    $"Perpendicular points' dot products must be 0, but was {value} for {i}, {binormals[i]} and {tangents[i]}");
            }
        }
    }
}