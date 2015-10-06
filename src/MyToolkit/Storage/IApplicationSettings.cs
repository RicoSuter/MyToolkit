//-----------------------------------------------------------------------
// <copyright file="IApplicationSettings.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

namespace MyToolkit.Storage
{
    internal interface IApplicationSettings
    {
        void SetSetting<T>(string key, T value, bool roaming, bool save);
        T GetSetting<T>(string key, T defaultValue, bool roaming);
        bool HasSetting<T>(string key, bool roaming);
        bool RemoveSetting(string key, bool roaming, bool save);
        bool CanRoam { get; }
    }
}