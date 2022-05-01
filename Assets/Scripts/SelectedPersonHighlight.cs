using System;
using System.Collections.Generic;
using Zenject;

public class SelectedPersonHighlight : IDisposable
{
	private PersonSelection _selection;
	private List<Person> _highlighted = new List<Person>();
	
	[Inject]
	public SelectedPersonHighlight(PersonSelection selection)
	{
		_selection = selection;
		_selection.Changed += SelectionOnChanged;
	}

	public void Dispose()
	{
		_selection.Changed -= SelectionOnChanged;
	}

	private void SelectionOnChanged(List<Person> selected)
	{
		foreach (var person in _highlighted)
		{
			person.SetSelection(false);
		}

		_highlighted.Clear();
		foreach (var person in selected)
		{
			person.SetSelection(true);
			_highlighted.Add(person);
		}
	}
}