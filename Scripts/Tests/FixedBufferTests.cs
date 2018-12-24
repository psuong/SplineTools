using NUnit.Framework;
using UnityEngine;
using static SplineTools.CatmullRomSpline;
using static SplineTools.Tests.CommonValues;

namespace SplineTools.Tests {

    public class FixedBufferTests {

        private int lineSegments, expectedSize;

        [SetUp]
        public void SetUp() {
            lineSegments = 100;
            expectedSize = lineSegments * Points.Length;
        }

        [Test]
        public void FixedBufferSampleTest() {
            SampleCatmullRomSpline(in Points, lineSegments, true, out Vector3[] samples);
            Assert.AreEqual(expectedSize, samples.Length, "Buffer size mismatch for points!");
        }
        
        [Test]
        public void FixedBufferTangentsTest() {
            SampleCatmullRomSplineTangents(in Points, lineSegments, true, out Vector3[] samples);
            Assert.AreEqual(expectedSize, samples.Length, "Buffer size mismatch for tangents!");
        }

        [Test]
        public void FixedBufferBinormalsTest() {
            SampleCatmullRomBinormals(in Points, lineSegments, true, out Vector3[] samples);
            Assert.AreEqual(expectedSize, samples.Length, "Buffer size mismatch for binormals!");
        }
    }
}
