using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtil
{
    public struct Bezier2Config
    {
        private Vector3 m_Start;
        private Vector3 m_Control;
        private Vector3 m_End;

        public Bezier2Config(Vector3 start, Vector3 control, Vector3 end)
        {
            m_Start = start;
            m_Control = control;
            m_End = end;
        }

        public Vector3 start { get { return m_Start; } }
        public Vector3 control { get { return m_Control; } }
        public Vector3 end { get { return m_End; } }
    }
    public struct Bezier3Config
    {
        private Vector3 m_Start;
        private Vector3 m_Control1;
        private Vector3 m_Control2;
        private Vector3 m_End;

        public Bezier3Config(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end)
        {
            m_Start = start;
            m_Control1 = control1;
            m_Control2 = control2;
            m_End = end;
        }

        public Vector3 start { get { return m_Start; } }
        public Vector3 control1 { get { return m_Control1; } }
        public Vector3 control2 { get { return m_Control2; } }
        public Vector3 end { get { return m_End; } }
    }

    public class UtilFunc
    {
        public static float MoveValue_float(float Src, float Dst, float MoveLen)
        {
            if (Dst > Src)
            {
                Src += MoveLen;
                if (Dst < Src)
                    Src = Dst;
            }
            else
            {
                Src -= MoveLen;
                if (Dst > Src)
                    Src = Dst;
            }

            return Src;
        }

        public static void MoveValue_float(ref float Src, float Dst, float MoveLen)
        {
            if (Dst > Src)
            {
                Src += MoveLen;
                if (Dst < Src)
                    Src = Dst;
            }
            else
            {
                Src -= MoveLen;
                if (Dst > Src)
                    Src = Dst;
            }
        }

        public static float CheckRange_float(float value, float range)
        {
            if (Mathf.Abs(value) < range)
                return value;
            bool isMinus = false;
            if (value < 0)
                isMinus = true;
            value = range;
            if (isMinus)
                value *= -1;
            return value;
        }

        public static float GetRepeatSin(float val, int RepeatCount)
        {
            RepeatCount--;
            val = Mathf.Sin(val);
            if (RepeatCount <= 0)
                return val;
            return GetRepeatSin(val * (Mathf.PI / 2), RepeatCount);
        }

        public static Vector3 Lerp(Vector3 from, Vector3 to, float t)
        {
            Vector3 r = new Vector3();
            r.x = Mathf.Lerp(from.x, to.x, t);
            r.y = Mathf.Lerp(from.y, to.y, t);
            r.z = Mathf.Lerp(from.z, to.z, t);
            return r;
        }

        public static void Shuffle<T>(ref List<T> arr)
        {
            if (arr.Count < 1)
                return;

            int ranIdx = 0;
            List<T> copy = new List<T>();

            while (arr.Count > 0)
            {
                ranIdx = Random.Range(0, arr.Count);
                copy.Add(arr[ranIdx]);
                arr.RemoveAt(ranIdx);
            }

            arr = new List<T>(copy);
        }

        public static List<T> Shuffle<T>(List<T> arr)
        {
            if (arr.Count < 1)
                return null;

            int ranIdx = 0;
            List<T> t = new List<T>(arr);
            List<T> copy = new List<T>();

            while (t.Count > 0)
            {
                ranIdx = Random.Range(0, t.Count);
                copy.Add(t[ranIdx]);
                t.RemoveAt(ranIdx);
            }

            return copy;
        }

        public static List<T> Shuffle<T>(List<T> arr, int cutLength)
        {
            if (arr.Count < 1 || cutLength < 1 || arr.Count < cutLength)
                return null;

            int ranIdx = 0;

            List<T> t = new List<T>(arr);
            List<T> copy = new List<T>();

            while (copy.Count < cutLength)
            {
                ranIdx = Random.Range(0, t.Count);
                copy.Add(t[ranIdx]);
                t.RemoveAt(ranIdx);
            }

            return copy;
        }

        public static T[] Shuffle<T>(T[] array)
        {
            List<T> tmp = new List<T>();
            tmp.AddRange(array);
            Shuffle<T>(ref tmp);
            return tmp.ToArray();
        }

        public static void Shuffle<T>(ref T[] array)
        {
            List<T> tmp = new List<T>();
            tmp.AddRange(array);
            Shuffle<T>(ref tmp);
            array = tmp.ToArray();
        }

        public static Vector3 Bezier(Bezier2Config bezier, float t)
        {
            float b = 1 - t;

            float b1 = b * b;
            float b2 = 2 * t * b;
            float b3 = t * t;

            return (b1 * bezier.start) + (b2 * bezier.control) + (b3 * bezier.end);
        }

        public static Vector3 Bezier(Vector3 s, Vector3 c, Vector3 e, float t)
        {
            float b = 1 - t;

            float b1 = b * b;
            float b2 = 2 * t * b;
            float b3 = t * t;

            return (b1 * s) + (b2 * c) + (b3 * e);
        }

        public static Vector3 Bezier_position(GameObject s, GameObject c, GameObject e, float t)
        {
            if (s == null || c == null || e == null)
                return Vector3.zero;

            float b = 1 - t;

            float b1 = b * b;
            float b2 = 2 * t * b;
            float b3 = t * t;

            return (b1 * s.transform.position) + (b2 * c.transform.position) + (b3 * e.transform.position);
        }

        public static Vector3 Bezier_localPosition(GameObject s, GameObject c, GameObject e, float t)
        {
            if (s == null || c == null || e == null)
                return Vector3.zero;

            float b = 1 - t;

            float b1 = b * b;
            float b2 = 2 * t * b;
            float b3 = t * t;

            return (b1 * s.transform.localPosition) + (b2 * c.transform.localPosition) + (b3 * e.transform.localPosition);
        }

        public static Vector3 Bezier(Bezier3Config bezier, float t)
        {
            float b = 1 - t;

            float b1 = b * b * b;
            float b2 = 3 * t * b * b;
            float b3 = 3 * t * t * b;
            float b4 = t * t * t;

            return (b1 * bezier.start) + (b2 * bezier.control1) + (b3 * bezier.control2) + (b4 * bezier.end);
        }

        public static Vector3 Bezier(Vector3 s, Vector3 c1, Vector3 c2, Vector3 e, float t)
        {
            float b = 1 - t;

            float b1 = b * b * b;
            float b2 = 3 * t * b * b;
            float b3 = 3 * t * t * b;
            float b4 = t * t * t;

            return (b1 * s) + (b2 * c1) + (b3 * c2) + (b4 * e);
        }

        public static Vector3 Bezier_position(GameObject s, GameObject c1, GameObject c2, GameObject e, float t)
        {
            if (s == null || c1 == null || c2 == null || e == null)
                return Vector3.zero;

            float b = 1 - t;

            float b1 = b * b * b;
            float b2 = 3 * t * b * b;
            float b3 = 3 * t * t * b;
            float b4 = t * t * t;

            return (b1 * s.transform.position) + (b2 * c1.transform.position) + (b3 * c2.transform.position) + (b4 * e.transform.position);
        }

        public static Vector3 Bezier_localPosition(GameObject s, GameObject c1, GameObject c2, GameObject e, float t)
        {
            if (s == null || c1 == null || c2 == null || e == null)
                return Vector3.zero;

            float b = 1 - t;

            float b1 = b * b * b;
            float b2 = 3 * t * b * b;
            float b3 = 3 * t * t * b;
            float b4 = t * t * t;

            return (b1 * s.transform.localPosition) + (b2 * c1.transform.localPosition) + (b3 * c2.transform.localPosition) + (b4 * e.transform.localPosition);
        }


        public static Vector3 CatmullRom(GameObject p0, GameObject p1, GameObject p2, GameObject p3, float i)
        {
            return CatmullRom(p0.transform.position, p1.transform.position, p2.transform.position, p3.transform.position, i);
        }

        public static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float i)
        {
            // comments are no use here... it's the catmull-rom equation.
            // Un-magic this, lord vector!
            return 0.5f *
                ((2 * p1) + (-p0 + p2) * i + (2 * p0 - 5 * p1 + 4 * p2 - p3) * i * i +
                 (-p0 + 3 * p1 - 3 * p2 + p3) * i * i * i);
        }
    }
}
