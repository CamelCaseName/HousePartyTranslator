namespace Translator.Helpers
{
	/// <summary>
	/// Helper class to make messagebox calls smaller and more concise
	/// </summary>
	public static class Msg
	{
		/// <summary>
		/// Shows a messagebox with the given message and title, info icon and ok button
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult InfoOk(string msg, string title = "Info") => MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Information);

		/// <summary>
		/// Shows a messagebox with the given message and title, info icon and ok and cancel buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult InfoOkCancel(string msg, string title = "Info") => MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

		/// <summary>
		/// Shows a messagebox with the given message and title, info icon and yes and no buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult InfoYesNo(string msg, string title = "Info") => MessageBox.Show(msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Information);

		/// <summary>
		/// Shows a messagebox with the given message and title, info icon and yes, no and cancel buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult InfoYesNoCancel(string msg, string title = "Info") => MessageBox.Show(msg, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

		/// <summary>
		/// Shows a messagebox with the given message and title, warning icon and ok button
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult WarningOk(string msg, string title = "Warning") => MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);

		/// <summary>
		/// Shows a messagebox with the given message and title, warning icon and ok and cancel buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult WarningOkCancel(string msg, string title = "Warning") => MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

		/// <summary>
		/// Shows a messagebox with the given message and title, warning icon and yes and no buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult WarningYesNo(string msg, string title = "Warning") => MessageBox.Show(msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

		/// <summary>
		/// Shows a messagebox with the given message and title, warning icon and yes, no and cancel buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult WarningYesNoCancel(string msg, string title = "Warning") => MessageBox.Show(msg, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

		/// <summary>
		/// Shows a messagebox with the given message and title, error icon and ok button
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult ErrorOk(string msg, string title = "Error") => MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Error);

		/// <summary>
		/// Shows a messagebox with the given message and title, error icon and ok and cancel buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult ErrorOkCancel(string msg, string title = "Error") => MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

		/// <summary>
		/// Shows a messagebox with the given message and title, error icon and yes and no buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult ErrorYesNo(string msg, string title = "Error") => MessageBox.Show(msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Error);

		/// <summary>
		/// Shows a messagebox with the given message and title, error icon and yes, no and cancel buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult ErrorYesNoCancel(string msg, string title = "Error") => MessageBox.Show(msg, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);

		/// <summary>
		/// Shows a messagebox with the given message and title, info icon, preferred default button, ok and cancel buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="def">preselected button</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult InfoOkCancel(string msg, string title = "Info", MessageBoxDefaultButton def = MessageBoxDefaultButton.Button2) => MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Information, def);

		/// <summary>
		/// Shows a messagebox with the given message and title, info icon, preferred default button, yes and no buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="def">preselected button</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult InfoYesNo(string msg, string title = "Info", MessageBoxDefaultButton def = MessageBoxDefaultButton.Button2) => MessageBox.Show(msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Information, def);

		/// <summary>
		/// Shows a messagebox with the given message and title, info icon, preferred default button, yes, no and cancel buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="def">preselected button</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult InfoYesNoCancel(string msg, string title = "Info", MessageBoxDefaultButton def = MessageBoxDefaultButton.Button3) => MessageBox.Show(msg, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, def);

		/// <summary>
		/// Shows a messagebox with the given message and title, warning icon, preferred default button, ok and cancel buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="def">preselected button</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult WarningOkCancel(string msg, string title = "Warning", MessageBoxDefaultButton def = MessageBoxDefaultButton.Button2) => MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, def);

		/// <summary>
		/// Shows a messagebox with the given message and title, warning icon, preferred default button, yes and no buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="def">preselected button</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult WarningYesNo(string msg, string title = "Warning", MessageBoxDefaultButton def = MessageBoxDefaultButton.Button2) => MessageBox.Show(msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, def);

		/// <summary>
		/// Shows a messagebox with the given message and title, warning icon, preferred default button, yes, no and cancel buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="def">preselected button</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult WarningYesNoCancel(string msg, string title = "Warning", MessageBoxDefaultButton def = MessageBoxDefaultButton.Button3) => MessageBox.Show(msg, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, def);

		/// <summary>
		/// Shows a messagebox with the given message and title, error icon, preferred default button, ok and cancel buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="def">preselected button</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult ErrorOkCancel(string msg, string title = "Error", MessageBoxDefaultButton def = MessageBoxDefaultButton.Button2) => MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Error, def);

		/// <summary>
		/// Shows a messagebox with the given message and title, error icon, preferred default button, yes and no buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="def">preselected button</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult ErrorYesNo(string msg, string title = "Error", MessageBoxDefaultButton def = MessageBoxDefaultButton.Button2) => MessageBox.Show(msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Error, def);

		/// <summary>
		/// Shows a messagebox with the given message and title, error icon, preferred default button, yes, no and cancel buttons
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="def">preselected button</param>
		/// <returns>the <see cref="DialogResult"/> with the button clicked by the user</returns>
		internal static DialogResult ErrorYesNoCancel(string msg, string title = "Error", MessageBoxDefaultButton def = MessageBoxDefaultButton.Button3) => MessageBox.Show(msg, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error, def);

		/// <summary>
		/// Shows a messagebox with the given message and title, info icon, ok and cancel buttons, then returns true if the user clicked the same button as asked for in the <paramref name="result"/>
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="result">a <see cref="DialogResult"/> which the clicked button is compared to</param>
		/// <returns>true if the user clicked the same button as asked for in the <paramref name="result"/></returns>
		internal static bool InfoOkCancelB(string msg, string title = "Info", DialogResult result = DialogResult.OK) => InfoOkCancel(msg, title) == result;

		/// <summary>
		/// Shows a messagebox with the given message and title, info icon, yes and no buttons, then returns true if the user clicked the same button as asked for in the <paramref name="result"/>
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="result">a <see cref="DialogResult"/> which the clicked button is compared to</param>
		/// <returns>true if the user clicked the same button as asked for in the <paramref name="result"/></returns>
		internal static bool InfoYesNoB(string msg, string title = "Info", DialogResult result = DialogResult.Yes) => InfoYesNo(msg, title) == result;

		/// <summary>
		/// Shows a messagebox with the given message and title, info icon, yes, no and cancel buttons, then returns true if the user clicked the same button as asked for in the <paramref name="result"/>
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="result">a <see cref="DialogResult"/> which the clicked button is compared to</param>
		/// <returns>true if the user clicked the same button as asked for in the <paramref name="result"/></returns>
		internal static bool InfoYesNoCancelB(string msg, string title = "Info", DialogResult result = DialogResult.Yes) => InfoYesNoCancel(msg, title) == result;

		/// <summary>
		/// Shows a messagebox with the given message and title, warning icon, ok and cancel buttons, then returns true if the user clicked the same button as asked for in the <paramref name="result"/>
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="result">a <see cref="DialogResult"/> which the clicked button is compared to</param>
		/// <returns>true if the user clicked the same button as asked for in the <paramref name="result"/></returns>
		internal static bool WarningOkCancelB(string msg, string title = "Warning", DialogResult result = DialogResult.OK) => WarningOkCancel(msg, title) == result;

		/// <summary>
		/// Shows a messagebox with the given message and title, warning icon, yes and no buttons, then returns true if the user clicked the same button as asked for in the <paramref name="result"/>
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="result">a <see cref="DialogResult"/> which the clicked button is compared to</param>
		/// <returns>true if the user clicked the same button as asked for in the <paramref name="result"/></returns>
		internal static bool WarningYesNoB(string msg, string title = "Warning", DialogResult result = DialogResult.Yes) => WarningYesNo(msg, title) == result;

		/// <summary>
		/// Shows a messagebox with the given message and title, warning icon, yes, no and cancel buttons, then returns true if the user clicked the same button as asked for in the <paramref name="result"/>
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="result">a <see cref="DialogResult"/> which the clicked button is compared to</param>
		/// <returns>true if the user clicked the same button as asked for in the <paramref name="result"/></returns>
		internal static bool WarningYesNoCancelB(string msg, string title = "Warning", DialogResult result = DialogResult.Yes) => WarningYesNoCancel(msg, title) == result;

		/// <summary>
		/// Shows a messagebox with the given message and title, error icon, ok and cancel buttons, then returns true if the user clicked the same button as asked for in the <paramref name="result"/>
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="result">a <see cref="DialogResult"/> which the clicked button is compared to</param>
		/// <returns>true if the user clicked the same button as asked for in the <paramref name="result"/></returns>
		internal static bool ErrorOkCancelB(string msg, string title = "Error", DialogResult result = DialogResult.OK) => ErrorOkCancel(msg, title) == result;

		/// <summary>
		/// Shows a messagebox with the given message and title, error icon, yes and no buttons, then returns true if the user clicked the same button as asked for in the <paramref name="result"/>
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="result">a <see cref="DialogResult"/> which the clicked button is compared to</param>
		/// <returns>true if the user clicked the same button as asked for in the <paramref name="result"/></returns>
		internal static bool ErrorYesNoB(string msg, string title = "Error", DialogResult result = DialogResult.Yes) => ErrorYesNo(msg, title) == result;

		/// <summary>
		/// Shows a messagebox with the given message and title, error icon, yes, no and cancel buttons, then returns true if the user clicked the same button as asked for in the <paramref name="result"/>
		/// </summary>
		/// <param name="msg">the message to display to the user</param>
		/// <param name="title">title of the window</param>
		/// <param name="result">a <see cref="DialogResult"/> which the clicked button is compared to</param>
		/// <returns>true if the user clicked the same button as asked for in the <paramref name="result"/></returns>
		internal static bool ErrorYesNoCancelB(string msg, string title = "Error", DialogResult result = DialogResult.Yes) => ErrorYesNoCancel(msg, title) == result;
	}
}
