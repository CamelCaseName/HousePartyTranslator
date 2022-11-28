using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translator.Core;
using Translator.UICompatibilityLayer;

namespace TranslatorAdmin.InterfaceImpls
{
    internal class WinUIHandler : IUIHandler<WinLineItem>
    {
        public MenuItems FileMenuItems { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string ReplaceBarText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string SearchBarText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public CreateTemplateDataDelegate CreateTemplateData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ITabController<WinLineItem> TabControl => throw new NotImplementedException();

        public string Language { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool ReplaceBarIsVisible => throw new NotImplementedException();

        MenuItems IUIHandler<WinLineItem>.FileMenuItems { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string IUIHandler<WinLineItem>.ReplaceBarText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string IUIHandler<WinLineItem>.SearchBarText { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Type IUIHandler<WinLineItem>.InternalFileDialogType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Type IUIHandler<WinLineItem>.InternalFolderDialogType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Type IUIHandler<WinLineItem>.InternalSaveFileDialogType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        CreateTemplateDataDelegate IUIHandler<WinLineItem>.CreateTemplateData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        ITabController<WinLineItem> IUIHandler<WinLineItem>.TabControl => throw new NotImplementedException();

        string IUIHandler<WinLineItem>.Language { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        bool IUIHandler<WinLineItem>.ReplaceBarIsVisible => throw new NotImplementedException();

        public void ClipboardSetText(string text) => throw new NotImplementedException();
        public ITab<WinLineItem>? CreateNewTab() => throw new NotImplementedException();
        public PopupResult ErrorOk(string message, string title = "Error") => throw new NotImplementedException();
        public PopupResult ErrorOkCancel(string message, string title = "Error") => throw new NotImplementedException();
        public bool ErrorOkCancel(string message, string title = "Error", PopupResult result = PopupResult.OK) => throw new NotImplementedException();
        public PopupResult ErrorYesNo(string message, string title = "Error") => throw new NotImplementedException();
        public bool ErrorYesNo(string message, string title = "Error", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public PopupResult ErrorYesNoCancel(string message, string title = "Error") => throw new NotImplementedException();
        public bool ErrorYesNoCancel(string message, string title = "Error", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public void FocusReplaceBar() => throw new NotImplementedException();
        public void FocusSearchBar() => throw new NotImplementedException();
        public PopupResult InfoOk(string message, string title = "Info") => throw new NotImplementedException();
        public PopupResult InfoOkCancel(string message, string title = "Info") => throw new NotImplementedException();
        public bool InfoOkCancel(string message, string title = "Info", PopupResult result = PopupResult.OK) => throw new NotImplementedException();
        public PopupResult InfoYesNo(string message, string title = "Info") => throw new NotImplementedException();
        public bool InfoYesNo(string message, string title = "Info", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public PopupResult InfoYesNoCancel(string message, string title = "Info") => throw new NotImplementedException();
        public bool InfoYesNoCancel(string message, string title = "Info", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public bool Login() => throw new NotImplementedException();
        public bool Logout() => throw new NotImplementedException();
        public void SetReplaceMenuInVisible() => throw new NotImplementedException();
        public void SetReplaceMenuVisible() => throw new NotImplementedException();
        public void SetTitle(string title) => throw new NotImplementedException();
        public void SignalAppExit() => throw new NotImplementedException();
        public void SignalUserEndWait() => throw new NotImplementedException();
        public void SignalUserWait() => throw new NotImplementedException();
        public void Update() => throw new NotImplementedException();
        public void UpdateProgress() => throw new NotImplementedException();
        public void UpdateResults() => throw new NotImplementedException();
        public PopupResult WarningOk(string message, string title = "Warning") => throw new NotImplementedException();
        public PopupResult WarningOkCancel(string message, string title = "Warning") => throw new NotImplementedException();
        public bool WarningOkCancel(string message, string title = "Warning", PopupResult result = PopupResult.OK) => throw new NotImplementedException();
        public PopupResult WarningYesNo(string message, string title = "Warning") => throw new NotImplementedException();
        public bool WarningYesNo(string message, string title = "Warning", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        public PopupResult WarningYesNoCancel(string message, string title = "Warning") => throw new NotImplementedException();
        public bool WarningYesNoCancel(string message, string title = "Warning", PopupResult result = PopupResult.YES) => throw new NotImplementedException();
        void IUIHandler<WinLineItem>.ClipboardSetText(string text) => throw new NotImplementedException();
        ITab<WinLineItem>? IUIHandler<WinLineItem>.CreateNewTab() => throw new NotImplementedException();
        PopupResult IUIHandler<WinLineItem>.ErrorOk(string message, string title) => throw new NotImplementedException();
        PopupResult IUIHandler<WinLineItem>.ErrorOkCancel(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<WinLineItem>.ErrorOkCancel(string message, string title, PopupResult result) => throw new NotImplementedException();
        PopupResult IUIHandler<WinLineItem>.ErrorYesNo(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<WinLineItem>.ErrorYesNo(string message, string title, PopupResult result) => throw new NotImplementedException();
        PopupResult IUIHandler<WinLineItem>.ErrorYesNoCancel(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<WinLineItem>.ErrorYesNoCancel(string message, string title, PopupResult result) => throw new NotImplementedException();
        void IUIHandler<WinLineItem>.FocusReplaceBar() => throw new NotImplementedException();
        void IUIHandler<WinLineItem>.FocusSearchBar() => throw new NotImplementedException();
        PopupResult IUIHandler<WinLineItem>.InfoOk(string message, string title) => throw new NotImplementedException();
        PopupResult IUIHandler<WinLineItem>.InfoOkCancel(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<WinLineItem>.InfoOkCancel(string message, string title, PopupResult result) => throw new NotImplementedException();
        PopupResult IUIHandler<WinLineItem>.InfoYesNo(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<WinLineItem>.InfoYesNo(string message, string title, PopupResult result) => throw new NotImplementedException();
        PopupResult IUIHandler<WinLineItem>.InfoYesNoCancel(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<WinLineItem>.InfoYesNoCancel(string message, string title, PopupResult result) => throw new NotImplementedException();
        bool IUIHandler<WinLineItem>.Login() => throw new NotImplementedException();
        bool IUIHandler<WinLineItem>.Logout() => throw new NotImplementedException();
        void IUIHandler<WinLineItem>.SetReplaceMenuInVisible() => throw new NotImplementedException();
        void IUIHandler<WinLineItem>.SetReplaceMenuVisible() => throw new NotImplementedException();
        void IUIHandler<WinLineItem>.SetTitle(string title) => throw new NotImplementedException();
        void IUIHandler<WinLineItem>.SignalAppExit() => throw new NotImplementedException();
        void IUIHandler<WinLineItem>.SignalUserEndWait() => throw new NotImplementedException();
        void IUIHandler<WinLineItem>.SignalUserWait() => throw new NotImplementedException();
        void IUIHandler<WinLineItem>.Update() => throw new NotImplementedException();
        void IUIHandler<WinLineItem>.UpdateProgress() => throw new NotImplementedException();
        void IUIHandler<WinLineItem>.UpdateResults() => throw new NotImplementedException();
        PopupResult IUIHandler<WinLineItem>.WarningOk(string message, string title) => throw new NotImplementedException();
        PopupResult IUIHandler<WinLineItem>.WarningOkCancel(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<WinLineItem>.WarningOkCancel(string message, string title, PopupResult result) => throw new NotImplementedException();
        PopupResult IUIHandler<WinLineItem>.WarningYesNo(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<WinLineItem>.WarningYesNo(string message, string title, PopupResult result) => throw new NotImplementedException();
        PopupResult IUIHandler<WinLineItem>.WarningYesNoCancel(string message, string title) => throw new NotImplementedException();
        bool IUIHandler<WinLineItem>.WarningYesNoCancel(string message, string title, PopupResult result) => throw new NotImplementedException();
    }
}
