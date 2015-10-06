//-----------------------------------------------------------------------
// <copyright file="ProjectTypeGuidMapper.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace MyToolkit.Build
{
    public enum VsProjectType
    {
        Unknown,

        WindowsCSharp,
        WindowsVBNet,
        WindowsCPlusPlus,

        WebApplication,
        WebSite,
        DistributedSystem,

        WindowsCommunicationFoundation,
        WindowsPresentationFoundation,

        VisualDatabaseTools,
        Database,
        DatabaseOther,

        Test,

        LegacySmartDevice,
        SmartDevice,

        Workflow,

        DeploymentMergeModule,
        DeploymentCab,
        DeploymentSetup,
        DeploymentSmartDeviceCab,

        VisualStudioToolsForApps,
        VisualStudioToolsForOffice,

        SharePointWorkflow,

        XnaWindows,
        XnaXBox,
        XnaZune,

        SharePoint,

        Silverlight,

        AspNetMvc1,
        AspNetMvc2,
        AspNetMvc3,
        AspNetMvc4,

        Extensibility,

        WindowsPhone81,
        WindowsPhone81Silverlight,
        StoreAppWindows81,
        StoreAppUniversal,
        StoreAppPortableUniversal,

        LightSwitch,
        LightSwitchProject,

        OfficeSharePointApp,
    }

    public static class ProjectTypeGuidMapper
    {
        private readonly static Dictionary<string, VsProjectType> _guids = new Dictionary<string, VsProjectType>
        {
            {"{CB4CE8C6-1BDB-4DC7-A4D3-65A1999772F8}", VsProjectType.LegacySmartDevice}, // VB.net 
            {"{4D628B5B-2FBC-4AA6-8C16-197242AEB884}", VsProjectType.SmartDevice}, // C# 
            {"{68B1623D-7FB9-47D8-8664-7ECEA3297D4F}", VsProjectType.SmartDevice}, // VB.net
            {"{14822709-B5A1-4724-98CA-57A101D1B079}", VsProjectType.Workflow}, // C# 
            {"{D59BE175-2ED0-4C54-BE3D-CDAA9F3214C8}", VsProjectType.Workflow}, // VB.net
            {"{06A35CCD-C46D-44D5-987B-CF40FF872267}", VsProjectType.DeploymentMergeModule}, 
            {"{3EA9E505-35AC-4774-B492-AD1749C4943A}", VsProjectType.DeploymentCab}, 
            {"{978C614F-708E-4E1A-B201-565925725DBA}", VsProjectType.DeploymentSetup}, 
            {"{AB322303-2255-48EF-A496-5904EB18DA55}", VsProjectType.DeploymentSmartDeviceCab}, 
            {"{A860303F-1F3F-4691-B57E-529FC101A107}", VsProjectType.VisualStudioToolsForApps}, 
            {"{BAA0C2D2-18E2-41B9-852F-F413020CAA33}", VsProjectType.VisualStudioToolsForOffice}, 
            {"{F8810EC1-6754-47FC-A15F-DFABD2E3FA90}", VsProjectType.SharePointWorkflow}, 
            {"{6D335F3A-9D43-41b4-9D22-F6F17C4BE596}", VsProjectType.XnaWindows}, 
            {"{2DF5C3F4-5A5F-47a9-8E94-23B4456F55E2}", VsProjectType.XnaXBox}, 
            {"{D399B71A-8929-442a-A9AC-8BEC78BB2433}", VsProjectType.XnaZune}, 
            {"{EC05E597-79D4-47f3-ADA0-324C4F7C7484}", VsProjectType.SharePoint}, // VB.net
            {"{593B0543-81F6-4436-BA1E-4747859CAAE2}", VsProjectType.SharePoint}, // C#
            {"{A1591282-1198-4647-A2B1-27E5FF5F6F3B}", VsProjectType.Silverlight}, 
            {"{603C0E0B-DB56-11DC-BE95-000D561079B0}", VsProjectType.AspNetMvc1}, 
            {"{F85E285D-A4E0-4152-9332-AB1D724D3325}", VsProjectType.AspNetMvc2}, 
            {"{E53F8FEA-EAE0-44A6-8774-FFD645390401}", VsProjectType.AspNetMvc3}, 
            {"{E3E379DF-F4C6-4180-9B81-6769533ABE47}", VsProjectType.AspNetMvc4}, 
            {"{82B43B9B-A64C-4715-B499-D71E9CA2BD60}", VsProjectType.Extensibility}, 
            {"{76F1466A-8B6D-4E39-A767-685A06062A39}", VsProjectType.WindowsPhone81}, 
            {"{C089C8C0-30E0-4E22-80C0-CE093F111A43}", VsProjectType.WindowsPhone81Silverlight}, // C#
            {"{DB03555F-0C8B-43BE-9FF9-57896B3C5E56}", VsProjectType.WindowsPhone81Silverlight}, // VB.net
            {"{BC8A1FFA-BEE3-4634-8014-F334798102B3}", VsProjectType.StoreAppWindows81}, 
            {"{D954291E-2A0B-460D-934E-DC6B0785DB48}", VsProjectType.StoreAppUniversal}, 
            {"{786C830F-07A1-408B-BD7F-6EE04809D6DB}", VsProjectType.StoreAppPortableUniversal}, 
            {"{8BB0C5E8-0616-4F60-8E55-A43933E57E9C}", VsProjectType.LightSwitch}, 
            {"{581633EB-B896-402F-8E60-36F3DA191C85}", VsProjectType.LightSwitchProject}, 
            {"{C1CDDADD-2546-481F-9697-4EA41081F2FC}", VsProjectType.OfficeSharePointApp}, 
            {"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", VsProjectType.WindowsCSharp}, 
            {"{F184B08F-C81C-45F6-A57F-5ABD9991F28F}", VsProjectType.WindowsVBNet}, 
            {"{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}", VsProjectType.WindowsCPlusPlus}, 
            {"{349C5851-65DF-11DA-9384-00065B846F21}", VsProjectType.WebApplication}, 
            {"{E24C65DC-7377-472B-9ABA-BC803B73C61A}", VsProjectType.WebSite}, 
            {"{F135691A-BF7E-435D-8960-F99683D2D49C}", VsProjectType.DistributedSystem}, 
            {"{3D9AD99F-2412-4246-B90B-4EAA41C64699}", VsProjectType.WindowsCommunicationFoundation}, 
            {"{60DC8134-EBA5-43B8-BCC9-BB4BC16C2548}", VsProjectType.WindowsPresentationFoundation}, 
            {"{C252FEB5-A946-4202-B1D4-9916A0590387}", VsProjectType.VisualDatabaseTools}, 
            {"{A9ACE9BB-CECE-4E62-9AA4-C7E7C5BD2124}", VsProjectType.Database}, 
            {"{4F174C21-8C12-11D0-8340-0000F80270F8}", VsProjectType.DatabaseOther}, 
            {"{3AC096D0-A1C2-E12C-1390-A8335801FDAB}", VsProjectType.Test}, 
            {"{20D4826A-C6FA-45DB-90F4-C717570B9F32}", VsProjectType.LegacySmartDevice}, // C#
        };

        /// <summary>Resolves the given GUID and returns a <see cref="VsProjectType"/>. </summary>
        /// <param name="guid">The GUID. </param>
        /// <returns>The <see cref="VsProjectType"/>. </returns>
        public static VsProjectType ResolveGuid(string guid)
        {
            guid = guid.ToUpperInvariant();
            return _guids.ContainsKey(guid) ? _guids[guid] : VsProjectType.Unknown;
        }
    }
}
