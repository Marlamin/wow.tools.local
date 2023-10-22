using System.Text;

namespace wow.tools.local.Services
{
    public class WSExpressionParser
    {
        private int offset = 0;
        private byte[] bytes = null;
        public Dictionary<int, Dictionary<string, object>> state = new Dictionary<int, Dictionary<string, object>>();

        public WSExpressionParser(string hexBytes)
        {
            bytes = Convert.FromHexString(hexBytes);
            bool enabled = BitConverter.ToBoolean(bytes, offset++);
            if (enabled)
            {
                state[0] = EvalLogicalExp();
            }

            state = state.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        private Dictionary<string, object> EvalLogicalExp()
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            ret["relational"] = EvalRelationalExp();
            ret["op"] = bytes[offset++];
            byte op = Convert.ToByte(ret["op"]);
            int i = 1;
            while (op != 0)
            {
                state[i] = new Dictionary<string, object>();
                state[i]["relational"] = EvalRelationalExp();
                if (offset >= bytes.Length)
                {
                    state[i]["op"] = 0;
                }
                else
                {
                    state[i]["op"] = bytes[offset++];
                }
                op = Convert.ToByte(state[i]["op"]);
                i++;
            }
            return ret;
        }

        private Dictionary<string, object> EvalRelationalExp()
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            ret["arethmatic"] = EvalArethmaticExp();
            ret["op"] = bytes[offset++];
            byte op = Convert.ToByte(ret["op"]);
            if (op != 0)
            {
                ret["subArethmatic"] = EvalArethmaticExp();
            }
            return ret;
        }

