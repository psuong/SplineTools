using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static SplineTools.CatmullRomSpline;
using static SplineTools.Tests.CommonValues;

namespace SplineTools.Tests {

    public class BufferTest {

        private int lineSegments, expectedSize;

        [SetUp]
        public void SetUp() {
            lineSegments = 100;
            expectedSize = lineSegments * Points.Length;
        }

        [Test]
        public void CheckingTheLoopedFixedPointsBufferSize() {
            SampleCatmullRomSpline(in Points, lineSegments, true, out Vector3[] samples);
            Assert.AreEqual(expectedSize, samples.Length, "Buffer size mismatch for points!");
        }
        
        [Test]
        public void CheckingTheLoopedFixedTangentsBufferSize() {
            SampleCatmullRomSplineTangents(in Points, lineSegments, true, out Vector3[] samples);
            Assert.AreEqual(expectedSize, samples.Length, "Buffer size mismatch for tangents!");
        }

        [Test]
        public void CheckingTheLoopedFixedBinormalsBufferSize() {
            SampleCatmullRomBinormals(in Points, lineSegments, true, out Vector3[] samples);
            Assert.AreEqual(expectedSize, samples.Length, "Buffer size mismatch for binormals!");
        }

        [Test]
        public void CheckingTheLoopedDynamicPointsBufferSize() {
            SampleCatmullRomSpline(in Points, lineSegments, true, out IList<Vector3> samples);
            Assert.AreEqual(expectedSize, samples.Count, "Buffer size mismatch for points!");
            samples.Clear();
        }

        [Test]
        public void CheckingTheLoopedDynamicTangentsBufferSize() {
            throw new NotImplementedException();
        }

        [Test]
        public void CheckingTheLoopedDynamicBinormalsBufferSize() {
            throw new NotImplementedException();
        }
    }
}
