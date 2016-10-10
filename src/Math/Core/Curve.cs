﻿using System.Collections.Generic;
using System.Linq;

namespace Fusee.Math.Core
{
    /// <summary>
    /// Represents a curve, using a list of CurveParts.
    /// </summary>
    public class Curve
    {
        /// <summary>
        /// The parts making up the Curve.
        /// </summary>
        public IList<CurvePart> CurveParts;

        /// <summary>
        /// Combines two Curves by creating a new one.
        /// </summary>
        /// <param name="a">The first curve to combine</param>
        /// <param name="b">The second curve to combine</param>
        /// <returns></returns>
        public static Curve CombineCurve(Curve a, Curve b)
        {
            //Concat returns a new list, without modifying the original
            var combinedCurve = new Curve { CurveParts = (IList<CurvePart>)a.CurveParts.Concat(b.CurveParts) };
            return combinedCurve;
        }

        /// <summary>
        /// Combines a list of Curves by creating a new Curve out of the list.
        /// </summary>
        /// <param name="curves">The curves to combine</param>
        /// <returns></returns>
        public static Curve CombineCurve(IEnumerable<Curve> curves)
        {
            var combinedCurve = new Curve { CurveParts = new List<CurvePart>() };
            foreach (var curve in curves)
            {
                foreach (var part in curve.CurveParts)
                {
                    combinedCurve.CurveParts.Add(part);
                }
            }
            return combinedCurve;
        }

        /// <summary>
        /// Calculates a polyline, representing the curve itself.
        /// </summary>
        /// <param name="curveSegments">The number of subdivisions per curve segment</param>
        /// <returns></returns>
        public IEnumerable<float3> CalcUniformPolyline(int curveSegments)
        {
            foreach (var part in CurveParts)
            {
                foreach (var vert in part.CalcUniformPolyline(curveSegments))
                {
                    yield return vert;
                }
            }
        }
    }


    /// <summary>
    /// Represents a open or closed part of a curve, using a list of CurveSegments and its starting point.
    /// </summary>
    public class CurvePart
    {
        /// <summary>
        /// A CurvePart can be closed or open.
        /// </summary>
        public bool Closed;

        /// <summary>
        /// This is the starting point of the CurvePart.
        /// </summary>
        public float3 StartPoint;

        /// <summary>
        /// The segments making up the CurvePart.
        /// </summary>
        public IList<CurveSegment> CurveSegments;

        /// <summary>
        /// Calculates a polyline, representing the curve part.
        /// </summary>
        /// <param name="segmentCount">The number of subdivisions per curve segment</param>
        /// <returns></returns>
        public IEnumerable<float3> CalcUniformPolyline(int segmentCount)
        {
            for (var i = 0; i < CurveSegments.Count; i++)
            {
                float3 sp;
                var degree = 0;

                if (CurveSegments[i].GetType() == typeof(LinearSegment))
                {
                    degree = 1;
                }
                else if (CurveSegments[i].GetType() == typeof(BezierConicSegment))
                {
                    degree = 2;
                }
                else if (CurveSegments[i].GetType() == typeof(BezierCubicSegment))
                {
                    degree = 3;
                }

                //If i == 0 sp = StartPoint, if not it's the last vert of the CurveSegment[i-1]s' list of vertices
                sp = i == 0 ? StartPoint : CurveSegments[i - 1].Vertices[CurveSegments[i - 1].Vertices.Count - 1];

                foreach (var vert in CurveSegments[i].CalcUniformPolyline(sp, segmentCount, degree))
                {
                    yield return vert;
                }
            }
        }
    }

    /// <summary>
    /// The base class for CurveSegments.
    /// A CurveSgment does not know its own start point. For the first CurveSegment in a sequence the start point is saved in the CurvePart belonging to the segment.
    /// The start point for the CurveSegment with index i is the last vertex in the CurveSegent[i-1]s' list of vertices.
    /// </summary>
    public abstract class CurveSegment
    {
        /// <summary>
        /// The Vertices, representet as float3, of a CurveSegment.
        /// </summary>
        public IList<float3> Vertices;

