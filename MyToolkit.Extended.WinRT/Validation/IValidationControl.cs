using System;

namespace MyToolkit.Validation
{
	public interface IValidationControl
	{
		Exception ValidationException { get; }
		event EventHandler<Exception> ValidationChanged;
	}
}