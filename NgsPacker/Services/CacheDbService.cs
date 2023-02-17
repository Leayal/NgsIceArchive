// -----------------------------------------------------------------------
// <copyright file="CacheDbService.cs" company="Logue">
// Copyright (c) 2021-2023 Masashi Yoshikawa All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using ImTools;
using NgsPacker.Entities;
using NgsPacker.Helpers;
using NgsPacker.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NgsPacker.Services;

/// <summary>
///     ファイル名キャッシュサービス
/// </summary>
public class CacheDbService : ICacheDbService, IDisposable
{
    /// <summary>
    ///     Zamboniサービス
    /// </summary>
    private readonly IZamboniService zamboniService;

    /// <summary>
    ///     データベースのコンテキスト
    /// </summary>
    private CacheDbContext context = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="CacheDbService" /> class.
    /// </summary>
    /// <param name="zamboniService">Zamboniサービス</param>
    public CacheDbService(IZamboniService zamboniService)
    {
        context.Database.EnsureCreated();
        this.zamboniService = zamboniService;
    }

    /// <inheritdoc />
    public async Task ScanFileｓ(DataDirectoryType target = DataDirectoryType.Ngs, bool force = false)
    {
        List<string> entries =
            new(Directory.EnumerateFiles(IceUtility.GetDataDir(), "*.*", SearchOption.AllDirectories));
        Debug.WriteLine("Entries: ", entries.Count);

        // CSVのヘッダ
        foreach (string path in entries)
        {
            Debug.Print(path);

            // ディレクトリチェック
            if (IceUtility.IsTargetPath(path))
            {
                // 除外条件だった場合スキップ
                continue;
            }

            // Iceファイルをバイトとして読み込む
            byte[] buffer = await File.ReadAllBytesAsync(path);

            // Iceファイルのヘッダチェック
            if (IceUtility.IsIceFile(buffer))
            {
                // Iceファイルでない場合はスキップ
                continue;
            }

            // SHA256計算機
            using SHA256 cryptor = SHA256.Create();

            // SHA256ハッシュ
            byte[] hash = cryptor.ComputeHash(buffer);

            // ファイルのハッシュを文字列にする
            string fileHash = BitConverter.ToString(hash).Replace("-", string.Empty);

            // ファイルのエントリ
            IceFiles fileEntry = context.IceFiles.Single(x => x.Name == path);

            if (fileEntry.List().IsEmpty)
            {
                // 新規登録
                await context.IceFiles.AddAsync(new IceFiles
                {
                    Name = path, Hash = fileHash, UpdatedAt = File.GetLastWriteTime(path)
                });
            }
            else
            {
                // ファイルは保存されているが、ハッシュが異なる場合
                if (fileEntry.Hash != fileHash)
                {
                    // ハッシュと更新日時を更新
                    fileEntry.Hash = fileHash;
                    fileEntry.UpdatedAt = DateTime.UtcNow;
                }

                await context.SaveChangesAsync();
            }
        }
    }

    /// <inheritdoc />
    public Task ScanContents(DataDirectoryType target = DataDirectoryType.All)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     ファイナライズ
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     コンテキストを破棄する
    /// </summary>
    /// <param name="disposing">破棄中か</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing && context == null)
        {
            return;
        }

        context.Dispose();
        context = null;
    }
}
