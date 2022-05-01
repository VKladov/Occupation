using System;

public static class TempData
{
	public static Property<int> TeamID = new Property<int>(0);
}

public class Property<T>
{
	public event Action Changed; 
	private T _value;

	public Property(T value)
	{
		_value = value;
	}

	public T Value
	{
		set
		{ 
			_value = value;
			Changed?.Invoke();
		}
		get => _value;
	}
}