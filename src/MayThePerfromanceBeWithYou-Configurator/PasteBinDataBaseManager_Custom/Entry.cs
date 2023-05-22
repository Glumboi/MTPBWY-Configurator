namespace PasteBinDataBaseManager_Custom;

public struct Entry
{
    string[] _values = new string[]{};
    string[] _types  = new string[]{};
    string _identifier  = String.Empty;

    public Entry(string identifier, string[] types, string[] values)
    {
        _values = values;
        _types = types;
        _identifier = $"[{identifier}]";
    }

    public string GetIdentifier()
    {
        return _identifier.Replace("[", "").Replace("]", "");
    }
    
    public string GetValueOfType(string type)
    {
        for (var index = 0; index < _types.Length; index++)
        {
            if (_types[index] == type)
            {
                return _values[index];
            }
        }
        
        //Do something else
        throw null;
    }
    
    public string GetTypeOfValue(string value)
    {
        for (var index = 0; index < _types.Length; index++)
        {
            if (_values[index] == value)
            {
                return _types[index];
            }
        }

        //Do something else
        return null;
    }
    
    public string GetEntryInOneLine()
    {
        string result;
        string createdString = "Identifier: " + GetIdentifier() + " | ";
            
        if (_values == null) return string.Empty;

        for (var index = 0; index < _values.Length; index++)
        {
            var str = _values[index];
            createdString += _types[index]+ ": " + _values[index] + ", ";
        }

        result = createdString;
        return result;
    }
}