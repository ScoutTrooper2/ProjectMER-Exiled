namespace ProjectMER.Features.Extensions;

public static class DictionaryExtensions
{
	public static bool TryAdd<DictKey, DictValue, T>(this Dictionary<DictKey, DictValue> dictionary, DictKey key, T value)
	{
		if (value is DictValue)
		{
			if (dictionary.ContainsKey(key))
				return false;

			dictionary.Add(key, (DictValue)(object)value!);
			return true;
		}

		return false;
	}
}
