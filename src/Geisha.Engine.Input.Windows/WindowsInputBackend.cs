using System.Windows.Forms;
using Geisha.Engine.Input.Backend;

namespace Geisha.Engine.Input.Windows
{
    /// <summary>
    ///     Input backend implementation for Windows. This implementation depends on WinForms and Win32.
    /// </summary>
    public sealed class WindowsInputBackend : IInputBackend
    {
        private readonly Form _form;

        /// <summary>
        ///     Creates new instance of <see cref="WindowsInputBackend" /> that handles input for specified <see cref="Form" />
        ///     instance.
        /// </summary>
        /// <param name="form"><see cref="Form" /> instance for which input backend handles input.</param>
        public WindowsInputBackend(Form form)
        {
            _form = form;
        }

        /// <summary>
        ///     Creates input provider suitable for Windows platform.
        /// </summary>
        /// <returns>New instance of <see cref="IInputProvider" /> suitable for Windows platform.</returns>
        public IInputProvider CreateInputProvider()
        {
            return new InputProvider(_form);
        }
    }
}