        private Dictionary<string, object> EvalArethmaticExp()
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            ret["value"] = EvalValue();
            ret["op"] = bytes[offset++];
            byte op = Convert.ToByte(ret["op"]);
            if (op != 0)
            {
                ret["subValue"] = EvalValue();
            }
            return ret;
        }
        private Dictionary<string, object> EvalValue()
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            ret["type"] = bytes[offset++];
            byte type = Convert.ToByte(ret["type"]);
            switch (type)
            {
                case 0:
                    ret["value"] = 0;
                    break;
                case 1:
                case 2:
                    ret["value"] = BitConverter.ToInt32(bytes, offset);
                    offset += 4;
                    break;
                case 3:
                    ret["function"] = BitConverter.ToInt32(bytes, offset);
                    offset += 4;
                    ret["functionArg1"] = EvalValue();
                    ret["functionArg2"] = EvalValue();
                    break;
                default:
                    throw new Exception("Unknown value type: " + type);
            }
            return ret;
        }
    }

    class HumanReadableWorldStateExpression
    {
        private Dictionary<int, string> worldStateExpressionMap = new Dictionary<int, string>();
        private string[] logOps = { "none", "and", "or", "xor" };
        private string[] relOps = { "ID", "=", "≠", "<", "≤", ">", "≥" };
        private string[] ariOps = { "ID", "+", "-", "*", "/", "%" };
        private string[] valueTypes = { "0", "value", "world_state", "function" };

        public HumanReadableWorldStateExpression(Dictionary<int, string> worldStateExpressionMap = null)
        {
            if (worldStateExpressionMap != null)
                this.worldStateExpressionMap = worldStateExpressionMap;
        }

        public string StateToString(List<Dictionary<string, object>> states)
        {
            StringBuilder str = new StringBuilder();
            foreach (var state in states)
            {
                if (int.Parse(state["op"].ToString()) != 0)
                {
                    str.Append("(");
                    str.Append(RelationalToString((Dictionary<string, object>)state["relational"]));
                    str.Append(") ");
                    str.Append(logOps[Convert.ToInt32(state["op"])]);
                    str.Append(" ");
                }
                else
                {
                    str.Append(RelationalToString((Dictionary<string, object>)state["relational"]));
                    str.Append(")");
                }
            }
            return str.ToString();
        }

        private string FunctionDesc(int funcID, Dictionary<string, object> val1, Dictionary<string, object> val2)
        {
            int arg1 = Convert.ToInt32(val1["value"]);
            int arg2 = Convert.ToInt32(val2["value"]);
            switch (funcID)
            {
                case 0:
                    return "0";
                case 1:
                    return "random(min: " + arg1 + ", max: " + arg2 + ")";
                case 2:
                    return "now.month()";
                case 3:
                    return "now.day()";
                case 4:
                    return "now.time_of_day()";
                case 5:
                    return "region";
                case 6:
                    return "now.imperial_hours()";
                case 7:
                    return "difficulty_id_old()";
                case 8:
                    return "holiday_start(holiday_id: " + ValueToString(val1) + ", duration_id: " + arg2 + ")";
                case 9:
                    return "holiday_left(holiday_id: " + ValueToString(val1) + ", duration_id: " + arg2 + ")";
                case 10:
                    return "holiday_active(holiday_id: " + ValueToString(val1) + ")";
                case 11:
                    return "now()";
                case 12:
                    return "week_number()";
                case 15:
                    return "difficulty_id()";
                case 16:
                    return "warmode_active()";
                case 22:
                    if (worldStateExpressionMap.Count > 0)
                    {
                        if (worldStateExpressionMap.ContainsKey(arg1))
                        {
                            var inlineExp = new WSExpressionParser(worldStateExpressionMap[arg1]);
                            return StateToString(inlineExp.state.Values.ToList());
                        }
                        else
                        {
                            return "missingInlineExpression(" + arg1 + ")";
                        }
                    }
                    else
                    {
                        return "inlineExpression(" + arg1 + ")";
                    }
                case 23:
                    return "keystone_affix()";
                case 28:
                    return "keystone_level()";
                case 33:
                    return "random(max: " + arg1 + ", seed: " + arg2 + ")";
                default:
                    string unkStr = "unknownFunction_" + funcID + "(";
                    unkStr += ValueToString(val1);
                    unkStr += ", " + ValueToString(val2);
                    unkStr += ")";
                    return unkStr;
            }
        }

        private string RelationalToString(Dictionary<string, object> relational)
        {
            StringBuilder str = new StringBuilder("(");
            Dictionary<string, object> arethmatic = (Dictionary<string, object>)relational["arethmatic"];
            if (Convert.ToInt32(arethmatic["op"]) != 0)
            {
                str.Append(ValueToString((Dictionary<string, object>)arethmatic["value"]));
                str.Append(" ");
                str.Append(ariOps[Convert.ToInt32(arethmatic["op"])]);
                str.Append(" ");
                str.Append(ValueToString((Dictionary<string, object>)arethmatic["subValue"]));
            }
            else
            {
                str.Append(ValueToString((Dictionary<string, object>)arethmatic["value"]));
            }
            if (Convert.ToInt32(relational["op"]) != 0)
            {
                str.Append(" ");
                str.Append(relOps[Convert.ToInt32(relational["op"])]);
                str.Append(" ");
                Dictionary<string, object> subArethmatic = (Dictionary<string, object>)relational["subArethmatic"];
                if (Convert.ToInt32(subArethmatic["op"]) != 0)
                {
                    str.Append(ValueToString((Dictionary<string, object>)subArethmatic["value"]));
                    str.Append(" ");
                    str.Append(ariOps[Convert.ToInt32(subArethmatic["op"])]);
                    str.Append(" ");
                    str.Append(ValueToString((Dictionary<string, object>)subArethmatic["subValue"]));
                }
                else
                {
                    str.Append(ValueToString((Dictionary<string, object>)subArethmatic["value"]));
                }
            }
            return str.ToString();
        }

        private string ValueToString(Dictionary<string, object> value)
        {
            switch (Convert.ToInt32(value["type"]))
            {
                case 0:
                    return "0";
                case 1:
                    return value["value"].ToString();
                case 2:
                    return valueTypes[Convert.ToInt32(value["type"])] + "(" + value["value"] + ")";
                case 3:
                    return FunctionDesc(Convert.ToInt32(value["function"]), (Dictionary<string, object>)value["functionArg1"], (Dictionary<string, object>)value["functionArg2"]);
                default:
                    return "";
            }
        }
    }
}