        /// <summary>
        /// Calculates a point on a beziér curve using De Casteljaus algorithm.
        /// </summary>
        /// <param name="t">Beziér curves are polynominals of t. t is element of [0, 1] </param>
        /// <param name="vertices">All control points, incl. start and end point</param>
        /// <returns></returns>
        public virtual float3 CalcPoint(float t, float3[] vertices)
        {
            if (vertices.Length == 1)
                return vertices[0];

            var newVerts = new float3[vertices.Length - 1];

            for (var i = 0; i < newVerts.Length; i++)
            {
                //calculates a weighted average of vertices[i] and vertices[i + 1] for x,y,z --> point on line between vertices[i] and vertices[i + 1]
                var x = (1 - t) * vertices[i].x + t * vertices[i + 1].x;
                var y = (1 - t) * vertices[i].y + t * vertices[i + 1].y;
                var z = (1 - t) * vertices[i].z + t * vertices[i + 1].z;
                newVerts[i] = new float3(x, y, z);
            }
            return CalcPoint(t, newVerts);
        }

        /// <summary>
        /// Calculates a polyline, representing the curve segment.
        /// </summary>
        /// <param name="startPoint">The segments starting point</param>
        /// <param name="segmentsPerCurve">The number of segments per curve</param>
        /// <param name="degree">The degree of the curve: 1 for linear, 2 for conic, 3 for cubic</param>
        public virtual IEnumerable<float3> CalcUniformPolyline(float3 startPoint, int segmentsPerCurve, int degree)
        {
            var controlPoints = new List<float3> { startPoint };
            controlPoints.AddRange(Vertices);

            for (var i = 0; i < controlPoints.Count - degree; i += degree)
            {
                //Returns all points that already lie on the curve (i +=2)
                yield return controlPoints[i];

                var verts = new float3[degree + 1];

                for (var j = 0; j < verts.Length; j++)
                {
                    verts[j] = controlPoints[i + j];
                }

                for (var j = 1; j < segmentsPerCurve; j++)
                {
                    var t = j / (float)segmentsPerCurve;
                    var point = CalcPoint(t, verts);
                    yield return point;
                }
            }
            //Manually adds the last control point to maintain the order of the points
            yield return controlPoints[controlPoints.Count - 1];
        }

    }

    /// <summary>
    /// Represents a linear segment of a CurvePart, using a list of float3.
    /// A CurveSgment does not know its own start point. For the first CurveSegment in a sequence the start point is saved in the CurvePart belonging to the segment.
    /// The start point for the CurveSegment with index i is the last vertex in the CurveSegent[i-1]s' list of vertices.
    /// </summary>
    public class LinearSegment : CurveSegment
    {
        /// <summary>
        /// Calculates a polyline, representing the curve segment.
        /// </summary>
        /// <param name="startPoint">The segments starting point</param>
        /// <param name="segmentsPerCurve">The number of segments per curve</param>
        /// <param name="degree">The degree of the curve: 1 for linear, 2 for conic, 3 for cubic</param>
        public override IEnumerable<float3> CalcUniformPolyline(float3 startPoint, int segmentsPerCurve, int degree)
        {
            var controlPoints = new List<float3> { startPoint };
            controlPoints.AddRange(Vertices);

            for (var i = 0; i < controlPoints.Count - degree; i += degree)
            {
                yield return controlPoints[i];

                var verts = new float3[degree + 1];

                for (var j = 0; j < verts.Length; j++)
                {
                    verts[j] = controlPoints[i + j];
                }

                for (var j = 1; j < segmentsPerCurve; j++)
                {
                    var t = j / (float)segmentsPerCurve;
                    yield return (1 - t) * verts[0] + t * verts[1];
                }
                yield return controlPoints[controlPoints.Count - 1];
            }
        }
    }

    /// <summary>
    /// Represents a conic beziér path of a CurvePart, using a list of float3.
    /// A CurveSgment does not know its own start point. For the first CurveSegment in a sequence the start point is saved in the CurvePart belonging to the segment.
    /// The start point for the CurveSegment with index i is the last vertex in the CurveSegent[i-1]s' list of vertices.
    /// </summary>
    public class BezierConicSegment : CurveSegment
    {

    }

    /// <summary>
    /// Represents a cubic beziér path of a CurvePart, using a list of float3.
    /// A CurveSgment does not know its own start point. For the first CurveSegment in a sequence the start point is saved in the CurvePart belonging to the segment.
    /// The start point for the CurveSegment with index i is the last vertex in the CurveSegent[i-1]s' list of vertices.
    /// </summary>
    public class BezierCubicSegment : CurveSegment
    {
       
    }

}
