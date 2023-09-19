using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Tapecorps.GameDevelopmentKit
{
    public static class GDK
    {

        public static bool Coinflip => UnityEngine.Random.Range(0, 2) == 0;
        public static System.Random Random = new System.Random();


#if UNITY_EDITOR
        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return a;
        }

#endif


        public static List<Type> RemoveEnums(List<Type> types)
        {
            types.RemoveAll(t => t.IsEnum);
            return types;
        }

        public static T RandomFromArray<T>(T[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }

        public static T RandomFromList<T>(List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

        public static UnityEngine.Object GetObjectByName<T>(List<T> list, string name)
        {
            foreach (T i in list)
            {
                if ((i as UnityEngine.Object).name == name)
                    return (i as UnityEngine.Object);
            }

            return null;
        }

        public static bool TryGetObjectByName<T>(List<T> list, string name, out T o)
        {
            o = default(T);
            foreach (T i in list)
            {
                if ((i as UnityEngine.Object).name == name)
                {
                    o = i;
                    return true;
                }
            }

            return false;
        }

        public static Transform FindParentByName(Transform target, string name)
        {
            if (target.parent != null)
            {
                if (target.parent.name == name)
                    return target.parent;
                else return FindParentByName(target.parent, name);
            }
            else return null;
        }

        public static string LimitStringLength(object obj, int Length)
        {
            string text = obj.ToString();

            string result = "";

            for (int i = 0; i < text.Length && i < Length; i++)
                result += text[i];

            return result;
        }

        public static Transform GetChildByName(Transform parent, string name, bool includeInactive = false)
        {
            Transform[] children = parent.GetComponentsInChildren<Transform>(includeInactive);

            foreach (Transform c in children)
                if (c.name.Trim() == name.Trim()) return c;

            return null;
        }

        public static bool TryGetChildByName(Transform parent, string name, out Transform result, bool includeInactive = false)
        {
            Transform[] children = parent.GetComponentsInChildren<Transform>(includeInactive);

            foreach (Transform c in children)
            {
                if (c.name.Trim() == name.Trim())
                {
                    result = c;
                    return true;
                }

            }

            result = null;
            return false;
        }
        public static bool TryGetChildByNameContains(Transform parent, string name, out Transform result, bool includeInactive = false)
        {
            Transform[] children = parent.GetComponentsInChildren<Transform>(includeInactive);

            foreach (Transform c in children)
            {
                if (c.name.Trim().Contains(name.Trim()))
                {
                    result = c;
                    return true;
                }

            }

            result = null;
            return false;
        }

        public static Transform GetChildByNameContains(Transform parent, string contains, bool includeInactive = false)
        {
            Transform[] children = parent.GetComponentsInChildren<Transform>(includeInactive);

            foreach (Transform c in children)
                if (c.name.Contains(contains)) return c;

            Debug.Log(contains + " not found. Total child count: " + children.Length);

            return null;
        }

        public static Transform[] GetChildrenByName(Transform parent, string name, bool includeInactive = false)
        {
            Transform[] children = parent.GetComponentsInChildren<Transform>(includeInactive);
            List<Transform> found = new List<Transform>();

            foreach (Transform c in children)
                if (c.name.Trim() == name.Trim()) found.Add(c);

            return found.ToArray();
        }

        public static Transform[] GetChildrenByNameContains(Transform parent, string contains, bool includeInactive = false)
        {
            Transform[] children = parent.GetComponentsInChildren<Transform>(includeInactive);
            List<Transform> found = new List<Transform>();

            foreach (Transform c in children)
                if (c.name.Contains(contains)) found.Add(c);

            return found.ToArray();
        }

        public static Vector3 LerpAngle(Vector3 from, Vector3 to, float time)
        {
            return new Vector3(Mathf.LerpAngle(from.x, to.x, time), Mathf.LerpAngle(from.y, to.y, time), Mathf.LerpAngle(from.z, to.z, time));
        }

        public static Vector2 LerpAngle(Vector2 from, Vector2 to, float time)
        {
            return new Vector2(Mathf.LerpAngle(from.x, to.x, time), Mathf.LerpAngle(from.y, to.y, time));
        }

        public static Rect SumRect(Rect rect1, Rect rect2)
        {
            return new Rect(rect1.position + rect2.position, rect1.size + rect2.size);
        }

        public static bool TryGetComponentInParent(ref Component component, GameObject target)
        {
            Type type = component.GetType();
            component = null;
            if (target.transform.parent == null)
            {
                return false;
            }
            else
            {
                component = target.GetComponentInParent(type);
                return component != null;
            }
        }

        /// <summary>Creates a deep copy of an object. Object must be Serializable.</summary>
        public static T CreateDeepCopy<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(ms);
            }
        }

        public static T Clone<T>(T source)
        {
            Type t = source.GetType();
            object result = Activator.CreateInstance(t);

            FieldInfo[] fields = t.GetFields();

            foreach (FieldInfo f in fields)
                f.SetValue(result, f.GetValue(source));

            return (T)result;
        }

        public static List<T> Shuffle<T>(this List<T> list)
        {
            int n = list.Count;
            System.Random r = new System.Random();
            while (n > 1)
            {
                n--;
                int k = r.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public static T[] Shuffle<T>(this T[] array)
        {
            int n = array.Length;
            List<T> list = array.ToList();
            System.Random r = new System.Random();
            while (n > 1)
            {
                n--;
                int k = r.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list.ToArray();
        }

        public static int GetRandomByRarity(object[] pool, int[] rarities, bool debug = false)
        {
            if (pool.Length != rarities.Length)
                return -1;

            int totalRarity = 0;

            foreach (int r in rarities)
                totalRarity += r;

            int random = UnityEngine.Random.Range(0, totalRarity);

            int targetIndex = 0;
            int last = 0;

            for (int j = 0; j < pool.Length; j++)
            {
                if (last + rarities[j] > random && random >= last)
                {
                    targetIndex = j;
                    break;
                }

                last += rarities[j];
            }

            return targetIndex;
        }

        public static string AddSpacesBeforeCapitals(string text)
        {
            string edited = "";

            foreach (char c in text)
            {
                if (c.ToString() == c.ToString().ToUpper())
                    edited += " ";
                edited += c;
            }

            return edited.Trim();
        }

        public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
        {
            var item = list[oldIndex];

            list.RemoveAt(oldIndex);

            if (newIndex > oldIndex) newIndex--;
        // the actual index could have shifted due to the removal

            list.Insert(newIndex, item);
        }

        public static void Move<T>(this List<T> list, T item, int newIndex)
        {
            if (item != null)
            {
                var oldIndex = list.IndexOf(item);
                if (oldIndex > -1)
                {
                    list.RemoveAt(oldIndex);

                    if (newIndex > oldIndex) newIndex--;
                // the actual index could have shifted due to the removal

                    list.Insert(newIndex, item);
                }
            }
        }


        public static string Print2DArrayAsTable(string[,] arr)
        {
            int rows = arr.GetLength(0); // Get the number of rows
            int cols = arr.GetLength(1); // Get the number of columns

            string table = "2D Array:\n";

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    table += arr[i, j].ToString().PadLeft(15) + " "; // Adjust padding as needed
                }
                table += "\n";
            }

            return table;
        }

    }

    public static class Shape
    {
        public static Vector2[] GenerateHexagonalPositions(int layerCount, float radius = 1f, float width = 1f)
        {
            List<Vector2> positions = new List<Vector2>();

            float xOffset = width * 3.0f / 2.0f;
            float zOffset = radius * Mathf.Sqrt(3.0f);

            for (int i = 0; i < layerCount; i++)
            {
                for (int q = -i; q <= i; q++)
                {
                    for (int r = Mathf.Max(-i, -q - i); r <= Mathf.Min(i, -q + i); r++)
                    {
                        float xPos = q * xOffset;
                        float yPos = r * zOffset + q * zOffset / 2.0f;
                        positions.Add(new Vector2(xPos, yPos));
                    }
                }
            }

            return positions.ToArray();
        }
        public static Vector3[] Heart(float scale)
        {
            List<Vector3> locations = new List<Vector3>();

            float radius = 2f * scale;
            int edges = 20;
            float angle = 250f / edges;


            for (int i = 0; i < edges; i++)
            {
                float x = Mathf.Cos(Mathf.Deg2Rad * (i * angle)) * radius;
                float y = Mathf.Sin(Mathf.Deg2Rad * (i * angle)) * radius;

                locations.Add(new Vector3(-x + radius, y, 0));
            }


            for (int i = 1; i < edges; i++)
            {
                float x = Mathf.Cos(Mathf.Deg2Rad * (i * angle)) * radius;
                float y = Mathf.Sin(Mathf.Deg2Rad * (i * angle)) * radius;
                locations.Add(new Vector3(x - radius, y, 0));
            }

            Vector3 first = locations[locations.Count - 1];
            Vector3 second = locations[locations.Count - 2];

            Vector3 delta = first - second;

            Vector3 current = first;

            int iteration = 0;

            while (current.x < 0 && iteration < 10)
            {
                locations.Add(current);
                locations.Add(new Vector3(-current.x, current.y));
                current += delta;
            }

            return locations.ToArray();
        }
    }

    public static class Calculations
    {
        public static int GCD(int[] numbers)
        {
            return numbers.Aggregate(GCD);
        }

        public static int GCD(int a, int b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        public static float Percentage(float main, float percentage)
        {
            return (percentage / 100f) * main;
        }

        public static float Percentage(string main, float percentage)
        {
            return (percentage / 100f) * float.Parse(main);
        }

        public static float Percentage(string main, string percentage)
        {
            return (float.Parse(percentage) / 100f) * float.Parse(main);
        }

        public static float Percentage(float main, string percentage)
        {
            return (float.Parse(percentage) / 100f) * main;
        }

        public static int PercentageInt(float main, float percentage)
        {
            return Mathf.RoundToInt((percentage / 100f) * main);
        }

        public static int PercentageInt(string main, float percentage)
        {
            return Mathf.RoundToInt((percentage / 100f) * float.Parse(main));
        }

        public static int PercentageInt(string main, string percentage)
        {
            return Mathf.RoundToInt((float.Parse(percentage) / 100f) * float.Parse(main));
        }

        public static int PercentageInt(float main, string percentage)
        {
            return Mathf.RoundToInt((float.Parse(percentage) / 100f) * main);
        }



        public static int TimeVectorToTick(Vector3Int timeVector)
        {
            timeVector.y += timeVector.x * 60;
            timeVector.z += timeVector.y * 60;
            return timeVector.z;
        }

    }

    public static class Geometry
    {
        public static float Angle2D(Vector2 from, Vector2 to)
        {
            return (Mathf.Rad2Deg * (Mathf.Atan2(from.y - to.y, from.x - to.x)) + 180);
        }

        public static Vector2 TangentLimiter(float angle, Vector2 limit)
        {
            Vector2 result = Vector2.zero;
            float x = 0;
            float y = 0;

            float _ang = angle % 360;
            angle = _ang;

            if (_ang < 0)
                angle = 360 + _ang;

            float a = limit.x;
            float b = limit.y;
            float c = Mathf.Sqrt(limit.x * limit.x + limit.y * limit.y);

            float aAngle = Mathf.Acos(a / c) * Mathf.Rad2Deg;
            float bAngle = 90 - aAngle;

            if (angle >= aAngle && angle < aAngle + bAngle * 2)
            {
                x = b * Mathf.Tan((bAngle - (angle - aAngle)) * Mathf.Deg2Rad);
                y = b;
            }
            else if (angle >= aAngle + bAngle * 2 && angle < aAngle * 3 + bAngle * 2)
            {
                x = -a;
                y = a * Mathf.Tan((aAngle - (angle - bAngle * 2 - aAngle)) * Mathf.Deg2Rad);
            }
            else if (angle >= aAngle * 3 + bAngle * 2 && angle < aAngle * 3 + bAngle * 4)
            {
                x = b * Mathf.Tan((-bAngle + (angle - bAngle * 2 - aAngle * 3)) * Mathf.Deg2Rad);
                y = -b;
            }
            else
            {
                x = a;
                y = a * Mathf.Tan(angle * Mathf.Deg2Rad);
            }

            result = new Vector2(x, y);

            return result;
        }


        public static Vector3 SquareToIsometric(Vector3 position, Vector2 xDisplacement, Vector2 yDisplacement)
        {
            return (position.x * xDisplacement) + (position.y * yDisplacement);
        }

        public static float CalculateIsometricAngle(Vector2 isometricScale)
        {
            float a = isometricScale.y / 2;
            float b = isometricScale.x / 2;

            float c = Mathf.Sqrt(a * a + b * b);

            return Mathf.Asin(a / c) * Mathf.Rad2Deg;
        }
    }

    public static class Imaging
    {
        public static Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
        {
            RenderTexture rt = new RenderTexture(targetX, targetY, 24);
            RenderTexture.active = rt;
            Graphics.Blit(texture2D, rt);
            Texture2D result = new Texture2D(targetX, targetY);
            result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
            result.Apply();
            return result;
        }

        public static Texture2D PickFromTexture2D(Texture2D original, int sizeX, int sizeY, int offsetX = 0, int offsetY = 0)
        {
            Texture2D result = new Texture2D(sizeX, sizeY);

            for (int y = 0; y < sizeY; y++)
                for (int x = 0; x < sizeX; x++)
                    result.SetPixel(x, y, original.GetPixel(offsetX + x, offsetY + y));

            result.Apply();

            return result;
        }

        public static Texture2D MultiplyAlpha(Texture2D original, float m)
        {
            Texture2D result = new Texture2D(original.width, original.height);

            for (int y = 0; y < result.height; y++)
            {

                for (int x = 0; x < result.width; x++)
                {
                    Color c = original.GetPixel(x, y);
                    c.a *= m;
                    result.SetPixel(x, y, c);
                }
            }

            result.Apply();

            return result;
        }

        //by: mathiassoeholm from: https://discussions.unity.com/t/yield-waitforseconds-outside-of-time-scale/49229
        public static class CoroutineUtil
        {
            public static IEnumerator WaitForRealSeconds(float time)
            {
                float start = Time.realtimeSinceStartup;
                while (Time.realtimeSinceStartup < start + time)
                {
                    yield return null;
                }
            }
        }

        public static Texture2D AddOnTexture2D(Texture2D a, Texture2D b, int offsetX, int offsetY)
        {
            offsetY = -offsetY;
            int width = Mathf.Max(a.width, b.width + Mathf.Abs(offsetX));
            int height = Mathf.Max(a.height, b.height + Mathf.Abs(offsetY));

            Vector2Int aOffset = Vector2Int.zero;

            if (offsetX < 0)
            {
                aOffset.x = -offsetX;
                offsetX = 0;
            }

            if (offsetY < 0)
            {
                aOffset.y = -offsetY;
                offsetY = 0;
            }

            Texture2D result = new Texture2D(width, height);

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    result.SetPixel(x, y, new Color(0, 0, 0, 0));

            for (int y = 0; y < a.height; y++)
                for (int x = 0; x < a.width; x++)
                    result.SetPixel(x + aOffset.x, y + aOffset.y, a.GetPixel(x, y));

            for (int y = 0; y < b.height; y++)
            {
                for (int x = 0; x < b.width; x++)
                {
                    Color bc = b.GetPixel(x, y);
                    Color ac = result.GetPixel(x + offsetX, y + offsetY);
                    result.SetPixel(x + offsetX, y + offsetY, Color.Lerp(ac, bc, bc.a));
                }
            }

            result.Apply();
            return result;
        }

        public static Texture2D CopyTexture2D(Texture2D original)
        {
            Texture2D result = new Texture2D(original.width, original.height);
            result.SetPixels(original.GetPixels());
            result.Apply();
            return result;
        }
    }

    public class Logic
    {
        public static bool All(bool[] conditions, bool state)
        {
            bool result = true;

            foreach (bool c in conditions)
            {
                if (c != state)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        public static bool Between(float value, float a, float b, bool inclusive = false)
        {
            float max = Mathf.Max(a, b);
            float min = Mathf.Min(a, b);

            return inclusive ? (value <= max && min <= value) : (value < max && min < value);
        }

    }

    public class Interval
    {
        public float Timeout
        {
            get; private set;
        }
        private float Last = 0f;
        public bool Available
        {
            get
            {
                return !Check();
            }
        }

        public Interval(float timeout, bool start = false)
        {
            Timeout = timeout;
            Last = (start) ? UnityEngine.Time.timeSinceLevelLoad + timeout : UnityEngine.Time.timeSinceLevelLoad;
        }

        public void ChangeTimeout(float timeout)
        {
            Timeout = timeout;
        }

        public bool Check()
        {
            if (Last < UnityEngine.Time.timeSinceLevelLoad)
            {
                Last = UnityEngine.Time.timeSinceLevelLoad + Timeout;
                return true;
            }

            return false;
        }

        public float GetLast()
        {
            return Last;
        }
    }
}
