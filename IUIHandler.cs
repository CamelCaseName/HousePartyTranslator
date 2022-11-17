using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UICompatibilityLayer
{
    public enum PopupResult
    {
        NONE,
        OK,
        YES,
        NO,
        CANCEL,
        ABORT,
        CONTINUE,
        IGNORE
    }

    public class MenuItems
    {
        public readonly List<IMenuItem> Items;
    }

    public interface IMenuItem
    {
        MenuItems d = new();
    }

    public interface IUIHandler
    {
        #region cursor
        void SignalUserWait();
        void SignalUserEndWait();

        #endregion

        #region message popup
        PopupResult InfoOk(string message, string title = "Info");
        PopupResult InfoOkCancel(string message, string title = "Info");
        PopupResult InfoYesNo(string message, string title = "Info");
        PopupResult InfoYesNoCancel(string message, string title = "Info");
        PopupResult WarningOk(string message, string title = "Warning");
        PopupResult WarningOkCancel(string message, string title = "Warning");
        PopupResult WarningYesNo(string message, string title = "Warning");
        PopupResult WarningYesNoCancel(string message, string title = "Warning");
        PopupResult ErrorOk(string message, string title = "Error");
        PopupResult ErrorOkCancel(string message, string title = "Error");
        PopupResult ErrorYesNo(string message, string title = "Error");
        PopupResult ErrorYesNoCancel(string message, string title = "Error");
        bool InfoOkCancel(string message, string title = "Info", PopupResult result = PopupResult.OK);
        bool InfoYesNo(string message, string title = "Info", PopupResult result = PopupResult.YES);
        bool InfoYesNoCancel(string message, string title = "Info", PopupResult result = PopupResult.YES);
        bool WarningOkCancel(string message, string title = "Warning", PopupResult result = PopupResult.OK);
        bool WarningYesNo(string message, string title = "Warning", PopupResult result = PopupResult.YES);
        bool WarningYesNoCancel(string message, string title = "Warning", PopupResult result = PopupResult.YES);
        bool ErrorOkCancel(string message, string title = "Error", PopupResult result = PopupResult.OK);
        bool ErrorYesNo(string message, string title = "Error", PopupResult result = PopupResult.YES);
        bool ErrorYesNoCancel(string message, string title = "Error", PopupResult result = PopupResult.YES);

        #endregion

        #region main window
        void SetTitle(string title);
        void GetFileMenuItems();
        #endregion

        #region list of translations

        #endregion

        #region translation textbox

        #endregion

        #region template textbox

        #endregion

        #region comment textbox

        #endregion

        #region line controls

        #endregion

        #region login user control

        #endregion

        #region search

        #endregion

        #region menubar

        #endregion

        #region file access/system access
        void SignalAppExit();
        #endregion

        #region tabs

        #endregion
    }
}
