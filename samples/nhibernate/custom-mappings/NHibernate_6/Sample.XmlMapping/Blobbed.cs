using System;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;

/// <summary>
/// https://gist.github.com/phillip-haydon/1936188
/// </summary>
[Serializable]
public class Blobbed<T> : IUserType where T : class
{
    public new bool Equals(object x, object y)
    {
        if (x == null && y == null)
            return true;
        if (x == null || y == null)
            return false;

        var xdocX = JsonConvert.SerializeObject(x);
        var xdocY = JsonConvert.SerializeObject(y);

        return xdocY == xdocX;
    }

    public int GetHashCode(object x)
    {
        if (x == null)
            return 0;

        return x.GetHashCode();
    }

    public object NullSafeGet(IDataReader rs, string[] names, object owner)
    {
        if (names.Length != 1)
            throw new InvalidOperationException("Only expecting one column...");

        var val = rs[names[0]] as string;

        if (val != null && !string.IsNullOrWhiteSpace(val))
        {
            return JsonConvert.DeserializeObject<T>(val);
        }

        return null;
    }

    public void NullSafeSet(IDbCommand cmd, object value, int index)
    {
        var parameter = (DbParameter)cmd.Parameters[index];

        if (value == null)
        {
            parameter.Value = DBNull.Value;
        }
        else
        {
            parameter.Value = JsonConvert.SerializeObject(value);
        }
    }

    public object DeepCopy(object value)
    {
        if (value == null)
            return null;

        //Serialized and Deserialized using json.net so that I don't
        //have to mark the class as serializable. Most likely slower
        //but only done for convenience.

        var serialized = JsonConvert.SerializeObject(value);

        return JsonConvert.DeserializeObject<T>(serialized);
    }

    public object Replace(object original, object target, object owner)
    {
        return original;
    }

    public object Assemble(object cached, object owner)
    {
        var str = cached as string;

        if (string.IsNullOrWhiteSpace(str))
            return null;

        return JsonConvert.DeserializeObject<T>(str);
    }

    public object Disassemble(object value)
    {
        if (value == null)
            return null;

        return JsonConvert.SerializeObject(value);
    }

    public SqlType[] SqlTypes => new SqlType[] { new StringClobSqlType() };

    public Type ReturnedType => typeof(T);

    public bool IsMutable => true;
}