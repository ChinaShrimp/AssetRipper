﻿using AssetRipper.Core.Project;
using AssetRipper.Core.Project.Exporters;
using AssetRipper.Core.Classes;
using AssetRipper.Core.Classes.Meta;
using AssetRipper.Core.Utils;
using System;
using System.IO;
using Object = AssetRipper.Core.Classes.Object.Object;

namespace AssetRipper.Core.Structure.Collections
{
	public class ManagerExportCollection : AssetExportCollection
	{
		public ManagerExportCollection(IAssetExporter assetExporter, Object asset) : this(assetExporter, (GlobalGameManager)asset) { }

		public ManagerExportCollection(IAssetExporter assetExporter, GlobalGameManager asset) : base(assetExporter, asset) { }

		public override bool Export(ProjectAssetContainer container, string dirPath)
		{
			string subPath = Path.Combine(dirPath, ProjectSettingsName);
			string fileName = $"{Asset.ExportPath}.asset";
			string filePath = Path.Combine(subPath, fileName);

			if (!DirectoryUtils.Exists(subPath))
			{
				DirectoryUtils.CreateVirtualDirectory(subPath);
			}

			ExportInner(container, filePath);
			return true;
		}

		public override long GetExportID(Object asset)
		{
			if (asset == Asset)
			{
				return 1;
			}
			throw new ArgumentException(nameof(asset));
		}

		public override MetaPtr CreateExportPointer(Object asset, bool isLocal)
		{
			throw new NotSupportedException();
		}

		protected const string ProjectSettingsName = "ProjectSettings";
	}
}
