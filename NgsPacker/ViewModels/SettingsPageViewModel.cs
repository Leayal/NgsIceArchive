// -----------------------------------------------------------------------
// <copyright file="SettingsPage.cs" company="Logue">
// Copyright (c) 2021 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using NgsPacker.Interfaces;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Globalization;

namespace NgsPacker.ViewModels
{
    public class SettingsPageViewModel : BindableBase
    {
        /// <summary>
        /// 多言語化サービス.
        /// </summary>
        private readonly ILocalizerService localizerService;

        /// <summary>
        /// 対応言語.
        /// </summary>
        public IList<CultureInfo> SupportedLanguages => localizerService.SupportedLanguages;

        /// <summary>
        /// 完了時に通知を出す.
        /// </summary>
        public static bool ToggleNotifyComplete { get => Properties.Settings.Default.NotifyComplete; set => Properties.Settings.Default.NotifyComplete = value; }

        /// <summary>
        /// 選択されている言語..
        /// </summary>
        public CultureInfo SelectedLanguage
        {
            get => localizerService != null ? localizerService.SelectedLanguage : null;
            set
            {
                if (localizerService != null && value != null && value != localizerService.SelectedLanguage)
                {
                    localizerService.SelectedLanguage = value;
                    Properties.Settings.Default.Language = value.ToString();
                }
            }
        }

        /// <summary>
        /// コンストラクタ.
        /// </summary>
        /// <param name="localizerService">多言語化サービス.</param>
        public SettingsPageViewModel(ILocalizerService localizerService)
        {
            // 多言語化サービスのインジェクション
            this.localizerService = localizerService;
        }
    }
}
