﻿using AssetRipper.Core.Project;
using AssetRipper.Core.Project.Exporters;
using AssetRipper.Core.Logging;
using AssetRipper.Core.Parser.Asset;
using AssetRipper.Core.Classes;
using AssetRipper.Core.Classes.Meta;
using AssetRipper.Core.Classes.Meta.Importers.Asset;
using AssetRipper.Core.Classes.Misc;
using AssetRipper.Core.Parser.Files.SerializedFiles;
using AssetRipper.Core.IO.Asset;
using System;
using System.Collections.Generic;
using Object = AssetRipper.Core.Classes.Object.Object;

namespace AssetRipper.Core.Structure.Collections
{
	public class FailExportCollection : IExportCollection
	{
		public FailExportCollection(IAssetExporter assetExporter, Object asset)
		{
			if (assetExporter == null)
			{
				throw new ArgumentNullException(nameof(assetExporter));
			}
			if (asset == null)
			{
				throw new ArgumentNullException(nameof(asset));
			}

			AssetExporter = assetExporter;
			m_asset = asset;
		}

		public bool Export(ProjectAssetContainer container, string dirPath)
		{
			Logger.Log(LogType.Warning, LogCategory.Export, $"Unable to export asset {Name}");
			return false;
		}

		public bool IsContains(Object asset)
		{
			return asset == m_asset;
		}

		public long GetExportID(Object asset)
		{
			if (asset == m_asset)
			{
				return ExportCollection.GetMainExportID(m_asset);
			}
			throw new ArgumentException(nameof(asset));
		}

		public UnityGUID GetExportGUID(Object _)
		{
			throw new NotSupportedException();
		}

		public MetaPtr CreateExportPointer(Object asset, bool isLocal)
		{
			if (isLocal)
			{
				throw new ArgumentException(nameof(isLocal));
			}

			long exportId = GetExportID(asset);
			AssetType type = AssetExporter.ToExportType(asset);
			return new MetaPtr(exportId, UnityGUID.MissingReference, type);
		}

		public IAssetExporter AssetExporter { get; }
		public ISerializedFile File => m_asset.File;
		public TransferInstructionFlags Flags => File.Flags;
		public IEnumerable<Object> Assets
		{
			get { yield return m_asset; }
		}
		public string Name => m_asset is NamedObject namedAsset ? namedAsset.ValidName : m_asset.GetType().Name;
		public AssetImporter MetaImporter => throw new NotSupportedException();

		private readonly Object m_asset;
	}
}
