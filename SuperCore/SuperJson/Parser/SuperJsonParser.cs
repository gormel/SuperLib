using System.Collections.Generic;
using System.Linq;
using SuperJson.Objects;

namespace SuperJson.Parser
{
    public class SuperJsonParser
    {
        private struct StrinToTokenPair
        {
            public string String { get; set; }
            public SuperToken Token { get; set; }

            public StrinToTokenPair(string s, SuperToken token)
            {
                String = s;
                Token = token;
            }
        }

        public string Write(SuperToken token)
        {
            switch (token.TokenType)
            {
                case SuperTokenType.Number:
                case SuperTokenType.Bool:
                    return $"{token.Value}";
                case SuperTokenType.String:
                    return $"\"{token.Value}\"";
                case SuperTokenType.Array:
                    var arrResult = "[";
                    foreach (var el in ((SuperArray)token).TypedValue)
                    {
                        arrResult += Write(el) + ",";
                    }
                    arrResult = arrResult.TrimEnd(',') + "]";
                    return arrResult;
                case SuperTokenType.Object:
                    var objResult = "{";
                    foreach (var prop in ((SuperObject)token).TypedValue)
                    {
                        objResult += $"\"{prop.Key}\":{Write(prop.Value)},";
                    }
                    return objResult.TrimEnd(',') + "}";
                case SuperTokenType.Null:
                    return "Null";
            }
            return null;
        }

        public SuperToken Parse(string rawData)
        {
            int pos = 0;
            return ParseToken(rawData, ref pos);
        }

        private SuperToken ParseToken(string data, ref int pos)
        {
            var savedPos = pos;

            var tryNumber = ParseNumber(data, ref pos);
            if (tryNumber != null)
                return tryNumber;

            pos = savedPos;

            var tryString = ParseString(data, ref pos);
            if (tryString != null)
                return tryString;

            pos = savedPos;

            var tryArray = ParseArray(data, ref pos);
            if (tryArray != null)
                return tryArray;

            pos = savedPos;

            var tryObject = ParseObject(data, ref pos);
            if (tryObject != null)
                return tryObject;

            pos = savedPos;

            var tryBool = ParseBool(data, ref pos);
            if (tryBool != null)
                return tryBool;

            pos = savedPos;

            var tryNull = ParseNull(data, ref pos);
            if (tryNull != null)
                return tryNull;

            return null;
        }

        private SuperNumber ParseNumber(string data, ref int pos)
        {
            if (pos >= data.Length || !char.IsDigit(data[pos]) && data[pos] != '-' && data[pos] != '+')
                return null;

            var parsed = "";

            if (data[pos] == '-' || data[pos] == '+')
                parsed += data[pos++];

            var seq = ParseDigitSequence(data, ref pos);
            if (seq == null)
                return null;
            parsed += seq;

            if (pos >= data.Length)
                return new SuperNumber(double.Parse(parsed));

            if (data[pos] == '.')
            {
                parsed += data[pos++];
                seq = ParseDigitSequence(data, ref pos);
                if (seq == null)
                    return null;
                parsed += seq;
            }

            if (pos >= data.Length)
                return new SuperNumber(double.Parse(parsed));

            if (data[pos] == 'e' || data[pos] == 'E')
            {
                parsed += data[pos++];
                if (pos >= data.Length)
                    return null;
                if (data[pos] == '-' || data[pos] == '+')
                    parsed += data[pos++];
                seq = ParseDigitSequence(data, ref pos);
                if (seq == null)
                    return null;
                parsed += seq;
            }

            return new SuperNumber(double.Parse(parsed));
        }

        private string ParseDigitSequence(string data, ref int pos)
        {
            if (pos >= data.Length || !char.IsDigit(data[pos]))
                return null;

            var result = "";
            while (pos < data.Length && char.IsDigit(data[pos]))
            {
                result += data[pos++];
            }
            return result;
        }

        private SuperString ParseString(string data, ref int pos)
        {
            if (pos >= data.Length || data[pos] != '"')
                return null;
            pos++;

            var parsed = "";
            while (pos < data.Length && data[pos] != '"')
            {
                parsed += data[pos++];
            }

            if (pos >= data.Length)
                return null;
            pos++;

            return new SuperString(parsed);
        }

        private SuperArray ParseArray(string data, ref int pos)
        {
            if (pos >= data.Length || data[pos] != '[')
                return null;

            SuperArray result;

            pos++;

            SkipSpaces(data, ref pos);

            var elems = ParseArrayElems(data, ref pos);
            if (elems == null)
                result = new SuperArray(new SuperToken[0]);
            else
                result = new SuperArray(elems.ToArray());

            SkipSpaces(data, ref pos);

            if (pos >= data.Length || data[pos] != ']')
                return null;

            pos++;

            return result;
        }

        private List<SuperToken> ParseArrayElems(string data, ref int pos)
        {
            if (pos >= data.Length)
                return null;

            var head = ParseToken(data, ref pos);

            if (head == null)
                return null;

            SkipSpaces(data, ref pos);

            if (data[pos] != ',')
                return new List<SuperToken>() {head};
            pos++;

            SkipSpaces(data, ref pos);

            var tail = ParseArrayElems(data, ref pos);

            if (tail == null)
                return null;

            return new[] {head}.Concat(tail).ToList();
        }

        private void SkipSpaces(string data, ref int pos)
        {
            if (pos >= data.Length)
                return;

            while (char.IsWhiteSpace(data[pos]))
            {
                pos++;
            }
        }

        private SuperObject ParseObject(string data, ref int pos)
        {
            if (pos >= data.Length || data[pos] != '{')
                return null;

            pos++;

            SkipSpaces(data, ref pos);

            SuperObject result = new SuperObject();

            var props = ParseObjectPropsList(data, ref pos);

            if (props == null)
                result.Value = new Dictionary<string, SuperObject>();
            else
                result.Value = props.ToDictionary(t => t.String, t => t.Token);

            SkipSpaces(data, ref pos);

            if (pos >= data.Length || data[pos] != '}')
                return null;

            pos++;

            return result;
        }

        private List<StrinToTokenPair> ParseObjectPropsList(string data, ref int pos)
        {
            if (pos >= data.Length)
                return null;

            var propName = ParseString(data, ref pos);
            if (propName == null)
                return null;

            SkipSpaces(data, ref pos);

            if (data[pos] != ':')
                return null;
            pos++;

            SkipSpaces(data, ref pos);

            var propValue = ParseToken(data, ref pos);

            if (propValue == null)
                return null;

            SkipSpaces(data, ref pos);

            if (pos >= data.Length || data[pos] != ',')
                return new List<StrinToTokenPair>() { new StrinToTokenPair(propName.TypedValue, propValue) };

            pos++;

            SkipSpaces(data, ref pos);

            var tail = ParseObjectPropsList(data, ref pos);
            if (tail == null)
                return null;

            return new[] { new StrinToTokenPair(propName.TypedValue, propValue) }.Concat(tail).ToList();
        }

        private SuperBool ParseBool(string data, ref int pos)
        {
            if (pos >= data.Length)
                return null;

            if (data.Substring(pos).StartsWith("True"))
            {
                pos += 4;
                return new SuperBool(true);
            }

            if (data.Substring(pos).StartsWith("False"))
            {
                pos += 5;
                return new SuperBool(false);
            }

            return null;
        }

        private SuperNull ParseNull(string data, ref int pos)
        {
            if (pos >= data.Length)
                return null;

            if (data.Substring(pos).StartsWith("Null"))
            {
                pos += 4;
                return new SuperNull();
            }

            return null;
        }
    }
}
