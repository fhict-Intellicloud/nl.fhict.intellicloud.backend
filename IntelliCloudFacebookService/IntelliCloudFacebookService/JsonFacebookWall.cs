using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliCloudFacebookService
{
    class JsonFacebookWall : DynamicObject, IEnumerable, IEnumerator
    {
        object _object;

            JsonFacebookWall(object jObject)
            {
                this._object = jObject;
            }

            public static dynamic GetDynamicJsonObject(byte[] buf)
            {
                return GetDynamicJsonObject(buf, Encoding.UTF8);
            }

            public static dynamic GetDynamicJsonObject(byte[] buf, Encoding encoding)
            {
                return GetDynamicJsonObject(encoding.GetString(buf));
            }

            public static dynamic GetDynamicJsonObject(string json)
            {
                object o = JsonConvert.DeserializeObject(json);
                return new JsonFacebookWall(o);
            }

            internal static dynamic GetDynamicJsonObject(JObject jObj)
            {
                return new JsonFacebookWall(jObj);
            }

            public object this[string s]
            {
                get
                {
                    JObject jObject = _object as JObject;
                    object obj = jObject.SelectToken(s);
                    if (obj == null) return true;

                    if (obj is JValue)
                        return GetValue(obj);
                    else
                        return new JsonFacebookWall(obj);
                }
            }

            public object this[int i]
            {
                get
                {
                    if (!(_object is JArray)) return null;

                    object obj = (_object as JArray)[i];
                    if (obj is JValue)
                    {
                        return GetValue(obj);
                    }
                    return new JsonFacebookWall(obj);
                }
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = null;

                if (_object is JArray)
                {
                    JArray jArray = _object as JArray;
                    switch (binder.Name)
                    {
                        case "Length":
                        case "Count": result = jArray.Count; break;
                        case "ToList": result = (Func<List<string>>)(() => jArray.Values().Select(x => x.ToString()).ToList()); break;
                        case "ToArray": result = (Func<string[]>)(() => jArray.Values().Select(x => x.ToString()).ToArray()); break;
                    }

                    return true;
                }

                JObject jObject = _object as JObject;
                object obj = jObject.SelectToken(binder.Name);
                if (obj == null) return true;

                if (obj is JValue)
                    result = GetValue(obj);
                else
                    result = new JsonFacebookWall(obj);

                return true;
            }

            object GetValue(object obj)
            {
                string val = ((JValue)obj).ToString();

                int resInt; double resDouble; DateTime resDateTime;

                if (int.TryParse(val, out resInt)) return resInt;
                if (DateTime.TryParse(val, out resDateTime)) return resDateTime;
                if (double.TryParse(val, out resDouble)) return resDouble;

                return val;
            }

            public override string ToString()
            {
                return _object.ToString();
            }

            int _index = -1;

            public IEnumerator GetEnumerator()
            {
                _index = -1;
                return this;
            }

            public object Current
            {
                get
                {
                    if (!(_object is JArray)) return null;
                    object obj = (_object as JArray)[_index];
                    if (obj is JValue) return GetValue(obj);
                    return new JsonFacebookWall(obj);
                }
            }

            public bool MoveNext()
            {
                if (!(_object is JArray)) return false;
                _index++;
                return _index < (_object as JArray).Count;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
    }
}
