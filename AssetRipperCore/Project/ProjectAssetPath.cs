﻿using AssetRipper.Core.Classes.Utils;
using AssetRipper.Core.Classes.Utils.Extensions;
using AssetRipper.Core.Utils;
using System;
using System.IO;
using Object = AssetRipper.Core.Classes.Object.Object;

namespace AssetRipper.Core.Project
{
	public struct ProjectAssetPath
	{
		public ProjectAssetPath(string root, string path)
		{
			Root = root;
			AssetPath = path;
		}

		public string SubstituteExportPath(Object asset)
		{
			string projectPath = SubstitutePath(asset.GetOriginalName());
			projectPath = DirectoryUtils.FixInvalidPathCharacters(projectPath);
			return Path.Combine(Root, projectPath);
		}

		private string SubstitutePath(string assetName)
		{
			if (assetName.Length > 0 && assetName != AssetPath && AssetPath.EndsWith(assetName, StringComparison.OrdinalIgnoreCase))
			{
				if (assetName.Length == AssetPath.Length)
				{
					return assetName;
				}
				if (AssetPath[AssetPath.Length - assetName.Length - 1] == ObjectUtils.DirectorySeparatorChar)
				{
					string directoryPath = AssetPath.Substring(0, AssetPath.Length - assetName.Length);
					return directoryPath + assetName;
				}
			}
			return AssetPath;
		}

		public string Root { get; }
		public string AssetPath { get; }
	}
}
