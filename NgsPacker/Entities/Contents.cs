// -----------------------------------------------------------------------
// <copyright file="Contents.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace NgsPacker.Entities
{
    /// <summary>
    /// ファイル内容物のエンティティ
    /// </summary>
    public class Contents
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// ファイルのID
        /// </summary>
        [Required]
        public string FileId { get; set; }

        /// <summary>
        /// 内容物のファイル名
        /// </summary>
        [Required]
        public string name { get; set; }
    }
}