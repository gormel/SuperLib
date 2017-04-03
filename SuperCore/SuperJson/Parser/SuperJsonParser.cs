using System.Collections.Generic;
using System.Globalization;
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
                    return ((SuperNumber) token).TypedValue.ToString(CultureInfo.InvariantCulture);
                case SuperTokenType.Bool:
                    return $"{token.Value.ToString().ToLower()}";
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
                    return "null";
            }
            return null;
        }

        public SuperToken Parse(string rawData)
        {
            int pos = 0;
            var result = ParseToken(rawData, ref pos, 0);
            SkipSpaces(rawData, ref pos);
            if (pos < rawData.Length)
                return null;
            return result;
        }
        
        private SuperToken ParseToken(string data, ref int pos, int deep)
        {
            SkipSpaces(data, ref pos);

            if (pos >= data.Length)
                return null;

            if (deep > 1000)
                return null;

            switch (data[pos])
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '-':
                    return ParseNumber(data, ref pos);
                case '"':
                    return ParseString(data, ref pos);
                case '[':
                    return ParseArray(data, ref pos, deep + 1);
                case '{':
                    return ParseObject(data, ref pos, deep + 1);
                case 't':
                case 'f':
                    return ParseBool(data, ref pos);
                case 'n':
                    return ParseNull(data, ref pos);
            }

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
            if (seq == null || seq.Length > 1 && seq[0] == '0')
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
            {
                double result;
                if (double.TryParse(parsed, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                    return new SuperNumber(result);
                return null;
            }

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

            double value;
            if (double.TryParse(parsed, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                return new SuperNumber(value);
            return null;
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
                var symbol = data[pos++];
                if (symbol >= 0 && symbol <= 0x1f) //is control symbol
                    return null;
                if (symbol == '\\')
                {
                    if (pos >= data.Length)
                        return null;
                    symbol = data[pos++];
                    switch (symbol)
                    {
                        case 'u':
                            if (pos + 3 >= data.Length)
                                return null;
                            var num = data.Substring(pos, 4);
                            int sym;
                            if (!int.TryParse(num, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out sym))
                                return null;
                            symbol = (char)sym;
                            break;
                        case '"':
                        case '\\':
                        case '/':
                            break;
                        case 'b':
                            symbol = '\b';
                            break;
                        case 'f':
                            symbol = '\f';
                            break;
                        case 'n':
                            symbol = '\n';
                            break;
                        case 'r':
                            symbol = '\r';
                            break;
                        case 't':
                            symbol = '\t';
                            break;
                        default:
                            return null;
                    }
                }

                parsed += symbol;
            }

            if (pos >= data.Length)
                return null;
            pos++;

            return new SuperString(parsed);
        }

        private SuperArray ParseArray(string data, ref int pos, int deep)
        {
            if (pos >= data.Length || data[pos] != '[')
                return null;

            pos++;

            SkipSpaces(data, ref pos);

            if (pos >= data.Length)
                return null;

            if (data[pos] == ']')
            {
                pos++;
                return new SuperArray(new SuperToken[0]);
            }

            var elems = ParseArrayElems(data, ref pos, deep + 1);
            if (elems == null)
                return null;

            var result = new SuperArray(elems.ToArray());

            SkipSpaces(data, ref pos);

            if (pos >= data.Length || data[pos] != ']')
                return null;

            pos++;

            return result;
        }

        private List<SuperToken> ParseArrayElems(string data, ref int pos, int deep)
        {
            if (deep > 10000)
                return null;
            
            if (pos >= data.Length)
                return null;
            
            var head = ParseToken(data, ref pos, deep + 1);

            if (head == null)
                return null;

            SkipSpaces(data, ref pos);

            if (pos >= data.Length)
                return null;

            if (data[pos] != ',')
                return new List<SuperToken>() {head};
            pos++;

            SkipSpaces(data, ref pos);

            var tail = ParseArrayElems(data, ref pos, deep + 1);

            if (tail == null || tail.Count < 1)
                return null;

            return new[] {head}.Concat(tail).ToList();
        }

        private void SkipSpaces(string data, ref int pos)
        {
            if (pos >= data.Length)
                return;

            while (data[pos] == ' ' || data[pos] == '\t' ||
                data[pos] == '\n' || data[pos] == '\r')
            {
                pos++;

                if (pos >= data.Length)
                    return;
            }
        }

        private SuperObject ParseObject(string data, ref int pos, int deep)
        {
            if (pos >= data.Length || data[pos] != '{')
                return null;

            pos++;

            SkipSpaces(data, ref pos);

            if (pos >= data.Length)
                return null;

            if (data[pos] == '}')
            {
                pos++;
                return new SuperObject();
            }

            SuperObject result = new SuperObject();

            var props = ParseObjectPropsList(data, ref pos, deep + 1);

            if (props == null)
                return null;

            result.Value = new Dictionary<string, SuperToken>();

            foreach (var pair in props)
            {
                result.TypedValue[pair.String] = pair.Token;
            }

            SkipSpaces(data, ref pos);

            if (pos >= data.Length || data[pos] != '}')
                return null;

            pos++;

            return result;
        }

        private List<StrinToTokenPair> ParseObjectPropsList(string data, ref int pos, int deep)
        {
            if (deep > 10000)
                return null;

            if (pos >= data.Length)
                return null;

            var propName = ParseString(data, ref pos);
            if (propName == null)
                return null;

            SkipSpaces(data, ref pos);

            if (pos >= data.Length)
                return null;

            if (data[pos] != ':')
                return null;
            pos++;

            SkipSpaces(data, ref pos);

            if (pos >= data.Length)
                return null;

            var propValue = ParseToken(data, ref pos, deep + 1);

            if (propValue == null)
                return null;

            SkipSpaces(data, ref pos);

            if (pos >= data.Length || data[pos] != ',')
                return new List<StrinToTokenPair>() { new StrinToTokenPair(propName.TypedValue, propValue) };

            pos++;

            SkipSpaces(data, ref pos);

            var tail = ParseObjectPropsList(data, ref pos, deep + 1);
            if (tail == null)
                return null;

            return new[] { new StrinToTokenPair(propName.TypedValue, propValue) }.Concat(tail).ToList();
        }

        private SuperBool ParseBool(string data, ref int pos)
        {
            if (pos >= data.Length)
                return null;

            if (data.Substring(pos).StartsWith("true"))
            {
                pos += 4;
                return new SuperBool(true);
            }

            if (data.Substring(pos).StartsWith("false"))
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

            if (data.Substring(pos).StartsWith("null"))
            {
                pos += 4;
                return new SuperNull();
            }

            return null;
        }
    }
}
