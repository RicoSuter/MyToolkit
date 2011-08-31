// Copyright (C) Microsoft Corporation. All Rights Reserved.
// This code released under the terms of the Microsoft Public License
// (Ms-PL, http://opensource.org/licenses/ms-pl.html).

using System.Windows;
using System.Windows.Controls;

namespace MyToolkit.Performance
{
    /// <summary>
    /// Implements a subclass of ListBoxItem that is used in conjunction with
    /// the DeferredLoadListBox to defer the loading of off-screen items.
    /// </summary>
    public class DeferredLoadListBoxItem : ListBoxItem
    {
        private object _maskedContent;
        private DataTemplate _maskedContentTemplate;

        internal bool Masked { get; set; }

        internal void MaskContent()
        {
            if (!Masked)
            {
                _maskedContent = Content;
                _maskedContentTemplate = ContentTemplate;
                Content = null;
                ContentTemplate = null;
                Masked = true;
            }
        }

        internal void UnmaskContent()
        {
            if (Masked)
            {
                ContentTemplate = _maskedContentTemplate;
                Content = _maskedContent;
                _maskedContentTemplate = null;
                _maskedContent = null;
                Masked = false;
            }
        }
    }
}
